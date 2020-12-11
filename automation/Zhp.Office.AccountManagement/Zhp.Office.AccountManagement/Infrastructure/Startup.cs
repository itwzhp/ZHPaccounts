using Atlassian.Jira;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Security.Cryptography;
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
            this.isDevelopmentEnvironment = builder.GetContext().EnvironmentName == "Development";

            var s = builder.Services;
            
            s.AddSingleton(CreateJiraClient);
            s.AddTransient<ITicketRepository, JiraTicketRepository>();

            //s.AddSingleton(CreateGraphClient);

            s.AddSingleton(LoadConfig);

            s.AddTransient<IPasswordGenerator, PasswordGenerator>();
            s.AddSingleton(s => RandomNumberGenerator.Create());
        }

        //private IGraphServiceClient CreateGraphClient(IServiceProvider c)
        //{
        //    //return new GraphServiceClient(new DelegateAuthenticationProvider());
        //}

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
                    EnableRequestTrace = this.isDevelopmentEnvironment,
                    EnableUserPrivacyMode = true,
                });
        }
    }
}
