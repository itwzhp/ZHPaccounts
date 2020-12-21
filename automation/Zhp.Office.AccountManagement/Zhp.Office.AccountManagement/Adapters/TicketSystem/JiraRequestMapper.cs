﻿using Atlassian.Jira;
using System.Linq;
using Zhp.Office.AccountManagement.Model;

namespace Zhp.Office.AccountManagement.Adapters.TicketSystem
{
    public interface IJiraRequestMapper
    {
        ActivationRequest? Map(Issue jiraIssue);
    }

    public class JiraRequestMapper : IJiraRequestMapper
    {
        public ActivationRequest? Map(Issue issue)
        {
            var customFields = issue.CustomFields.ToDictionary(f => f.Name, f => f.Values?.FirstOrDefault());
            bool isAnyFieldMissing = false;

            string FindValue(string label)
            {
                if (customFields.TryGetValue(label, out var value) && value != null)
                    return value;

                isAnyFieldMissing = true;
                return string.Empty;
            }

            var request = new ActivationRequest
            {
                Id = issue.Key.Value,
                FirstName = FindValue("Name"),
                LastName = FindValue("Surname"),
                MembershipNumber = FindValue("Member ID (reporter)"),
                FirstLevelUnit = FindValue("Hufiec"),
                SecondLevelUnit = FindValue("Chorągiew"),
            };

            // todo this should be set on input form
            if (!string.IsNullOrEmpty(request.FirstLevelUnit) &&
                !request.FirstLevelUnit.Contains("Chorągiew") &&
                !request.FirstLevelUnit.Contains("Hufiec"))
                request.FirstLevelUnit = $"Hufiec {request.FirstLevelUnit}";

            if (!request.SecondLevelUnit.Contains("Główna Kwatera") &&
                !request.SecondLevelUnit.Contains("Chorągiew"))
                request.SecondLevelUnit = $"Chorągiew {request.SecondLevelUnit}";

            return isAnyFieldMissing ? null : request;
        }
    }
}