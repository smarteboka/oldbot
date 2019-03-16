using Microsoft.Extensions.Options;
using Noobot.Core.Configuration;

namespace Oldbot.ConsoleApp
{
    internal class NoobotConfig : IConfigReader, INoobotConfig
    {
        private readonly OldbotConfig _config;

        public NoobotConfig(IOptions<OldbotConfig> config)
        {
            _config = config.Value;
        }

        //dont-care
        public T GetConfigEntry<T>(string entryName)
        {
            return default;
        }

        public string SlackApiKey => _config.SlackApiKey;
        
        public string SlackApiKeyOauth2  => _config.SlackApiKey;

        public bool HelpEnabled => _config.HelpEnabled;
        public bool StatsEnabled => _config.StatsEnabled;
        
        public bool AboutEnabled => _config.AboutEnabled;
    }
}