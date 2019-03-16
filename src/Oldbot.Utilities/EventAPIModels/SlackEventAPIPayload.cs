namespace Oldbot.OldFunction
{
    public class SlackEventAPIPayload
    {
        public SlackEventAPIPayload()
        {
        }
        
        public Event Event { get; set; }

        public string GetChannel()
        {
            return Event != null ? Event.Channel : string.Empty;
        }
    }
}