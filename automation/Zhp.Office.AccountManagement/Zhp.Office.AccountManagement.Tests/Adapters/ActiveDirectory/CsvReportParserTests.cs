using FluentAssertions;
using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Zhp.Office.AccountManagement.Adapters.ActiveDirectory;

namespace Zhp.Office.AccountManagement.Tests.Adapters.ActiveDirectory
{
    public class CsvReportParserTests
    {
        [Fact]
        public async Task LoadsData()
        {
            var data = Assembly.GetExecutingAssembly().GetManifestResourceStream("Zhp.Office.AccountManagement.Tests.Adapters.ActiveDirectory.TestReport.csv")
                ?? throw new Exception("Test data not found");

            var result = await CsvReportParser.ReadEntries(data, CancellationToken.None).ToListAsync();

            result.Should().HaveCount(7);
            result[0].ExchangeLastActivityDate.Should().BeNull();
            result[1].ExchangeLastActivityDate.Should().BeSameDateAs(new DateTime(2021, 03, 25));
            result[3].AssignedProducts.Should().Be("POWER BI (FREE)+OFFICE 365 E2");
            result[4].UserPrincipalName.Should().Be("sp9zak@testowisko.zhp.pl");
        }

        [Fact]
        public void ReportEntryLastActivity_CountsProperly()
        {
            var record = new CsvReportParser.ReportEntry(
                UserPrincipalName: "aa@bb.pl",
                AssignedProducts: string.Empty,

                ExchangeLastActivityDate: null,
                OneDriveLastActivityDate: new DateTime(2010, 04, 08),
                SkypeLastActivityDate: new DateTime(2008, 04, 08),
                SharePointLastActivityDate: new DateTime(2013, 04, 08),
                YammerLastActivityDate: null,
                TeamsLastActivityDate: null,

                ExchangeLicenseAssignDate: null,
                OneDriveLicenseAssignDate: null,
                SkypeLicenseAssignDate: null,
                SharePointLicenseAssignDate: null,
                YammerLicenseAssignDate: null,
                TeamsLicenseAssignDate: null);

            record.LastActivity.Should().Be(record.SharePointLastActivityDate);
        }
    }
}
