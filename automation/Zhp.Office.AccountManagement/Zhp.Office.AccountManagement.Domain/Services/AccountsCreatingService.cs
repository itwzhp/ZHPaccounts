using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using Zhp.Office.AccountManagement.Domain.Ports;
using Zhp.Office.AccountManagement.Model;

namespace Zhp.Office.AccountManagement.Domain.Services
{
    public class AccountsCreatingService
    {
        private readonly IAccountManager accountManager;
        private readonly ITicketRepository ticketRepository;
        private readonly ILogger<AccountsCreatingService> logger;
        private readonly IPasswordGenerator passwordGenerator;
        private readonly IMailAddressGenerator mailAddressGenerator;
        private readonly ICommentFormatter commentFormatter;

        public AccountsCreatingService(
            IAccountManager accountManager,
            ITicketRepository ticketRepository,
            ILogger<AccountsCreatingService> logger,
            IPasswordGenerator passwordGenerator,
            IMailAddressGenerator mailAddressGenerator,
            ICommentFormatter commentFormatter)
        {
            this.accountManager = accountManager;
            this.ticketRepository = ticketRepository;
            this.logger = logger;
            this.passwordGenerator = passwordGenerator;
            this.mailAddressGenerator = mailAddressGenerator;
            this.commentFormatter = commentFormatter;
        }

        public async Task CreateAccounts(CancellationToken token)
        {
            IReadOnlyCollection<ActivationRequest> tickets;
            do
            {
                token.ThrowIfCancellationRequested();

                tickets = await ticketRepository.GetApprovedActivationRequests(token);
                logger.LogInformation($"Received {tickets.Count} requests.");

                token.ThrowIfCancellationRequested();

                var (validTickets, duplicates) = FindDuplicates(tickets);
                logger.LogInformation($"Found {duplicates.Count} duplicated requests.");

                foreach (var duplicate in duplicates)
                    await HandleDuplicate(duplicate, token);

                foreach (var ticket in validTickets)
                    await HandleTicket(ticket, token);

                logger.LogInformation($"Batch finished");
            } while (tickets.Any());
        }

        private static (IReadOnlyCollection<ActivationRequest> valid, IReadOnlyCollection<ActivationRequest> duplicates) FindDuplicates(IReadOnlyCollection<ActivationRequest> tickets)
        {
            var validTickets = tickets.GroupBy(t => t.MembershipNumber).Select(g => g.First()).ToList();
            var validTicketIds = validTickets.Select(t => t.Id).ToHashSet();

            var duplicates = tickets.Where(t => !validTicketIds.Contains(t.Id)).ToList();

            return (validTickets, duplicates);
        }

        private async Task HandleTicket(ActivationRequest ticket, CancellationToken token)
        {
            try
            {
                logger.LogDebug($"Handling ticket {ticket.Id}...");
                var possibleMails = mailAddressGenerator.GetPossibleAddressesForUser(ticket.FirstName, ticket.LastName);
                logger.LogDebug($"Addresses generated.");

                var password = passwordGenerator.GeneratePassword();
                logger.LogDebug($"Password generated.");

                MailAddress? addedMailAddress = null;
                foreach (var mail in possibleMails)
                {
                    if(await accountManager.TryAddUser(ticket, mail, password, token))
                    {
                        addedMailAddress = mail;
                        break;
                    }
                }

                if (addedMailAddress == null)
                    throw new Exception("Unknown error, unable to create user");

                var comment = commentFormatter.GetMailCreatedComment(addedMailAddress, password, ticket);
                await ticketRepository.MarkAsDone(ticket.Id, comment);
            }
            catch (OperationCanceledException) when (token.IsCancellationRequested) { throw; }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Unable to create account for {ticket.Id}");

                await ticketRepository.MarkForManualReview(ticket.Id, ex.Message);
            }
        }

        private async Task HandleDuplicate(ActivationRequest ticket, CancellationToken token)
           => await ticketRepository.MarkForManualReview(ticket.Id, "Prawdopodobnie duplikat innego zgłoszenia", token);
    }
}
