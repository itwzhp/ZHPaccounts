using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.UserSecrets;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Reflection;
using System.Threading;
using Zhp.Office.AccountManagement.Cli;
using Zhp.Office.AccountManagement.Domain.Services;
using Zhp.Office.AccountManagement.Functions;
using Zhp.Office.AccountManagement.Infrastructure;

[assembly: UserSecretsId("c30a6998-6579-48ef-96bb-d97f196f9591")]

await new HostBuilder()
                .ConfigureServices(services => {
                    Startup.Configure(services);
                    services.AddTransient<OldAccountCleaner>();
                    services.AddTransient<CreateAccounts>();
                    services.AddHostedService<RunJob>();
                })
                .ConfigureAppConfiguration((ctx, config) => {
                    config.AddJsonFile("appsettings.json", false, false)
                        .AddUserSecrets(Assembly.GetExecutingAssembly(), false);
                })
                .ConfigureLogging(b => {
                    b.AddConsole()
                        .SetMinimumLevel(LogLevel.Information)
                        .AddDebug();
                })
                .UseEnvironment("Development")
                .RunConsoleAsync();

