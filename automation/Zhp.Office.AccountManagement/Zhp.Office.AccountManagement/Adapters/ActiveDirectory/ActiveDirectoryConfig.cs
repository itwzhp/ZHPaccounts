using System;
using System.Collections.Generic;

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

        public Dictionary<string, Guid> RemovableLicenses { get; } = new();
    }
}
