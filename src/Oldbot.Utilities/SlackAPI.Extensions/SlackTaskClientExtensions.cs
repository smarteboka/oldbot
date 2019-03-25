using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Oldbot.Utilities.SlackAPIFork;
using SlackAPI;
using SearchResponseMessages = Oldbot.Utilities.SlackAPIFork.SearchResponseMessages;
using SearchSort = Oldbot.Utilities.SlackAPIFork.SearchSort;
using SearchSortDirection = Oldbot.Utilities.SlackAPIFork.SearchSortDirection;
using SlackTaskClient = Oldbot.Utilities.SlackAPIFork.SlackTaskClient;

namespace Oldbot.Utilities.SlackAPI.Extensions
{
    public class SlackTaskClientExtensions : SlackTaskClient, ISlackClient
    {
        /// <summary>
        /// Need a seperate bottoken when using the reactions API,
        /// or else the app will post reactions as the user installing the app :/
        /// </summary>
        protected readonly string BotToken;
        
        public SlackTaskClientExtensions(string appToken, string bottoken) : base(appToken)
        {
            BotToken = bottoken;
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
                unfurl_links = "false",
                unfurl_media = "true",
                as_user = "false",
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
            request.Headers.Add("Authorization", $"Bearer {SlackToken}");
            request.Content = httpContent;

            return await httpClient.SendAsync(request);
        }
        
        public async Task<HttpResponseMessage[]> AddReactions(string channelId, string thread_ts)
        {
            var t1 = React(channelId, thread_ts, "older_man");
            var t2 = React(channelId, thread_ts, "older_man::skin-tone-2");
            var t3 = React(channelId, thread_ts, "older_man::skin-tone-3");
            var t4 = React(channelId, thread_ts, "older_man::skin-tone-4");
            var t5 = React(channelId, thread_ts, "older_man::skin-tone-5");
            var res = await Task.WhenAll(t1, t2, t3, t4, t5);
            return res;
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
            request.Headers.Add("Authorization", $"Bearer {BotToken}");
            request.Content = httpContent;
            return await httpClient.SendAsync(request);
        }
    }

    public class NoopClient : ISlackClient
    {
        public Task<SearchResponseMessages> SearchMessagesAsync(string query, SearchSort? sorting = null, SearchSortDirection? direction = null, bool enableHighlights = false, int? count = null, int? page = null)
        {
            return Task.FromResult(new SearchResponseMessages()
            {
                messages = new SlackAPIFork.SearchResponseMessagesContainer
                {
                    matches = new SlackAPIFork.ContextMessage[0]
                }
            });
        }
        
        public Task<HttpResponseMessage> SendMessage(string channel, string message, string eventTs, string permalink)
        {
            var httpResponseMessage = new HttpResponseMessage
            {
                Content = new StringContent("{}")
            };
            return Task.FromResult(httpResponseMessage);
        }

        public Task<HttpResponseMessage[]> AddReactions(string channelId, string thread_ts)
        {
            var httpResponseMessage = new []
            { 
                new HttpResponseMessage
                {
                    Content = new StringContent("{}")
                }
            };
            return Task.FromResult(httpResponseMessage);
        }
    }
}