using System;

namespace Zhp.Office.AccountManagement.Adapters.ActiveDirectory
{
    public class ActiveDirectoryConfig
    {
        public string TenantId { get; private set; } = string.Empty;

        public string DevClientId { get; private set; } = string.Empty;

        public string ProdClientId { get; private set; } = string.Empty;
        public string ProdCertificateBase64 { get; private set; } = string.Empty;

        public string ProdCertPassword { get; private set; } = string.Empty;

        public Guid DefaultLicenseSku { get; private set; }

        public Guid[] OtherRemovableLicenses { get; private set; } = Array.Empty<Guid>();

        public string[] RemovableLicensesNames { get; private set; } = Array.Empty<string>();
    }
}
