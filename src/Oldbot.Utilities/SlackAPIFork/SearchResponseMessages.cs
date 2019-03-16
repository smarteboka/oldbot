using SlackAPI;

namespace Oldbot.Utilities.SlackAPIFork
{
    [RequestPath("search.messages")]
    public class SearchResponseMessages : Response
    {
        public SearchResponseMessagesContainer messages;
    }

    public class SearchResponseMessagesContainer
    {
        public ContextMessage[] matches;
    }

    public enum SearchSort
    {
        score,
        timestamp
    }

    public enum SearchSortDirection
    {
        asc,
        desc
    }
}