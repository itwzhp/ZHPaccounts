using System.Threading;
using System.Threading.Tasks;
using Atlassian.Jira;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace Zhp.Office.AccountManagement
{
    public class CreateAccounts
    {
        public CreateAccounts(Jira jira)
        {

        }

        [FunctionName("CreateAccounts")]
        public async Task Run([TimerTrigger("0 17 3 * * *",RunOnStartup = true)]TimerInfo myTimer, ILogger log, CancellationToken token)
        {
            //var result = await client.Issues.GetIssuesFromJqlAsync(new IssueSearchOptions("")
            //{
                
            //}, token);

            //Console.WriteLine(result.Count());
        }
    }
}
