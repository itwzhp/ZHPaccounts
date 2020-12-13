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
        private bool enableChanges;
        private readonly FunctionConfig.JiraConfig jiraConfig;

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

            return new ActivationRequest[0];
        }

        public async Task MarkAsDone(string id)
        {
            if(!enableChanges)
            {
                log.LogInformation($"Jira sandbox mode: marking as done: {id}");
                return;
            }
        }


        public async Task Test()
        {
            // todo it works from Open state, but not from Approved :(
            var issue = await jiraClient.Issues.GetIssueAsync("MS365-4671");
            issue.Resolution = "Done";
            await issue.WorkflowTransitionAsync("11"

                , new WorkflowTransitionUpdates
                {
                    Comment = @"Gratulujemy konta!
Lista
- a
- b
- c

ListaHtml
<ul>
<li>a</li>
<li>1</li>
<li>i</li>
</ul>
"
                }
                );

        }
    }
}
