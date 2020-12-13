using System.Collections.Generic;
using System.Net.Mail;

namespace Zhp.Office.AccountManagement.Domain.Services
{
    public interface IMailAddressGenerator
    {
        IEnumerable<MailAddress> GetPossibleAddressesForUser(string firstName, string lastName);
    }

    public class MailAddressGenerator : IMailAddressGenerator
    {
        public IEnumerable<MailAddress> GetPossibleAddressesForUser(string firstName, string lastName)
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
