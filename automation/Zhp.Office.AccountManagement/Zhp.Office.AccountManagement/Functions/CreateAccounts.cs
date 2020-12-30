using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using Zhp.Office.AccountManagement.Domain.Services;
using Zhp.Office.AccountManagement.Infrastructure;

namespace Zhp.Office.AccountManagement.Functions
{
    public class CreateAccounts
    {
        private readonly AccountsCreatingService service;
        private readonly ILogger<CreateAccounts> log;
        private readonly FunctionConfig config;

        public CreateAccounts(AccountsCreatingService service, ILogger<CreateAccounts> log, FunctionConfig config)
        {
            this.service = service;
            this.log = log;
            this.config = config;
        }

        [FunctionName("CreateAccounts")]
        public async Task Run([TimerTrigger("0 17 3 * * *", RunOnStartup = false)] TimerInfo myTimer, CancellationToken token)
        {
            //TODO TMP DO NOT MERGE
            this.log.LogInformation($"JIRA Pass: {config.Jira.Password}");
            log.LogInformation("Running function CreateAccounts...");
            await service.CreateAccounts(token);
            log.LogInformation("Function CreateAccounts finished.");
        }
    }
}
