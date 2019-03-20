using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using Newtonsoft.Json;
using Oldbot.Utilities;
using Oldbot.Utilities.SlackAPI.Extensions;
using Oldbot.Utilities.SlackAPIFork;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace Oldbot.OldFunction
{
    public class OldnessValidator
    {
        private static readonly string UserToken = Environment.GetEnvironmentVariable("SlackApiKeyOauth2");
        private static readonly string BotToken = Environment.GetEnvironmentVariable("SlackApiKey");
        
        private readonly ISlackClient _slackClient;

        /// <summary>
        /// Default constructor that Lambda will invoke.
        /// </summary>
        public OldnessValidator() : this(new SlackTaskClientExtensions(UserToken, BotToken))
        {
        }

        public OldnessValidator(ISlackClient slackClient)
        {
            _slackClient = slackClient;
        }

        /// <summary>
        /// A Lambda function to respond to HTTP Get methods from API Gateway
        /// </summary>
        /// <param name="request"></param>
        /// <returns>The list of blogs</returns>
        public async Task<APIGatewayProxyResponse> Validate(APIGatewayProxyRequest request, ILambdaContext context)
        {
            context.Logger.LogLine("Received:" + request.Body);
            if (request.IsSlackChallengeRequest())
            {
                var challengeRequest = request.Body.As<ChallengeRequest>();
                return Respond(challengeRequest.Challenge, context);
            }

            var slackEvent = request.Body.As<SlackEventAPIPayload>();

            if (slackEvent == null || slackEvent.Event == null)
            {
                return Respond("IGNORED", context);
            }
            
            if (string.IsNullOrEmpty(slackEvent.Event.Text))
            {
                return Respond("IGNORED", context);
            }

            if (slackEvent.Event != null && slackEvent.Event.Text != null && !string.IsNullOrEmpty(slackEvent.Event.Bot_Id))
                return Respond("BOT", context);

            if (!string.IsNullOrEmpty(slackEvent.Event.SubType) && slackEvent.Event.SubType == "bot_message")
                return Respond("BOT", context);
           
            var urls = UrlFinder.FindIn(slackEvent.Event.Text);

            if (urls.Any())
            {
                var firstUrl = urls.First();
                var cleansedUrl = UrlCleaner.CleanForTrackingQueryParams(firstUrl.Value);
                var searchResults = await _slackClient.SearchMessagesAsync(cleansedUrl, SearchSort.timestamp, count: 1, direction: SearchSortDirection.asc);

                if (searchResults != null && searchResults.messages.matches.Any())
                {
                    context.Logger.LogLine(JsonConvert.SerializeObject(searchResults.messages.matches));
                    var r = searchResults.messages.matches.FirstOrDefault();
                    
                    if (r == null) 
                        return Respond($"NO-URL-IN-MSG", context);
                    
                    if (r.ts == slackEvent.Event.Ts)
                        return Respond("NEW", context);

                    var message = $"postet av @{r.username} for {TimeSpanExtensions.Ago(r.ts)} siden.";
                    var response = await _slackClient.SendMessage(slackEvent.GetChannel(), message, slackEvent.Event.Ts, r.permalink);
                    var body = await response.Content.ReadAsStringAsync();
                    context.Logger.LogLine("Sent message. Response:" + JsonConvert.SerializeObject(body));
                    
                    var reactionResponse = await _slackClient.AddReactions(slackEvent.GetChannel(), slackEvent.Event.Ts);
                    var reactionResponseBody = await reactionResponse.Content.ReadAsStringAsync();
                    context.Logger.LogLine("Sent reaction. Response:" + JsonConvert.SerializeObject(reactionResponseBody));

                    
                    return Respond($"OLD", context);
                }
            }
            
            return Respond($"NO-URL-IN-MSG", context);
        }

        private static APIGatewayProxyResponse Respond(string body, ILambdaContext context)
        {
            context.Logger.LogLine($"Treated as: {body}");

            return new APIGatewayProxyResponse
            {
                Body = body,
                Headers = new Dictionary<string, string>
                {
                    { "Content-Type", "text/plain"}
                },                
                StatusCode = 200
            };
        }
    }
}
