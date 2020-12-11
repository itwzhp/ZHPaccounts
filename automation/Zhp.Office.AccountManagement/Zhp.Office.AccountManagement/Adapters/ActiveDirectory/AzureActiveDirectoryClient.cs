using Microsoft.Graph;
using System.Threading.Tasks;
using Zhp.Office.AccountManagement.Domain.Ports;
using Zhp.Office.AccountManagement.Model;

namespace Zhp.Office.AccountManagement.Adapters.ActiveDirectory
{
    public class AzureActiveDirectoryClient : IAccountManager
    {
        private readonly IGraphServiceClient client;

        public AzureActiveDirectoryClient(IGraphServiceClient client)
        {
            this.client = client;
        }

        public async Task AddUser(ActivationRequest request)
        {
            var user = new User
            {

            };

            await this.client.Users.Request().AddAsync(user);
        }
    }
}
