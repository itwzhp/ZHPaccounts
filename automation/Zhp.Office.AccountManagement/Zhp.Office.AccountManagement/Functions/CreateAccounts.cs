using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Zhp.Office.AccountManagement.Domain.Ports;
using Zhp.Office.AccountManagement.Infrastructure;

namespace Zhp.Office.AccountManagement.Functions
{
    public class CreateAccounts
    {
        private readonly ITicketRepository ticketRepo;
        private readonly ILogger<CreateAccounts> logger;

        public CreateAccounts(ITicketRepository ticketRepo, ILogger<CreateAccounts> logger)
        {
            this.ticketRepo = ticketRepo;
            this.logger = logger;
        }

        [FunctionName("CreateAccounts")]
        public async Task Run([TimerTrigger("0 17 3 * * *", RunOnStartup = FunctionConfig.IsDebugBuild)]TimerInfo myTimer, CancellationToken token)
        {
            await this.ticketRepo.Test();

            // todo get all approved tickets
            // todo create accounts for them
            // todo mark as done
        }
    }
}
