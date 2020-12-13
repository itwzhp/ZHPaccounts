using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using Zhp.Office.AccountManagement.Domain.Ports;
using Zhp.Office.AccountManagement.Infrastructure;
using Zhp.Office.AccountManagement.Model;

namespace Zhp.Office.AccountManagement.Adapters.ActiveDirectory
{
    public class AzureActiveDirectoryClient : IAccountManager
    {
        private readonly IGraphServiceClient client;
        private readonly ILogger<AzureActiveDirectoryClient> logger;
        private readonly bool enableChanges;

        public AzureActiveDirectoryClient(IGraphServiceClient client, FunctionConfig config, ILogger<AzureActiveDirectoryClient> logger)
        {
            this.client = client;
            this.logger = logger;
            this.enableChanges = config.EnableChanges;
        }

        public async ValueTask<bool> TryAddUser(ActivationRequest request, MailAddress email, string password, CancellationToken token)
        {
            if(!enableChanges)
            {
                this.logger.LogInformation($"Sandbox graphAPI client: adding user {email}");
                return true;
            }

            var user = new User
            {

            };

            await this.client.Users.Request().AddAsync(user);

            return false;
        }
    }
}
