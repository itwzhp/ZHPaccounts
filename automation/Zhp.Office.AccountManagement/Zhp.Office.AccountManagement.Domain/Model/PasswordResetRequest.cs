using System.Net.Mail;

namespace Zhp.Office.AccountManagement.Model
{
    public class PasswordResetRequest
    {
        public string Id { get; set; } = string.Empty;

        public MailAddress MailAddress { get; set; } = new MailAddress("example@example.com");
    }
}
