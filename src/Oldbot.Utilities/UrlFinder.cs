using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Oldbot.Utilities
{
    public static class UrlFinder
    {
        private static readonly Regex UrlsRegex = new Regex("((http(s)?://|www\\.)([A-Z0-9.-:]{1,})\\.[0-9A-Z?;~&#=\\-_\\./]{2,})", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static MatchCollection FindIn(string messageText)
        {
            return UrlsRegex.Matches(messageText);
        }
    }
}