namespace Oldbot.ConsoleApp
{
    internal interface INoobotConfig
    {
        string SlackApiKey { get; }
        string SlackApiKeyOauth2 { get; }
        bool HelpEnabled { get; }
        bool StatsEnabled { get; }
        bool AboutEnabled { get; }
    }
}