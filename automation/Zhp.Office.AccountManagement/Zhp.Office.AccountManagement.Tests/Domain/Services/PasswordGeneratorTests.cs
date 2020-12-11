﻿using FluentAssertions;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using Xunit;
using Zhp.Office.AccountManagement.Domain.Services;

namespace Zhp.Office.AccountManagement.Tests.Domain.Services
{
    /// <summary>
    /// Requirements based on https://docs.microsoft.com/en-us/azure/active-directory/authentication/concept-sspr-policy#password-policies-that-only-apply-to-cloud-user-accounts
    /// </summary>
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

        [Theory, Repeat(50)]
        public void PasswordShouldHaveValidLength(int runNumber)
            => subject.GeneratePassword().Length.Should().BeGreaterOrEqualTo(8).And.BeLessOrEqualTo(256);

        [Theory, Repeat(200)]
        public void PasswordShouldHave3Of4CharGroups(int runNumber)
        {
            var basicGroups = new[] { UnicodeCategory.LowercaseLetter, UnicodeCategory.UppercaseLetter, UnicodeCategory.DecimalDigitNumber };
            var charGroups = subject.GeneratePassword().Select(c => char.GetUnicodeCategory(c)).Distinct().ToList();

            var foundGroups = basicGroups.Count(g => charGroups.Contains(g))
                            + (charGroups.Any(g => !basicGroups.Contains(g) && g != char.GetUnicodeCategory(' ')) ? 1 : 0);

            foundGroups.Should().BeGreaterOrEqualTo(3);
        }

        [Theory, Repeat(200)]
        public void PasswordsShouldContainOnlyAllowedChars(int runNumber)
            => subject.GeneratePassword().Should().MatchRegex(@"^[a-zA-Z0-9 @#$%^&*\-_!+=\[\]{}|\\:',\.?/`~""();]*$");

        [Theory, Repeat(50)]
        public void PasswordsShouldNotContainUnicode(int runNumber)
            => subject.GeneratePassword().AsEnumerable().Should().NotContain(c => c > 255);

        [Fact]
        public void PasswordsShouldDiffer()
            => Enumerable.Range(1, 200)
            .Select(_ => subject.GeneratePassword())
            .Should().OnlyHaveUniqueItems();
    }
}