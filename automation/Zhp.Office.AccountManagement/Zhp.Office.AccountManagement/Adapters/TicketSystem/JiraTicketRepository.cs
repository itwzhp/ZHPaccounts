using Atlassian.Jira;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
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
        private static readonly ConcurrentDictionary<string, Issue> cache = new ConcurrentDictionary<string, Issue>();
        private readonly Jira jiraClient;
        private readonly ILogger<JiraTicketRepository> log;
        private readonly IJiraRequestMapper mapper;
        private readonly bool enableChanges;
        private readonly JiraConfig jiraConfig;

        private bool wereSomeRequestsAlreadyReturned = false;

        public JiraTicketRepository(Jira jiraClient, FunctionConfig config, ILogger<JiraTicketRepository> log, IJiraRequestMapper mapper)
        {
            this.jiraClient = jiraClient;
            this.log = log;
            this.mapper = mapper;
            jiraConfig = config.Jira;
            enableChanges = config.EnableChanges;
        }

        public async Task<IReadOnlyCollection<ActivationRequest>> GetApprovedActivationRequests(CancellationToken token)
        {
            // if we are in debug mode, don't return the same data on subsequent calls
            if (!enableChanges && wereSomeRequestsAlreadyReturned)
                return System.Array.Empty<ActivationRequest>();

            var results = (await jiraClient.Issues.GetIssuesFromJqlAsync(new IssueSearchOptions(jiraConfig.Queries.ApprovedActivationsTicket)
            {
                MaxIssuesPerRequest = jiraConfig.JiraQueryBatchSize
            }, token)).ToList();

            results.ForEach(r => cache.TryAdd(r.Key.Value, r));

            wereSomeRequestsAlreadyReturned = true;
            return results.Select(mapper.Map).OfType<ActivationRequest>().ToList();
        }
        private async ValueTask<Issue> GetIssue(string id, CancellationToken token)
            => cache.TryGetValue(id, out var item)
                ? item
                : await jiraClient.Issues.GetIssueAsync(id, token);

        public async Task MarkAsDone(string id, string? comment, CancellationToken token)
            => await RunWorkflow(await GetIssue(id, token), jiraConfig.Workflows.MarkAsDone.ToString(), comment, token);

        public async Task MarkForManualReview(string id, string? comment, CancellationToken token)
        {
            var issue = await GetIssue(id, token);
            log.LogInformation($"Marking issue {id} to manual review: {comment}");

            // adding comments to Jira doesn't work
            // if(comment != null && enableChanges)
            //    await issue.AddCommentAsync($"Niepowodzenie: {comment}", token);
            await RunWorkflow(issue, jiraConfig.Workflows.MarkForManualReview.ToString(), comment, token);
        }

        private async Task RunWorkflow(Issue issue, string workflowId, string? comment, CancellationToken token)
        {
            if (!enableChanges)
            {
                log.LogInformation($"Jira sandbox mode: running workflow {workflowId} on issue {issue.JiraIdentifier}");
                return;
            }

            await issue.WorkflowTransitionAsync(workflowId, new WorkflowTransitionUpdates
            {
                Comment = comment
            }, token);

            cache.Remove(issue.JiraIdentifier, out _);
        }
    }
}
