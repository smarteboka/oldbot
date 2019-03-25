using System.Linq;
using System.Text.RegularExpressions;

namespace Oldbot.Utilities
{
    public static class RegexHelper
    {
        private static readonly Regex UrlsRegex = new Regex("((http(s)?://|www\\.)([A-Z0-9.\\-:]{1,})\\.[0-9A-Z?;~&#=\\-_\\./]{2,})", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static MatchCollection FindURl(string messageText)
        {
            return UrlsRegex.Matches(messageText);
        }
        
        public static string FindStringURl(string messageText)
        {
            var matches = UrlsRegex.Matches(messageText);
            return matches.Any() ? matches.First().Value : null;
        }
        
        public static readonly Regex ChannelsRegex = new Regex("<#\\w+\\|(\\w+)>", RegexOptions.Compiled);

        public static string FindChannelName(string messageText)
        {
            var matches = ChannelsRegex.Matches(messageText);
            if (matches.Any() && matches.First().Groups.Any())
            {
                return matches.First().Groups.Last().Value;
            }

            return null;
        }
        
        public static string RemoveChannel(string messageText)
        {
            return ChannelsRegex.Replace(messageText, "",10);
        }
        
        public static readonly Regex UserRegex = new Regex("<@\\w+>", RegexOptions.Compiled);


        public static string RemoveUser(string messageText)
        {
            return UserRegex.Replace(messageText, "");
        }
    }
}