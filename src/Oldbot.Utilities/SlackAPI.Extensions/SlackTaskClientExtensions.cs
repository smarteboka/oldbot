using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using SlackAPI;
using SlackTaskClient = Oldbot.Utilities.SlackAPIFork.SlackTaskClient;

namespace Oldbot.Utilities.SlackAPI.Extensions
{
    public class SlackTaskClientExtensions : SlackTaskClient, ISlackClient
    {
        public SlackTaskClientExtensions(string userToken, string botUserToken) : base(userToken, botUserToken)
        {
            
        }
        
        public async Task<HttpResponseMessage> SendMessage(string channelId, string message, string thread_ts, string permalink)
        {
            var httpClient = new HttpClient();
            var stringContent = new ChatMessage
            {
                Channel = channelId,
                Parse = "full",
                Link_Names = 1,        
                thread_ts = thread_ts,
                unfurl_links = "true",
                unfurl_media = "true",
                Text = permalink,
                attachments = new []
                {
                    new Attachment
                    {
                        text = $":older_man: {message}",
                        color = "#FF0000"
                    }
                }
                
            }.ToSerialized();
            
            var httpContent = new StringContent(stringContent,Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Post, "https://slack.com/api/chat.postMessage");
            request.Headers.Add("Authorization", $"Bearer {BotApiToken}");
            request.Content = httpContent;

            return await httpClient.SendAsync(request);
        }
        
        public async Task<HttpResponseMessage> AddReactions(string channelId, string thread_ts)
        {
            await React(channelId, thread_ts, "older_man");
            await React(channelId, thread_ts, "older_man::skin-tone-2");
            await React(channelId, thread_ts, "older_man::skin-tone-3");
            await React(channelId, thread_ts, "older_man::skin-tone-4");
            return await React(channelId, thread_ts, "older_man::skin-tone-5");
        }

        private async Task<HttpResponseMessage> React(string channelId, string thread_ts, string olderMan)
        {
            var httpClient = new HttpClient();
            var stringContent = new Reaction
            {
                Name = olderMan,
                Channel = channelId,
                Timestamp = thread_ts
            }.ToSerialized();

            var httpContent = new StringContent(stringContent, Encoding.UTF8, "application/json");
            var request = new HttpRequestMessage(HttpMethod.Post, "https://slack.com/api/reactions.add");
            request.Headers.Add("Authorization", $"Bearer {BotApiToken}");
            request.Content = httpContent;
            return await httpClient.SendAsync(request);
        }
    }
}