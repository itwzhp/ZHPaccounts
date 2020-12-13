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

        public AccountsCreatingService(
            IAccountManager accountManager,
            ITicketRepository ticketRepository,
            ILogger<AccountsCreatingService> logger,
            IPasswordGenerator passwordGenerator,
            IMailAddressGenerator mailAddressGenerator)
        { 
            this.accountManager = accountManager;
            this.ticketRepository = ticketRepository;
            this.logger = logger;
            this.passwordGenerator = passwordGenerator;
            this.mailAddressGenerator = mailAddressGenerator;
        }

        public async Task CreateAccounts(CancellationToken token)
        {
            var tickets = await ticketRepository.GetApprovedActivationRequests(token);
            token.ThrowIfCancellationRequested();

            await Task.WhenAll(tickets.Select(t => HandleTicket(t, token)).ToList());
        }

        private async Task HandleTicket(ActivationRequest ticket, CancellationToken token)
        {
            try
            {
                var possibleMails = mailAddressGenerator.GetPossibleAddressesForUser(ticket.FirstName, ticket.LastName);
                var password = passwordGenerator.GeneratePassword();

                var addedMailAddress = await possibleMails.ToAsyncEnumerable()
                    .FirstOrDefaultAwaitAsync(
                        mail => accountManager.TryAddUser(ticket, mail, password, token),
                        token);

                if (addedMailAddress == null)
                    throw new Exception("Unknown error, unable to create user");

                var comment = $"login: {addedMailAddress}\nhasło: {password}";
                await ticketRepository.MarkAsDone(ticket.Id, comment, token);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Unable to create account for {ticket.Id}");

                await ticketRepository.MarkForManualReview(ticket.Id, ex.Message, token);
            }
        }
    }
}
