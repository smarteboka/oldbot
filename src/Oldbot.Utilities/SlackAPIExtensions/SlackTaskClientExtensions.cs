using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Oldbot.Utilities.SlackAPIFork;

namespace Oldbot.Utilities.SlackAPIExtensions
{
    public class SlackTaskClientExtensions : SlackTaskClient, ISlackClient
    {
        public SlackTaskClientExtensions(string token) : base(token)
        {
            
        }
        
        public async Task<HttpResponseMessage> SendMessage(string channelId, string message, string thread_ts)
        {
            var httpClient = new HttpClient();
            var stringContent = new ChatMessage
            {
                Channel = channelId,
                Text = message,
                Parse = "full",
                link_names = 1,
                thread_ts = thread_ts
                
            }.ToSerialized();
            
            var httpContent = new StringContent(stringContent,Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Post, "https://slack.com/api/chat.postMessage");
            request.Headers.Add("Authorization", $"Bearer {APIToken}");
            request.Content = httpContent;

            return await httpClient.SendAsync(request);
        }
    }
}