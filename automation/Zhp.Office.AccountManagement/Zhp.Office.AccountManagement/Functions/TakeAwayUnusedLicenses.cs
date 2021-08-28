using Microsoft.Azure.Functions.Worker;
using System.Threading;
using System.Threading.Tasks;
using Zhp.Office.AccountManagement.Domain.Services;
using Zhp.Office.AccountManagement.Infrastructure;

namespace Zhp.Office.AccountManagement.Functions
{
    public class TakeAwayUnusedLicenses
    {
        private readonly OldAccountCleaner cleaner;
        private readonly uint daysThreshold;

        public TakeAwayUnusedLicenses(OldAccountCleaner cleaner, FunctionConfig config)
        {
            this.cleaner = cleaner;
            this.daysThreshold = config.OldAccountThresholdInDays;
        }

        [Function("TakeAwayUnusedLicenses")]
        public async Task Run([TimerTrigger("0 37 1 1 * *", RunOnStartup = false)] TimerInfo myTimer, CancellationToken token)
            => await cleaner.CleanOldAccounts(daysThreshold, token);
    }
}
