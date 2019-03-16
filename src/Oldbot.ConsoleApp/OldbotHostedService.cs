using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Noobot.Core;

namespace Oldbot.ConsoleApp
{
    internal class OldbotHostedService : IHostedService
    {
        private readonly INoobotCore _noobotCore;

        public OldbotHostedService(INoobotCore noobotCore)
        {
            _noobotCore = noobotCore;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await _noobotCore.Connect();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _noobotCore.Disconnect();
            return Task.CompletedTask;
        }
    }
}