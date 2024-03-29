using FluentAssertions;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using Xunit;
using Zhp.Office.AccountManagement.Domain.Services;

namespace Zhp.Office.AccountManagement.Tests.Domain.Services
{
    [SuppressMessage("Usage", "xUnit1026:Theory methods should use all of their parameters", Justification = "Required for Repeat attribute")]
    public class PasswordGeneratorTests : IDisposable
    {
        private readonly RandomNumberGenerator generator = RandomNumberGenerator.Create();
        private readonly IPasswordGenerator subject;

        public PasswordGeneratorTests()
        {
            subject = new PasswordGenerator(generator);
        }

        public void Dispose()
            => generator.Dispose();

        // Requirements based on https://docs.microsoft.com/en-us/azure/active-directory/authentication/concept-sspr-policy#password-policies-that-only-apply-to-cloud-user-accounts
        #region Microsoft Azure AD Password policy requirements

        [Theory, Repeat(50)]
        public void AzureADPolicy_PasswordShouldHaveValidLength(int runNumber)
            => subject.GeneratePassword().Length.Should().BeGreaterOrEqualTo(8).And.BeLessOrEqualTo(256);

        [Theory, Repeat(200)]
        public void AzureADPolicy_PasswordShouldHave3Of4CharGroups(int runNumber)
        {
            var basicGroups = new[] { UnicodeCategory.LowercaseLetter, UnicodeCategory.UppercaseLetter, UnicodeCategory.DecimalDigitNumber };

            var password = subject.GeneratePassword();

            var charGroups = password.Select(c => char.GetUnicodeCategory(c)).Distinct().ToList();

            var foundGroups = basicGroups.Count(g => charGroups.Contains(g))
                            + (charGroups.Any(g => !basicGroups.Contains(g) && g != char.GetUnicodeCategory(' ')) ? 1 : 0);

            foundGroups.Should().BeGreaterOrEqualTo(3, $"Password {password} is too simple");
        }

        [Theory, Repeat(200)]
        public void AzureADPolicy_PasswordsShouldContainOnlyAllowedChars(int runNumber)
            => subject.GeneratePassword().Should().MatchRegex(@"^[a-zA-Z0-9 @#$%^&*\-_!+=\[\]{}|\\:',\.?/`~""();]*$");

        [Theory, Repeat(50)]
        public void AzureADPolicy_PasswordsShouldNotContainUnicode(int runNumber)
            => subject.GeneratePassword().AsEnumerable().Should().NotContain(c => c > 255);

        #endregion

        #region Current implementation tests

        [Theory, Repeat(50)]
        public void PasswordShouldHaveValidLength(int runNumber)
            => subject.GeneratePassword().Should().HaveLength(8);

        [Theory, Repeat(200)]
        public void PasswordShouldContainDigitLowerAndUpperLetter(int runNumber)
        {
            string password = subject.GeneratePassword();

            bool isPasswordStrong = password.Any(char.IsDigit) && password.Any(char.IsUpper) && password.Any(char.IsLower);

            isPasswordStrong.Should().BeTrue($"Password {password} is too simple");
        }

        [Theory, Repeat(200)]
        public void PasswordsShouldContainOnlyAllowedChars(int runNumber)
            => subject.GeneratePassword().Should().MatchRegex(@"^[a-zA-Z0-9]*$");

        [Fact]
        public void PasswordsShouldDiffer()
            => Enumerable.Range(1, 200)
            .Select(_ => subject.GeneratePassword())
            .Should().OnlyHaveUniqueItems();

        #endregion
    }
}
