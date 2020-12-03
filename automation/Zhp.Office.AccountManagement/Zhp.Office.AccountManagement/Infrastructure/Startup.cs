using Atlassian.Jira;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Zhp.Office.AccountManagement.TicketSystem;

namespace Zhp.Office.AccountManagement.Infrastructure
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            var s = builder.Services;
            
            s.AddSingleton(CreateJiraClient);
            s.AddSingleton(LoadConfig);
            s.AddTransient<ITicketRepository, JiraTicketRepository>();
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
            return Jira.CreateOAuthRestClient(
                config.JiraUri,
                config.ConsumerKey,
                config.ConsumerSecret,
                config.OAuthAccessToken,
                config.OAuthTokenSecret);
        }
    }
}
