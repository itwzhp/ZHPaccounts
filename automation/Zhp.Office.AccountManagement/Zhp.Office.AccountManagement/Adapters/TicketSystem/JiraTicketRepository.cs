using Atlassian.Jira;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Zhp.Office.AccountManagement.Domain.Ports;
using Zhp.Office.AccountManagement.Infrastructure;
using Zhp.Office.AccountManagement.Model;

namespace Zhp.Office.AccountManagement.Adapters.TicketSystem
{
    internal class JiraTicketRepository : ITicketRepository
    {
        private readonly Jira jiraClient;
        private readonly ILogger<JiraTicketRepository> log;
        private readonly bool enableChanges;
        private readonly JiraConfig jiraConfig;

        public JiraTicketRepository(Jira jiraClient, FunctionConfig config, ILogger<JiraTicketRepository> log)
        {
            this.jiraClient = jiraClient;
            this.log = log;
            this.jiraConfig = config.Jira;
            this.enableChanges = config.EnableChanges;
        }

        public async Task<IReadOnlyCollection<ActivationRequest>> GetApprovedActivationRequests(CancellationToken token)
        {
            var firstQuery = await jiraClient.Issues.GetIssuesFromJqlAsync(new IssueSearchOptions(jiraConfig.Queries.ApprovedActivationsTicket)
            {
                MaxIssuesPerRequest = jiraConfig.JiraQueryBatchSize
            }, token);

            return firstQuery.Select(Map).OfType<ActivationRequest>().ToList();
        }

        public static ActivationRequest? Map(Issue issue)
        {
            Dictionary<string, string?> customFields = issue.CustomFields.ToDictionary(f => f.Name, f => f.Values?.FirstOrDefault());
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
                FirstLevelUnit = FindValue("Hufiec"), //TODO Add "Hufiec"
                SecondLevelUnit = FindValue("Chorągiew"), //TODO Add "Chorągiew"
            };

            return isAnyFieldMissing ? null : request;
        }

        public async Task MarkAsDone(string id, string? comment, CancellationToken token)
            => await RunWorkflow(id, jiraConfig.Workflows.MarkAsDone.ToString(), comment, token);

        public async Task MarkForManualReview(string id, string? comment, CancellationToken token)
            => await RunWorkflow(id, jiraConfig.Workflows.MarkForManualReview.ToString(), comment, token);

        private async Task RunWorkflow(string issueId, string workflowId, string? comment, CancellationToken token)
        {
            // todo cache issues
            var issue = await jiraClient.Issues.GetIssueAsync(issueId, token);
            if (!enableChanges)
            {
                log.LogInformation($"Jira sandbox mode: running workflow {workflowId} on issue {issueId}");
                return;
            }

            await issue.WorkflowTransitionAsync(workflowId, new WorkflowTransitionUpdates
            {
                Comment = comment
            }, token);
        }
    }
}
