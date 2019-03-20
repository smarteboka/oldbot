namespace Oldbot.OldFunction
{
    public class Event
    {
        public Event()
        {
        }
        
        public string Type { get; set; }

        public string Ts { get; set; }    
        
        public string Text { get; set; }   
        public string Channel { get; set; }
        public string Bot_Id { get; set; }
        public string SubType { get; set; }
    }
}