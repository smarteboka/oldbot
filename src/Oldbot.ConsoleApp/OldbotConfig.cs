using System;

namespace Oldbot.ConsoleApp
{
    internal class OldbotConfig : INoobotConfig
    {
        public string SlackApiKeyBotUser { get; } = Environment.GetEnvironmentVariable("OldBot_SlackApiKey_BotUser");
        
        public string SlackApiKeySlackApp { get; } = Environment.GetEnvironmentVariable("OldBot_SlackApiKey_SlackApp");

        public bool HelpEnabled { get; set; } = false;
        public bool StatsEnabled { get; set; } = false;

        public bool AboutEnabled { get; set; } = false;
    }
}