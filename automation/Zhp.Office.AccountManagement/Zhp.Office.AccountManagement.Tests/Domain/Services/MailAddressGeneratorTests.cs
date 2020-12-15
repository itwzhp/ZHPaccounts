using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using Xunit;
using Zhp.Office.AccountManagement.Domain.Services;

namespace Zhp.Office.AccountManagement.Tests.Domain.Services
{
    public class MailAddressGeneratorTests
    {
        private readonly IMailAddressGenerator subject = new MailAddressGenerator();

        public static IEnumerable<object[]> TestData => new[]
        {
            new object[]{"Jan", "Kowalski"},
            new object[]{"Jan", "KOWALSKI" },
            new object[]{" Jan  ", " KOWALSKI  "},
            new object[]{ "Jan", "Kowalski-Nowak" },
            new object[]{ "Jan", " Kowalski - nowak" },
            new object[]{ "Grażyna", "Ąężćńłśó" },
        };

        [Theory]
        [MemberData(nameof(TestData))]
        public void GetPossibleAddressesForUser_CreatesAllAddresses(string firstName, string lastName)
        {
            var adresses = subject.GetPossibleAddressesForUser(firstName, lastName).ToList();

            adresses.Should().HaveCountGreaterOrEqualTo(100);
        }

        [Theory]
        [MemberData(nameof(TestData))]
        public void GetPossibleAddressesForUser_AllAddressesAreLowercase(string firstName, string lastName)
        {
            var adresses = subject.GetPossibleAddressesForUser(firstName, lastName).ToList();

            adresses.Should().OnlyContain(m => m.ToString() == m.ToString().ToLower());
        }

        [Theory]
        [InlineData(" Jan  ", " KOWALSKI  ", "jan.kowalski@zhp.net.pl")]
        [InlineData( "Jan", "Kowalski-Nowak","jan.kowalski-nowak@zhp.net.pl")]
        [InlineData( "John", "von Neuman","john.vonneuman@zhp.net.pl")]
        [InlineData( "Grażyna", "Ąężćńłśó", "grazyna.aezcnlso@zhp.net.pl")]
        [InlineData("Danuta", "Hübner", "danuta.hubner@zhp.net.pl")]
        [InlineData("Renée", "O’Connor", "renee.oconnor@zhp.net.pl")]
        public void GetPossibleAddressesForUser_ValidAddresses(string firstName, string lastName, string expectedMail)
        {
            var adresses = subject.GetPossibleAddressesForUser(firstName, lastName).ToList();

            adresses.Should().StartWith(new MailAddress(expectedMail));
        }
    }
}
