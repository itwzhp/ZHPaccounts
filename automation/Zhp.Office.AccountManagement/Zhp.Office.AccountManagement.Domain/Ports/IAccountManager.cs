using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using Zhp.Office.AccountManagement.Model;

namespace Zhp.Office.AccountManagement.Domain.Ports
{
    public interface IAccountManager
    {
        ValueTask<bool> TryAddUser(ActivationRequest request, MailAddress email, string password, CancellationToken token);
    }
}
