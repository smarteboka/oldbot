using System;

namespace Oldbot.ConsoleApp
{
    internal class OldbotConfig : INoobotConfig
    {
        public string SlackApiKey { get; set; }
        
        public string SlackApiKeyOauth2 { get; } = Environment.GetEnvironmentVariable("SlackApiKeyOauth2");

        public bool HelpEnabled { get; set; } = false;
        public bool StatsEnabled { get; set; } = false;

        public bool AboutEnabled { get; set; } = false;
    }
}