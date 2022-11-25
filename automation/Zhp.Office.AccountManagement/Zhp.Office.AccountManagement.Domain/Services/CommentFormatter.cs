using System.Collections.Concurrent;
using System.IO;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using Zhp.Office.AccountManagement.Model;

namespace Zhp.Office.AccountManagement.Domain.Services
{
    public interface ICommentFormatter
    {
        string GetMailCreatedComment(MailAddress mail, string password, ActivationRequest request);
    }

    public class CommentFormatter : ICommentFormatter
    {
        private static readonly ConcurrentDictionary<string, string> templateCache = new ConcurrentDictionary<string, string>();

        private static string GetTemplate(string name)
            => templateCache.GetOrAdd(name, n => {
                var assembly = Assembly.GetExecutingAssembly();
                var fileName = $"Zhp.Office.AccountManagement.Domain.CommentTemplates.{n}.txt";
                using var resourceStream = assembly.GetManifestResourceStream(fileName);
                if (resourceStream == null) throw new FileNotFoundException("Comment template file not found", fileName);
                using var reader = new StreamReader(resourceStream, Encoding.UTF8);
                return reader.ReadToEnd();
            });

        private static string GetComment(string name, params (string key, string value)[] parameters)
        {
            var mesage = new StringBuilder(GetTemplate(name));

            foreach (var (key, value) in parameters)
            {
                mesage.Replace($"{{{key}}}", value);
            }

            return mesage.ToString();
        }

        public string GetMailCreatedComment(MailAddress mail, string password, ActivationRequest request)
            => GetComment(
                "AccountCreated",
                ("login", mail.ToString()),
                ("password", password),
                ("firstLevelUnit", request.FirstLevelUnit),
                ("secondLevelUnit", request.SecondLevelUnit),
                ("membershipNumber", request.MembershipNumber));
    }
}
