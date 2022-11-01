using Diacritics.Extensions;
using System.Collections.Generic;
using System.Linq;
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

            yield return new MailAddress($"{firstName}.{lastName}@zhp.pl");
            yield return new MailAddress($"{lastName}.{firstName}@zhp.pl");
            yield return new MailAddress($"{firstName[0]}.{lastName}@zhp.pl");
            for (int i = 1; i <= 99; i++)
                yield return new MailAddress($"{firstName}.{lastName}{i}@zhp.pl");
        }

        private static string Clean(string text)
            => string.Join(string.Empty,
                text
                .ToLowerInvariant()
                .Replace(" ", string.Empty)
                .RemoveDiacritics()
                .Where(c => char.IsLetter(c) || c == '-'));
    }
}
