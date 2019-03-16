namespace Oldbot.Utilities.SlackAPIExtensions
{
    public class ChatMessage
    {
        public string Channel { get; set; }
        public string Text { get; set; }
        public string Parse { get; set; }

        public int link_names { get; set; } = 1;
        public string thread_ts { get; set; }
    }
}