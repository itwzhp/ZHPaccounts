using Atlassian.Jira;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using Zhp.Office.AccountManagement.Model;

namespace Zhp.Office.AccountManagement.Adapters.TicketSystem
{
    public interface IJiraRequestMapper
    {
        ActivationRequest? MapActivation(Issue jiraIssue);

        PasswordResetRequest? MapPasswordReset(Issue jiraIssue);
    }

    public class JiraRequestMapper : IJiraRequestMapper
    {
        private readonly ILogger<JiraRequestMapper> log;

        public JiraRequestMapper(ILogger<JiraRequestMapper> log)
        {
            this.log = log;
        }

        private string FindRequiredValue(IReadOnlyDictionary<string, string?> customFields, string label, List<string> missingRequiredFields)
        {
            if (customFields.TryGetValue(label, out var value) && value != null)
                return value;

            missingRequiredFields.Add(label);

            return string.Empty;
        }

        private string? FindOptionalValue(IReadOnlyDictionary<string, string?> customFields, string label)
        {
            if (customFields.TryGetValue(label, out var value) && value != null)
                return value;

            return null;
        }

        public ActivationRequest? MapActivation(Issue issue)
        {
            var customFields = issue.CustomFields.ToDictionary(f => f.Name, f => f.Values?.FirstOrDefault());
            var missingRequiredFields = new List<string>();

            var request = new ActivationRequest
            {
                Id = issue.Key.Value,
                FirstName = FindRequiredValue(customFields, "Name", missingRequiredFields),
                LastName = FindRequiredValue(customFields, "Surname", missingRequiredFields),
                MembershipNumber = FindRequiredValue(customFields, "Member ID (reporter)", missingRequiredFields),
                FirstLevelUnit = FindOptionalValue(customFields, "Hufiec"),
                SecondLevelUnit = FindRequiredValue(customFields, "Chorągiew", missingRequiredFields),
            };

            // todo this should be set on input form
            if (!string.IsNullOrEmpty(request.FirstLevelUnit) &&
                !request.FirstLevelUnit.Contains("Chorągiew") &&
                !request.FirstLevelUnit.Contains("Hufiec"))
                request.FirstLevelUnit = $"Hufiec {request.FirstLevelUnit}";

            if (!request.SecondLevelUnit.Contains("Główna Kwatera") &&
                !request.SecondLevelUnit.Contains("Chorągiew"))
                request.SecondLevelUnit = $"Chorągiew {request.SecondLevelUnit}";

            if (missingRequiredFields.Any())
                log.LogWarning($"Ignoring ticket {issue.Key.Value}, missing fields: {string.Join(", ", missingRequiredFields)}");

            return missingRequiredFields.Any() ? null : request;
        }

        public PasswordResetRequest? MapPasswordReset(Issue issue)
        {
            var customFields = issue.CustomFields.ToDictionary(f => f.Name, f => f.Values?.FirstOrDefault());
            var mailString = FindOptionalValue(customFields, "todo");

            try
            {
                var mail = new MailAddress(mailString); // todo use MailAddress.TryCreate on .NET 5
                return new PasswordResetRequest
                {
                    Id = issue.Key.Value,
                    MailAddress = mail
                };
            }
            catch
            {
                return null;
            }
        }
    }
}
