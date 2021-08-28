using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Zhp.Office.AccountManagement.Adapters.ActiveDirectory
{
    public static class CsvReportParser
    {
        public static async IAsyncEnumerable<ReportEntry> ReadEntries(Stream inputStream, [EnumeratorCancellation] CancellationToken token)
        {
            using var textReader = new StreamReader(inputStream);
            using var reader = new CsvReader(textReader, CultureInfo.InvariantCulture);
            reader.Context.RegisterClassMap<ReportEntryMap>();
            await foreach (var record in reader.GetRecordsAsync<ReportEntry>(token))
            {
                yield return record;
            }
        }

        public class ReportEntry
        {
            public string UserPrincipalName { get; set; } = string.Empty;

            public string AssignedProducts { get; set; } = string.Empty;

            public DateTime? ExchangeLastActivityDate { get; set; }

            public DateTime? OneDriveLastActivityDate { get; set; }

            public DateTime? SharePointLastActivityDate { get; set; }

            public DateTime? SkypeLastActivityDate { get; set; }

            public DateTime? YammerLastActivityDate { get; set; }

            public DateTime? TeamsLastActivityDate { get; set; }

            public DateTime? LastActivity
                => new[]
                {
                    ExchangeLastActivityDate,
                    OneDriveLastActivityDate,
                    SharePointLastActivityDate,
                    SkypeLastActivityDate,
                    YammerLastActivityDate,
                    TeamsLastActivityDate
                }
                .Aggregate((d1, d2) => d1 > d2 || !d2.HasValue ? d1 : d2);
        }

        private class ReportEntryMap : ClassMap<ReportEntry>
        {
            public ReportEntryMap()
            {
                Map(r => r.UserPrincipalName).Name("User Principal Name");
                Map(r => r.AssignedProducts).Name("Assigned Products");

                Map(r => r.ExchangeLastActivityDate).Name("Exchange Last Activity Date");
                Map(r => r.OneDriveLastActivityDate).Name("OneDrive Last Activity Date");
                Map(r => r.SharePointLastActivityDate).Name("SharePoint Last Activity Date");
                Map(r => r.SkypeLastActivityDate).Name("Skype For Business Last Activity Date");
                Map(r => r.YammerLastActivityDate).Name("Yammer Last Activity Date");
                Map(r => r.TeamsLastActivityDate).Name("Teams Last Activity Date");

                Map(r => r.LastActivity).Ignore();
            }
        }
    }
}
