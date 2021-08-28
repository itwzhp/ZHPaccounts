using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Reflection;
using Zhp.Office.AccountManagement.Infrastructure;

var host = new HostBuilder()
                .ConfigureFunctionsWorkerDefaults()
                .ConfigureServices(services => Startup.Configure(services))
                .ConfigureAppConfiguration((ctx, config) =>
                {
                    bool isDevelopmentEnvironment = ctx.HostingEnvironment.IsDevelopment();

                    config.AddJsonFile("appsettings.json", false, false);

                    if (isDevelopmentEnvironment)
                        config.AddUserSecrets(Assembly.GetExecutingAssembly(), false);

                    config.AddEnvironmentVariables();
                })
                .Build();

host.Run();
