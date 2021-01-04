using System.Collections.Generic;
using System.Linq;
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

        public PasswordResetService(ITicketRepository ticketRepository,
            IPasswordGenerator passwordGenerator,
            IAccountManager accountManager,
            ICommentFormatter commentFormatter)
        {
            this.ticketRepository = ticketRepository;
            this.passwordGenerator = passwordGenerator;
            this.accountManager = accountManager;
            this.commentFormatter = commentFormatter;
        }

        public async Task ResetPasswords(CancellationToken token)
        {
            IReadOnlyCollection<PasswordResetRequest> tickets;
            do
            {
                tickets = await ticketRepository.GetApprovedPasswordResetRequests(token);
                token.ThrowIfCancellationRequested();

                await Task.WhenAll(tickets.Select(t => Handle(t, token)));
            } while (tickets.Count > 0);
        }

        private async Task Handle(PasswordResetRequest ticket, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            var password = passwordGenerator.GeneratePassword();

            await accountManager.ResetPassword(ticket.MailAddress, password, token);

            var comment = commentFormatter.GetPasswordResetComment(ticket.MailAddress, password);
            await ticketRepository.MarkAsDone(ticket.Id, comment, token);          
        }
    }
}
