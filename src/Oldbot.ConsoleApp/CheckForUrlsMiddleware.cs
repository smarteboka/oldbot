using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Noobot.Core.MessagingPipeline.Middleware;
using Noobot.Core.MessagingPipeline.Middleware.ValidHandles;
using Noobot.Core.MessagingPipeline.Request;
using Noobot.Core.MessagingPipeline.Response;
using Attachment = Noobot.Core.MessagingPipeline.Response.Attachment;
using SearchResponseMessages = Oldbot.Utilities.SlackAPIFork.SearchResponseMessages;
using SearchSort = Oldbot.Utilities.SlackAPIFork.SearchSort;
using SearchSortDirection = Oldbot.Utilities.SlackAPIFork.SearchSortDirection;
using Oldbot.Utilities;
using Oldbot.Utilities.SlackAPI.Extensions;

namespace Oldbot.ConsoleApp
{
    public class CheckForUrlsMiddleware : MiddlewareBase
    {
        private readonly SlackTaskClientExtensions _client;
        
        public CheckForUrlsMiddleware(IMiddleware next) : base(next)
        {
            var oldbotConfig = new OldbotConfig();
            _client = new SlackTaskClientExtensions(oldbotConfig.SlackApiKeySlackApp, oldbotConfig.SlackApiKeyBotUser);
            HandlerMappings = new[]
            {
                new HandlerMapping
                {
                    ValidHandles = new IValidHandle[] { new AlwaysMatchHandle() },
                    Description = "Annoys the heck out of everyone",
                    EvaluatorFunc = RespondIfLinkFound,
                    MessageShouldTargetBot = false,
                    ShouldContinueProcessing = true,
                    VisibleInHelp = false
                }
            };
        }

        private IEnumerable<ResponseMessage> RespondIfLinkFound(IncomingMessage message, IValidHandle matchedHandle)
        {
            var messages = new List<ResponseMessage>();
            var urls = RegexHelper.FindURl(message.RawText);

            if (urls.Any())
            {
                var firstUrl = urls.First();
          
                SearchResponseMessages searchResults = null;
                try
                {
                    if (Uri.IsWellFormedUriString(firstUrl.Value, UriKind.Absolute))
                    {
                        var cleansedUrl = UrlCleaner.CleanForTrackingQueryParams(firstUrl.Value);
                        searchResults = _client.SearchMessagesAsync(cleansedUrl, SearchSort.timestamp, count:1, direction:SearchSortDirection.asc).GetAwaiter().GetResult();
                    }
                }
                catch (Exception e)
                {
                    messages.Add(RespondToBotChannelWithJson(e));
                }

                if (searchResults != null)
                {
                   
                    if (searchResults.messages.matches.Any())
                    {
                        var r = searchResults.messages.matches.FirstOrDefault();
                        if (r != null)
                        {
                            
                            messages.Add(message.ReplyToChannel($":older_man: postet av @{r.username} for {TimeSpanExtensions.Ago(r.ts)} siden. {r.permalink}"));
                            messages.Add(RespondToBotsChannel(Jsonify(r)));
                        }
                    }
                    
                }
                else
                {
                    messages.Add(RespondToBotsChannel("no results"));
                }
            }

            return messages;
        }

  

        private static ResponseMessage RespondToBotChannelWithJson(object searchResults)
        {
            var json = Jsonify(searchResults);
            return RespondToBotsChannel(json);
        }

        private static ResponseMessage RespondToBotsChannel(string message)
        {
            return ResponseMessage.ChannelMessage("bottests", message, new List<Attachment>());
        }

        private static string Jsonify(object searchResults)
        {
            var json = JsonConvert.SerializeObject(searchResults);
            json = "```\n" + json + "\n" + "```";
            return json;
        }
    }
}