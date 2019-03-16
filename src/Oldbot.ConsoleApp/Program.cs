using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Oldbot.ConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var host = new HostBuilder()
                    .ConfigureAppConfiguration((hostContext, configApp) => { configApp.AddEnvironmentVariables(); })
                    .ConfigureServices((context, services) => { services.AddOldbot(context.Configuration); })
                    .ConfigureLogging((context, configLogging) =>
                    {
                        configLogging
                            .SetMinimumLevel(LogLevel.Trace)
                            .AddConsole()
                            .AddDebug();
                    })
                    .UseConsoleLifetime()
                    .Build();

            using (host)
            {
                await host.RunAsync();
            }
        }
    }
}
