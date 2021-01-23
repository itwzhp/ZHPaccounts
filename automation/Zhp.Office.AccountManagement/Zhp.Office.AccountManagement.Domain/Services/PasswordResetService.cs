using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Zhp.Office.AccountManagement.Domain.Ports;
using Zhp.Office.AccountManagement.Model;

namespace Zhp.Office.AccountManagement.Domain.Services
{
    public class PasswordResetService
    {
        private readonly ITicketRepository ticketRepository;
        private readonly IPasswordGenerator passwordGenerator;
        private readonly IAccountManager accountManager;
        private readonly ICommentFormatter commentFormatter;
        private readonly ILogger<PasswordResetService> logger;

        public PasswordResetService(ITicketRepository ticketRepository,
            IPasswordGenerator passwordGenerator,
            IAccountManager accountManager,
            ICommentFormatter commentFormatter,
            ILogger<PasswordResetService> logger)
        {
            this.ticketRepository = ticketRepository;
            this.passwordGenerator = passwordGenerator;
            this.accountManager = accountManager;
            this.commentFormatter = commentFormatter;
            this.logger = logger;
        }

        public async Task ResetPasswords(CancellationToken token)
        {
            IReadOnlyCollection<PasswordResetRequest> tickets;
            do
            {
                tickets = await ticketRepository.GetApprovedPasswordResetRequests(token);

                foreach (var ticket in tickets)
                {
                    await Handle(ticket, token);
                }    
            } while (tickets.Count > 0);
        }

        private async Task Handle(PasswordResetRequest ticket, CancellationToken token)
        {
            try
            {
                token.ThrowIfCancellationRequested();

                var password = passwordGenerator.GeneratePassword();

                await accountManager.ResetPassword(ticket.MailAddress, password, token);

                var comment = commentFormatter.GetPasswordResetComment(ticket.MailAddress, password);
                await ticketRepository.MarkAsDone(ticket.Id, comment, token);
            }
            catch (OperationCanceledException) when (token.IsCancellationRequested) { throw; }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Unable to reset password for {ticket.Id}");

                await ticketRepository.MarkForManualReview(ticket.Id, ex.Message);
            }
        }
    }
}
