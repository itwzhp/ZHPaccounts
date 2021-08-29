using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Zhp.Office.AccountManagement.Domain.Ports;

namespace Zhp.Office.AccountManagement.Domain.Services
{
    public class OldAccountCleaner
    {
        private readonly ILastUsageRepository lastUsageRepo;
        private readonly IAccountManager manager;
        private readonly ILogger<OldAccountCleaner> logger;

        public OldAccountCleaner(ILastUsageRepository oldUserFinder, IAccountManager manager, ILogger<OldAccountCleaner> logger)
        {
            this.lastUsageRepo = oldUserFinder;
            this.manager = manager;
            this.logger = logger;
        }

        public async Task CleanOldAccounts(uint thresholdInDays, CancellationToken token)
        {
            var lastUsageThreshold = DateTime.UtcNow.Date.AddDays(-thresholdInDays);
            var freshAccoundThreshold = DateTime.UtcNow.Date.AddDays(-60); 

            var oldAccounts = await lastUsageRepo.FindLastActivity(token)
                .Where(r => r.LastUsage == null || r.LastUsage < lastUsageThreshold)
                .Where(r => r.LicenseAssignDate != null && r.LicenseAssignDate < freshAccoundThreshold)
                .ToListAsync(token);

            logger.LogInformation($"Staring cleaning {oldAccounts.Count} accounts");

            uint errors = 0;
            foreach(var account in oldAccounts)
            {
                try
                {
                    await manager.TakeAwayLicense(account.Username, token);
                }
                catch(Exception ex)
                {
                    errors++;
                    logger.LogWarning(ex, $"Unable to remove licenses from user {account}: {ex}");
                }
            }

            logger.LogInformation($"Staring cleaning {oldAccounts.Count - errors} accounts, {errors} errors");
        }
    }
}
