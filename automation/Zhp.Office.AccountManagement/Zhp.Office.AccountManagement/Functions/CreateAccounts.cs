using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Zhp.Office.AccountManagement.Domain.Services;

namespace Zhp.Office.AccountManagement.Functions
{
    public class CreateAccounts
    {
        private readonly AccountsCreatingService service;
        private readonly ILogger<CreateAccounts> log;

        public CreateAccounts(AccountsCreatingService service, ILogger<CreateAccounts> log)
        {
            this.service = service;
            this.log = log;
        }

        [FunctionName("CreateAccounts")]
#warning Do not publish with RunOnStartup
        public async Task Run([TimerTrigger("0 17 3 * * *", RunOnStartup = true)]TimerInfo myTimer, CancellationToken token)
        {
            log.LogInformation("Running function CreateAccounts...");
            await service.CreateAccounts(token);
            log.LogInformation("Function CreateAccounts finished.");
        }
    }
}
