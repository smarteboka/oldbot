using Common.Logging;
using Microsoft.Extensions.DependencyInjection;
using Noobot.Core;
using Noobot.Core.Configuration;
using Noobot.Core.DependencyResolution;

namespace Oldbot.ConsoleApp
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddOldbot(this IServiceCollection services, Microsoft.Extensions.Configuration.IConfiguration configuration)
        {
            services.AddSingleton<IConfiguration,OldbotConfiguration>();
            services.Configure<OldbotConfig>(configuration);
            services.AddSingleton<IConfigReader,NoobotConfig>();
            services.AddSingleton<ILog,DotNetCoreLogger>();
            services.AddSingleton<ContainerFactory>();
            services.AddSingleton(s =>
            {
                var containerFactory = s.GetService<ContainerFactory>();
                var container = containerFactory.CreateContainer();
                return container.GetNoobotCore();
            });
            
            services.AddHostedService<OldbotHostedService>();

            return services;
        }
    }
}