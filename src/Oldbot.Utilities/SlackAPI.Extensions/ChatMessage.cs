using SlackAPI;

namespace Oldbot.Utilities.SlackAPI.Extensions
{
    public class ChatMessage
    {
        public string Channel { get; set; }
        public string Text { get; set; }
        public string Parse { get; set; }

        public int Link_Names { get; set; } = 1;
        public string thread_ts { get; set; }
        public string unfurl_links { get; set; }
        public string unfurl_media { get; set; }
        public string as_user { get; set; }
        
        public Attachment[] attachments { get; set; }
    }
}