using FluentAssertions;
using System.Net.Mail;
using Xunit;
using Zhp.Office.AccountManagement.Domain.Services;
using Zhp.Office.AccountManagement.Model;

namespace Zhp.Office.AccountManagement.Tests.Domain.Services
{
    public class CommentFormatterTests
    {
        private readonly ICommentFormatter subject = new CommentFormatter();

        [Fact]
        public void GetMailCreatedComment_CommentContainsMailAndPassword()
        {
            var request = new ActivationRequest
            {
                FirstLevelUnit = "Hufiec Puck",
                SecondLevelUnit = "Chorągiew Gdańska",
                MembershipNumber = "12345678",
            };

            subject.GetMailCreatedComment(new MailAddress("test@example.com"), "somePassword-ąłść", request)
                .Should().Contain("test@example.com")
                .And.Contain("somePassword-ąłść")
                .And.Contain("Hufiec Puck")
                .And.Contain("Chorągiew Gdańska")
                .And.Contain("12345678");
        }

        [Fact]
        public void GetPasswordResetComment_CommentContainsPassword()
        {
            subject.GetPasswordResetComment(new MailAddress("test@example.com"), "SomePassword123")
                .Should().Contain("SomePassword123");
        }
    }
}
