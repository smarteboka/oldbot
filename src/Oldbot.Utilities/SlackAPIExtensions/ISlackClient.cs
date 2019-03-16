using System.Net.Http;
using System.Threading.Tasks;
using Oldbot.Utilities.SlackAPIFork;

namespace Oldbot.Utilities.SlackAPIExtensions
{
    public interface ISlackClient
    {
        Task<SearchResponseMessages> SearchMessagesAsync(string query, SearchSort? sorting = null, SearchSortDirection? direction = null, bool enableHighlights = false, int? count = null, int? page = null);
        Task<HttpResponseMessage> SendMessage(string getChannel, string message, string eventTs);
    }
}