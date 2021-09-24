using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Runtime.CompilerServices;
using System.Threading;
using Zhp.Office.AccountManagement.Domain.Model;
using Zhp.Office.AccountManagement.Domain.Ports;
using Zhp.Office.AccountManagement.Infrastructure;

namespace Zhp.Office.AccountManagement.Adapters.ActiveDirectory
{
    internal class AzureDirectoryLastUsageRepository : ILastUsageRepository
    {
        private readonly GraphServiceClient client;
        private readonly ActiveDirectoryConfig config;

        public AzureDirectoryLastUsageRepository(GraphServiceClient client, FunctionConfig config)
        {
            this.client = client;
            this.config = config.ActiveDirectory;
        }

        public async IAsyncEnumerable<LastUsageRecord> FindLastActivity([EnumeratorCancellation] CancellationToken token)
        {
            const string reportLenght = "D180"; // this is actually unused - report is identical whatever you set there
            var response = await client.Reports.GetOffice365ActiveUserDetail(reportLenght).Request().GetResponseAsync(token);
            
            var csv = await response.Content.ReadAsStreamAsync(token);

            var records = CsvReportParser.ReadEntries(csv, token);
            var recordsWithLicenses = records
                .Select(r => (record: r, removableLicenses: FindLicensesToRemove(r)))
                .Where(r => r.removableLicenses.Any());

            await foreach(var (record, licenses) in recordsWithLicenses)
            {
                if(MailAddress.TryCreate(record.UserPrincipalName, out var mail))
                    yield return new(mail, record.LastActivity, record.LastLicenseAssign, licenses);
            }
        }

        private Guid[] FindLicensesToRemove(CsvReportParser.ReportEntry r) =>
            r.AssignedProducts.Split('+')
                .Select(config.RemovableLicenses.GetValueOrDefault)
                .Where(g => g != Guid.Empty)
                .ToArray();
    }
}
