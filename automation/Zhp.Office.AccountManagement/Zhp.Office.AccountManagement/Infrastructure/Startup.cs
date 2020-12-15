using Atlassian.Jira;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.Graph.Auth;
using Microsoft.Identity.Client;
using System;
using System.Security.Cryptography;
using Zhp.Office.AccountManagement.Adapters.ActiveDirectory;
using Zhp.Office.AccountManagement.Adapters.TicketSystem;
using Zhp.Office.AccountManagement.Domain.Ports;
using Zhp.Office.AccountManagement.Domain.Services;

namespace Zhp.Office.AccountManagement.Infrastructure
{
    public class Startup : FunctionsStartup
    {
        private bool isDevelopmentEnvironment;

        public override void Configure(IFunctionsHostBuilder builder)
        {
            isDevelopmentEnvironment = builder.GetContext().EnvironmentName != "Production";

            var s = builder.Services;

            s.AddSingleton(CreateJiraClient);
            s.AddTransient<ITicketRepository, JiraTicketRepository>();

            s.AddSingleton(CreateGraphClient);
            s.AddTransient<IAccountManager, AzureActiveDirectoryClient>();

            s.AddSingleton(LoadConfig);

            s.AddTransient<IPasswordGenerator, PasswordGenerator>();
            s.AddTransient<IMailAddressGenerator, MailAddressGenerator>();
            s.AddTransient<AccountsCreatingService>();
            s.AddSingleton(s => RandomNumberGenerator.Create());
        }

        private IGraphServiceClient CreateGraphClient(IServiceProvider c)
        {
            var config = c.GetRequiredService<FunctionConfig>().ActiveDirectory;
            IAuthenticationProvider provider;

            if (isDevelopmentEnvironment)
            {
                var logger = c.GetRequiredService<ILogger<InteractiveAuthenticationProvider>>();
                void log(Microsoft.Identity.Client.LogLevel level, string message, bool containsPii)
                    => logger.LogDebug(message);

                IPublicClientApplication publicClientApplication = PublicClientApplicationBuilder
                    .Create(config.DevClientId)
                    .WithAuthority(AzureCloudInstance.AzurePublic, config.TenantId)
                    .WithRedirectUri("http://localhost:1234")
                    .WithLogging(log)
                    .Build();

                provider = new InteractiveAuthenticationProvider(publicClientApplication);
            }
            else
            {
                // todo
                IConfidentialClientApplication confidentialClientApplication = ConfidentialClientApplicationBuilder
                    .Create(config.ProdClientId)
                    .WithTenantId("TODO")
                    .WithClientSecret("TODO")
                    .Build();

                provider = new ClientCredentialProvider(confidentialClientApplication);
            }

            return new GraphServiceClient(provider);
        }

        public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
        {
            base.ConfigureAppConfiguration(builder);

            builder.ConfigurationBuilder.SetBasePath(builder.GetContext().ApplicationRootPath)
                .AddJsonFile("appsettings.json");
        }

        private FunctionConfig LoadConfig(IServiceProvider c) => 
            c.GetRequiredService<IConfiguration>()
                .Get<FunctionConfig>(o => o.BindNonPublicProperties = true);

        private Jira CreateJiraClient(IServiceProvider c)
        {
            var config = c.GetRequiredService<FunctionConfig>().Jira;
            return Jira.CreateRestClient(
                config.JiraUri,
                config.User,
                config.Password,
                new JiraRestClientSettings
                {
                    EnableUserPrivacyMode = true,
                });
        }
    }
}
