using System.Net.Http;
using System.Threading.Tasks;
using Oldbot.Utilities.SlackAPIFork;

namespace Oldbot.Utilities.SlackAPI.Extensions
{
    public interface ISlackClient
    {
        Task<SearchResponseMessages> SearchMessagesAsync(string query, SearchSort? sorting = null, SearchSortDirection? direction = null, bool enableHighlights = false, int? count = null, int? page = null);
        Task<HttpResponseMessage> SendMessage(string channel, string message, string eventTs, string permalink);
        Task<HttpResponseMessage[]> AddReactions(string channelId, string thread_ts);
    }
}