using FluentAssertions;
using System.Net.Mail;
using Xunit;
using Zhp.Office.AccountManagement.Domain.Services;

namespace Zhp.Office.AccountManagement.Tests.Domain.Services
{
    public class CommentFormatterTests
    {
        private readonly ICommentFormatter subject = new CommentFormatter();

        [Fact]
        public void GetMailCreatedComment_CommentContainsMailAndPassword()
        {
            subject.GetMailCreatedComment(new MailAddress("test@example.com"), "somePassword-ąłść")
                .Should().Contain("test@example.com")
                .And.Contain("somePassword-ąłść");
        }
    }
}
