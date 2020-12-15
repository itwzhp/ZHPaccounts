using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using Zhp.Office.AccountManagement.Model;

namespace Zhp.Office.AccountManagement.Domain.Ports
{
    public interface IAccountManager
    {
        /// <summary>
        /// Creates user if no user with such address yet. Throws in case of error
        /// </summary>
        /// <returns>True, if user created. False if mail was already used</returns>
        ValueTask<bool> TryAddUser(ActivationRequest request, MailAddress email, string password, CancellationToken token);
    }
}
