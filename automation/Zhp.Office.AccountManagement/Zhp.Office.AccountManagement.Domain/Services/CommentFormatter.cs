using System.Collections.Concurrent;
using System.IO;
using System.Net.Mail;
using System.Reflection;
using System.Text;

namespace Zhp.Office.AccountManagement.Domain.Services
{
    public interface ICommentFormatter
    {
        string GetMailCreatedComment(MailAddress mail, string password);
    }

    public class CommentFormatter : ICommentFormatter
    {
        private static readonly ConcurrentDictionary<string, string> templateCache = new ConcurrentDictionary<string, string>();

        private static string GetTemplate(string name)
            => templateCache.GetOrAdd(name, n =>
            {
                var assembly = Assembly.GetExecutingAssembly();
                using var resourceStream = assembly.GetManifestResourceStream($"Zhp.Office.AccountManagement.Domain.CommentTemplates.{n}.txt");
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

        public string GetMailCreatedComment(MailAddress mail, string password)
            => GetComment(
                "AccountCreated",
                ("login", mail.ToString()),
                ("password", password));
    }
}
