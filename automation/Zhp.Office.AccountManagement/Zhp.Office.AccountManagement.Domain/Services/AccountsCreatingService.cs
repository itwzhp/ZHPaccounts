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

        public AccountsCreatingService(IAccountManager accountManager, ITicketRepository ticketRepository)
        {
            this.accountManager = accountManager;
            this.ticketRepository = ticketRepository;
        }

        public async Task CreateAccounts(CancellationToken token)
        {
            var tickets = await ticketRepository.GetApprovedActivationRequests(token);
            token.ThrowIfCancellationRequested();

            await Task.WhenAll(tickets.Select(t => HandleTicket(t, token)).ToList());
        }

        private async Task<(MailAddress mail, string password)> HandleTicket(ActivationRequest ticket, CancellationToken token)
        {
            try
            {
                var possibleMails = GetPossibleMails(ticket.FirstName, ticket.LastName);
                var password = "aaa";

                var addedMailAddress = await possibleMails.ToAsyncEnumerable().FirstOrDefaultAwaitAsync(
                    mail => accountManager.TryAddUser(ticket, mail, password, token),
                    token);

                return (addedMailAddress, password);
            }
            catch
            {
                //mark failed
            }
            return (null, null);
        }

        private static IEnumerable<MailAddress> GetPossibleMails(string firstName, string lastName)
        {
            firstName = Clean(firstName);
            lastName = Clean(lastName);

            yield return new MailAddress($"{firstName}.{lastName}@zhp.net.pl");
            yield return new MailAddress($"{lastName}.{firstName}@zhp.net.pl");
            yield return new MailAddress($"{firstName[0]}.{lastName}@zhp.net.pl");
            for (int i = 1; i <= 99; i++)
                yield return new MailAddress($"{firstName}.{lastName}{i}@zhp.net.pl");
        }

        private static string Clean(string text)
            => text
                .ToLowerInvariant()
                .Replace(" ", string.Empty);
    }
}
