using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SlackAPI;

namespace Oldbot.Utilities.SlackAPIFork
{
    public class SlackTaskClient : SlackClientBase
    {
        protected readonly string SlackToken;

        public SlackTaskClient(string slackToken)
        {
            SlackToken = slackToken;
        }
        
        public Task<SearchResponseMessages> SearchMessagesAsync(string query, SearchSort? sorting = null, SearchSortDirection? direction = null, bool enableHighlights = false, int? count = null, int? page = null)
        {
            List<Tuple<string, string>> parameters = new List<Tuple<string, string>>();
            parameters.Add(new Tuple<string, string>("query", query));

            if (sorting.HasValue)
                parameters.Add(new Tuple<string, string>("sort", sorting.Value.ToString()));

            if (direction.HasValue)
                parameters.Add(new Tuple<string, string>("sort_dir", direction.Value.ToString()));

            if (enableHighlights)
                parameters.Add(new Tuple<string, string>("highlight", "1"));

            if (count.HasValue)
                parameters.Add(new Tuple<string, string>("count", count.Value.ToString()));

            if (page.HasValue)
                parameters.Add(new Tuple<string, string>("page", page.Value.ToString()));

            return APIRequestWithTokenAsync<SearchResponseMessages>(parameters.ToArray());
        }

        public Task<K> APIRequestWithTokenAsync<K>(params Tuple<string, string>[] postParameters)
            where K : Response
        {
            Tuple<string, string>[] tokenArray = new Tuple<string, string>[]
            {
                new Tuple<string, string>("token", SlackToken)
            };

            return APIRequestAsync<K>(tokenArray, postParameters);
        }
    }

 
}