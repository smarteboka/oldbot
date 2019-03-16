using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using SlackAPI;

namespace Oldbot.Utilities.SlackAPIFork
{
    public class SlackClientBase
    {
        protected const string APIBaseLocation = "https://slack.com/api/";
        
        public Task<K> APIRequestAsync<K>(Tuple<string, string>[] getParameters, Tuple<string, string>[] postParameters)
            where K : Response
        {
            RequestPath path = RequestPath.GetRequestPath<K>();
            //TODO: Custom paths? Appropriate subdomain paths? Not sure.
            //Maybe store custom path in the requestpath.path itself?

            Uri requestUri = GetSlackUri(Path.Combine(APIBaseLocation, path.Path), getParameters);
            HttpWebRequest request = CreateWebRequest(requestUri);

            //This will handle all of the processing.
            var state = new RequestStateForTask<K>(request, postParameters);
            return state.Execute();
        }
        
        protected HttpWebRequest CreateWebRequest(Uri requestUri)
        {
            var httpWebRequest = (HttpWebRequest)HttpWebRequest.Create(requestUri);
            return httpWebRequest;
        }

        protected Uri GetSlackUri(string path, Tuple<string, string>[] getParameters)
        {
            string parameters = getParameters
                .Where(x => x.Item2 != null)
                .Select(new Func<Tuple<string, string>, string>(a =>
                {
                    try
                    {
                        return string.Format("{0}={1}", Uri.EscapeDataString(a.Item1), Uri.EscapeDataString(a.Item2));
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException(string.Format("Failed when processing '{0}'.", a), ex);
                    }
                }))
                .Aggregate((a, b) =>
                {
                    if (string.IsNullOrEmpty(a))
                        return b;
                    else
                        return string.Format("{0}&{1}", a, b);
                });

            Uri requestUri = new Uri(string.Format("{0}?{1}", path, parameters));
            return requestUri;
        }
    }
}