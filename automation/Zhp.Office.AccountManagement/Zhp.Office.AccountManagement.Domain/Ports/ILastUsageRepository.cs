using System.Collections.Generic;
using System.Threading;
using Zhp.Office.AccountManagement.Domain.Model;

namespace Zhp.Office.AccountManagement.Domain.Ports
{
    public interface ILastUsageRepository
    {
        /// <summary>
        /// Return account names of users and wheh they last used their license
        /// </summary>
        IAsyncEnumerable<LastUsageRecord> FindLastActivity(CancellationToken token);
    }
}
