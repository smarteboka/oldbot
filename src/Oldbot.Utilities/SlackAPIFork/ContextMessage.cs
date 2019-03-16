using SlackAPI;

namespace Oldbot.Utilities.SlackAPIFork
{
    public class ContextMessage : Message
    {
        public Message previous_2;
        public Message previous;
        public Message next;
        public Message next_2;
    }
    
    public class Message : SlackSocketMessage
    {
        public Channel channel;
        public string ts; //epoch
        public string user;
        public string username;
        public string text;
        public bool is_starred;
        public string permalink;
        public Reaction[] reactions;
    }

    public class Channel
    {
        public string name;
        public string id;
    }
}