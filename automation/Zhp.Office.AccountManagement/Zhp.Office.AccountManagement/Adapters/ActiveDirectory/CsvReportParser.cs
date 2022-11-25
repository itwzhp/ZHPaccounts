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

        public record ReportEntry(
            string UserPrincipalName,
            string AssignedProducts,
            DateTime? ExchangeLastActivityDate,
            DateTime? OneDriveLastActivityDate,
            DateTime? SharePointLastActivityDate,
            DateTime? SkypeLastActivityDate,
            DateTime? YammerLastActivityDate,
            DateTime? TeamsLastActivityDate,

            DateTime? ExchangeLicenseAssignDate,
            DateTime? OneDriveLicenseAssignDate,
            DateTime? SharePointLicenseAssignDate,
            DateTime? SkypeLicenseAssignDate,
            DateTime? YammerLicenseAssignDate,
            DateTime? TeamsLicenseAssignDate)
        {
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

            public DateTime? LastLicenseAssign
                => new[]
                {
                    ExchangeLicenseAssignDate,
                    OneDriveLicenseAssignDate,
                    SharePointLicenseAssignDate,
                    SkypeLicenseAssignDate,
                    YammerLicenseAssignDate,
                    TeamsLicenseAssignDate
                }
                .Aggregate((d1, d2) => d1 > d2 || !d2.HasValue ? d1 : d2);
        }

        private class ReportEntryMap : ClassMap<ReportEntry>
        {
            public ReportEntryMap()
            {
                Parameter(nameof(ReportEntry.UserPrincipalName)).Name("User Principal Name");
                Parameter(nameof(ReportEntry.AssignedProducts)).Name("Assigned Products");

                Parameter(nameof(ReportEntry.ExchangeLastActivityDate)).Name("Exchange Last Activity Date");
                Parameter(nameof(ReportEntry.OneDriveLastActivityDate)).Name("OneDrive Last Activity Date");
                Parameter(nameof(ReportEntry.SharePointLastActivityDate)).Name("SharePoint Last Activity Date");
                Parameter(nameof(ReportEntry.SkypeLastActivityDate)).Name("Skype For Business Last Activity Date");
                Parameter(nameof(ReportEntry.YammerLastActivityDate)).Name("Yammer Last Activity Date");
                Parameter(nameof(ReportEntry.TeamsLastActivityDate)).Name("Teams Last Activity Date");

                Parameter(nameof(ReportEntry.ExchangeLicenseAssignDate)).Name("Exchange License Assign Date");
                Parameter(nameof(ReportEntry.OneDriveLicenseAssignDate)).Name("OneDrive License Assign Date");
                Parameter(nameof(ReportEntry.SharePointLicenseAssignDate)).Name("SharePoint License Assign Date");
                Parameter(nameof(ReportEntry.SkypeLicenseAssignDate)).Name("Skype For Business License Assign Date");
                Parameter(nameof(ReportEntry.YammerLicenseAssignDate)).Name("Yammer License Assign Date");
                Parameter(nameof(ReportEntry.TeamsLicenseAssignDate)).Name("Teams License Assign Date");

                Map(r => r.LastActivity).Ignore();
            }
        }
    }
}
