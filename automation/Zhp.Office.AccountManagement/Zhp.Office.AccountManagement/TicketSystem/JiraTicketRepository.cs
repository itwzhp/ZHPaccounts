using Atlassian.Jira;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Zhp.Office.AccountManagement.Infrastructure;

namespace Zhp.Office.AccountManagement.TicketSystem
{
    internal interface ITicketRepository
    {
        Task<IReadOnlyCollection<Issue>> GetActivationIssues(CancellationToken token);
    }

    internal class JiraTicketRepository : ITicketRepository
    {
        private readonly Jira jiraClient;
        private readonly FunctionConfig config;

        public JiraTicketRepository(Jira jiraClient, FunctionConfig config)
        {
            this.jiraClient = jiraClient;
            this.config = config;
        }

        public async Task<IReadOnlyCollection<Issue>> GetActivationIssues(CancellationToken token)
        {
            var firstQuery = await jiraClient.Issues.GetIssuesFromJqlAsync(new IssueSearchOptions(config.Jira.ActivationTicketsQuery)
            {
                MaxIssuesPerRequest = config.Jira.JiraQueryBatchSize
            }, token);

            var result = firstQuery.ToList();
            // todo query for more
            return result;
        }
    }
}
