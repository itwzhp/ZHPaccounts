using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Zhp.Office.AccountManagement.Model;

namespace Zhp.Office.AccountManagement.Domain.Ports
{
    public interface ITicketRepository
    {
        Task<IReadOnlyCollection<ActivationRequest>> GetApprovedActivationRequests(CancellationToken token);

        Task MarkAsDone(IEnumerable<string> ids);

        Task Test();
    }
}
