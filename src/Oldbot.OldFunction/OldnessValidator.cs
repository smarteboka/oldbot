using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

using Amazon.Lambda.Core;
using Amazon.Lambda.APIGatewayEvents;
using Newtonsoft.Json;
using Oldbot.Utilities;
using Oldbot.Utilities.SlackAPIExtensions;
using Oldbot.Utilities.SlackAPIFork;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.Json.JsonSerializer))]

namespace Oldbot.OldFunction
{
    public class OldnessValidator
    {
        private static readonly string BotToken = Environment.GetEnvironmentVariable("SlackApiKey");
        private static readonly string UserToken = Environment.GetEnvironmentVariable("SlackApiKeyOauth2");
        private readonly ISlackClient _slackClient;

        /// <summary>
        /// Default constructor that Lambda will invoke.
        /// </summary>
        public OldnessValidator() : this(new SlackTaskClientExtensions(UserToken))
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
                return Respond(challengeRequest.Challenge);
            }

            var slackEvent = request.Body.As<SlackEventAPIPayload>();

            if (slackEvent == null || slackEvent.Event == null)
            {
                return Respond("IGNORED");
            }
            
            if (string.IsNullOrEmpty(slackEvent.Event.Text))
            {
                return Respond("IGNORED");
            }

            if (slackEvent.Event != null && slackEvent.Event.Text != null && !string.IsNullOrEmpty(slackEvent.Event.Bot_Id))
                return Respond("BOT");
           
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
                        return Respond($"NO-URL-IN-MSG");
                    
                    if (r.ts == slackEvent.Event.Ts)
                        return Respond("NEW");

                    var message = $":older_man: postet av @{r.username} for {TimeSpanExtensions.Ago(r.ts)} siden. {r.permalink}";
                    var response = await _slackClient.SendMessage(slackEvent.GetChannel(), message, slackEvent.Event.Ts);
                    var body = await response.Content.ReadAsStringAsync();
                    context.Logger.LogLine("Sent message. Response:" + JsonConvert.SerializeObject(body));
                    return Respond($"OLD");
                }
            }
            
            return Respond($"NO-URL-IN-MSG");
        }

        private static APIGatewayProxyResponse Respond(string body)
        {
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

    /*
     *
     * {
    "token": "<token>",
    "team_id": "T0EC3DG3A",
    "api_app_id": "AGXHANA5D",
    "event": {
        "type": "message",
        "subtype": "message_changed",
        "hidden": true,
        "message": {
            "client_msg_id": "A61DACD9-E220-4119-B75D-2FB5CD364307",
            "type": "message",
            "text": "Interessant artikkel om 737 Max: <https://www.nrk.no/urix/flyet-som-har-blitt-en-hodepine-for-boeing-1.14470605>",
        
     */
}
