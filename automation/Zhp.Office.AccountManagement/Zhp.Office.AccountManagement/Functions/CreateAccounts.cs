using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Zhp.Office.AccountManagement.Domain.Services;

namespace Zhp.Office.AccountManagement.Functions
{
    public class CreateAccounts
    {
        private readonly AccountsCreatingService service;

        public CreateAccounts(AccountsCreatingService service)
        {
            this.service = service;
        }

        [FunctionName("CreateAccounts")]
#warning Do not publish with RunOnStartup
        public async Task Run([TimerTrigger("0 17 3 * * *", RunOnStartup = true)]TimerInfo myTimer, CancellationToken token)
        {
            await service.CreateAccounts(token);
        }
    }
}
