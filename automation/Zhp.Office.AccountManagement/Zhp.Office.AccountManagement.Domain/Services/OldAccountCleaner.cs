using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
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

        public async Task<List<Model.LastUsageRecord>> CleanOldAccounts(uint thresholdInDays, CancellationToken token)
        {
            var lastUsageThreshold = DateTime.UtcNow.Date.AddDays(-thresholdInDays);
            var freshAccoundThreshold = DateTime.UtcNow.Date.AddDays(-90); 

            var oldAccounts = await lastUsageRepo.FindLastActivity(token)
                .Where(r => r.LastUsage == null || r.LastUsage < lastUsageThreshold)
                .Where(r => r.LicenseAssignDate != null && r.LicenseAssignDate < freshAccoundThreshold)
                .ToListAsync(token);

            logger.LogInformation($"Staring cleaning {oldAccounts.Count} accounts");

            uint errors = 0;
            int count = 0;

            System.IO.File.WriteAllLines(@"C:\Users\karol\source\TMP\usrs\out.csv", oldAccounts.Select(r => $"{r.Username},{r.LicenseAssignDate:d},{r.LastUsage:d}").Prepend("UserName,LicenseAssignDate,LastUsage"));

            foreach (var account in oldAccounts)
            {
                try
                {
                    logger.LogInformation($"Progress: {++count}/{oldAccounts.Count}");
                    await manager.TakeAwayLicense(account.Username, account.Licenses, token);
                }
                catch(Exception ex)
                {
                    errors++;
                    logger.LogWarning(ex, $"Unable to remove licenses from user {account}: {ex}");
                }
            }

            logger.LogInformation($"Finished cleaning {oldAccounts.Count - errors} accounts, {errors} errors");
            return oldAccounts;
        }
    }
}
