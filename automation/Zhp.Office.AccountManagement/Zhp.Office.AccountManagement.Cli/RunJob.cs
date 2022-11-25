using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Zhp.Office.AccountManagement.Domain.Services;
using Zhp.Office.AccountManagement.Functions;

namespace Zhp.Office.AccountManagement.Cli
{
    internal class RunJob : BackgroundService
    {
        private readonly OldAccountCleaner cleaner;
        private readonly IHost host;
        private readonly ILogger<RunJob> logger;
        private readonly CreateAccounts func;

        public RunJob(OldAccountCleaner cleaner, IHost host, ILogger<RunJob> logger, CreateAccounts func)
        {
            this.cleaner = cleaner;
            this.host = host;
            this.logger = logger;
            this.func = func;
        }

        async override protected Task ExecuteAsync(CancellationToken stoppingToken)
        {
            //logger.LogInformation("Starting...");
            //var cleanedList = await cleaner.CleanOldAccounts(400, stoppingToken);

            //File.WriteAllLines(@"C:\Users\karol\source\TMP\usrs\out.csv", cleanedList.Select(r => $"{r.Username},{r.LicenseAssignDate:d},{r.LastUsage:d}").Prepend("UserName,LicenseAssignDate,LastUsage"));

            //logger.LogInformation("Finished. Shutting down...");
            //await host.StopAsync(CancellationToken.None);

            await this.func.Run(null!, CancellationToken.None);
        }
    }
}
