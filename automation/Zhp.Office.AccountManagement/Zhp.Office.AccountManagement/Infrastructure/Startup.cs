using Atlassian.Jira;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.Graph.Auth;
using Microsoft.Identity.Client;
using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Zhp.Office.AccountManagement.Adapters.ActiveDirectory;
using Zhp.Office.AccountManagement.Adapters.TicketSystem;
using Zhp.Office.AccountManagement.Domain.Ports;
using Zhp.Office.AccountManagement.Domain.Services;

namespace Zhp.Office.AccountManagement.Infrastructure
{
    internal static class Startup
    {
        public static void Configure(IServiceCollection s)
        {

            s.AddSingleton(CreateJiraClient);
            s.AddTransient<ITicketRepository, JiraTicketRepository>();
            s.AddTransient<IJiraRequestMapper, JiraRequestMapper>();

            s.AddSingleton(CreateGraphClient);
            s.AddTransient<IAccountManager, AzureActiveDirectoryClient>();
            s.AddTransient<ILastUsageRepository, AzureDirectoryLastUsageRepository>();

            s.AddSingleton(LoadConfig);

            s.AddTransient<IPasswordGenerator, PasswordGenerator>();
            s.AddTransient<ICommentFormatter, CommentFormatter>();
            s.AddTransient<IMailAddressGenerator, MailAddressGenerator>();
            s.AddTransient<AccountsCreatingService>();
            s.AddTransient<OldAccountCleaner>();
            s.AddSingleton(s => RandomNumberGenerator.Create());

            GraphServiceClient CreateGraphClient(IServiceProvider c)
            {
                var config = c.GetRequiredService<FunctionConfig>().ActiveDirectory;
                bool isDevelopmentEnvironment = c.GetRequiredService<IHostEnvironment>().IsDevelopment();
                IAuthenticationProvider provider;

                if (isDevelopmentEnvironment)
                {
                    var logger = c.GetRequiredService<ILogger<InteractiveAuthenticationProvider>>();
                    void log(Microsoft.Identity.Client.LogLevel level, string message, bool containsPii)
                        => logger.LogDebug(message);

                    var publicClientApplication = PublicClientApplicationBuilder
                        .Create(config.DevClientId)
                        .WithAuthority(AzureCloudInstance.AzurePublic, config.TenantId)
                        .WithRedirectUri("http://localhost:1234")
                        .WithLogging(log)
                        .Build();

                    provider = new InteractiveAuthenticationProvider(publicClientApplication);
                }
                else
                {
                    var cert = Convert.FromBase64String(config.ProdCertificateBase64);
                    var confidentialClientApplication = ConfidentialClientApplicationBuilder
                        .Create(config.ProdClientId)
                        .WithTenantId(config.TenantId)
                        .WithCertificate(new X509Certificate2(cert, config.ProdCertPassword))
                        .Build();

                    provider = new ClientCredentialProvider(confidentialClientApplication);
                }

                return new GraphServiceClient(provider);
            }

            static FunctionConfig LoadConfig(IServiceProvider c) =>
                c.GetRequiredService<IConfiguration>()
                    .Get<FunctionConfig>(o => o.BindNonPublicProperties = true);

            static Jira CreateJiraClient(IServiceProvider c)
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
}
