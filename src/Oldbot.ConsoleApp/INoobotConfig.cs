namespace Oldbot.ConsoleApp
{
    internal interface INoobotConfig
    {
        string SlackApiKeyBotUser { get; }
        string SlackApiKeySlackApp { get; }
        bool HelpEnabled { get; }
        bool StatsEnabled { get; }
        bool AboutEnabled { get; }
    }
}