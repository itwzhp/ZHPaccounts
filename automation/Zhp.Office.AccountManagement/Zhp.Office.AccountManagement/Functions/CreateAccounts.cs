using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
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

        [Function("CreateAccounts")]
        //TODO [FixedDelayRetry(3, "00:01:10")]
        public async Task Run([TimerTrigger("0 17 3 * * *", RunOnStartup = false)] TimerInfo myTimer, CancellationToken token)
        {
            try
            {
                log.LogInformation("Running function CreateAccounts...");
                await service.CreateAccounts(token);
                log.LogInformation("Function CreateAccounts finished.");
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Cought error during CreateAccounts");
            }
        }
    }
}
