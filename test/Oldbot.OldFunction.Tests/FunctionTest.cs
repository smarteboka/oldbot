using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

using Xunit;
using Amazon.Lambda.Core;
using Amazon.Lambda.TestUtilities;
using Amazon.Lambda.APIGatewayEvents;
using Newtonsoft.Json;
using Oldbot.OldFunction;
using Oldbot.Utilities;
using Oldbot.Utilities.SlackAPI.Extensions;
using Oldbot.Utilities.SlackAPIFork;

namespace Oldbot.OldFunction.Tests
{
    public class FunctionTest
    {
        [Fact]
        public async Task EmptyBodyWorks()
        {
            var request = new APIGatewayProxyRequest
            {
                Body = null
            };

            var validateOldness = new OldnessValidator();
            var response = await validateOldness.Validate(request, new TestLambdaContext());
            Assert.Equal(200, response.StatusCode);
            Assert.Equal("IGNORED", response.Body);
        }
        
        [Fact]
        public async Task SlackChallengeWorks()
        {
            var request = new APIGatewayProxyRequest
            {
                Body = "{\"token\": \"troll\", \"challenge\": \"someChallenge\",\"type\": \"url_verification\"}"
            };

            var validateOldness = new OldnessValidator();
            var response = await validateOldness.Validate(request, new TestLambdaContext());
            Assert.Equal(200, response.StatusCode);
            Assert.Equal("someChallenge", response.Body);
        }
        
        [Fact]
        public async Task FindingUrlsWorks()
        {
            await TestIt("OLD", "la oss se litt på https://www.juggel.no");
            await TestIt("OLD", "<https://ilaks.no/na-kan-du-kjope-norsk-laks-pa-automater-i-singapore/>");
            await TestIt("OLD", "https://ilaks.no/na-kan-du-kjope-norsk-laks-pa-automater-i-singapore");
            await TestIt("OLD", "GI meg gi meg lox <https://ilaks.no/na-kan-du-kjope-norsk-laks-pa-automater-i-singapore/>");
        }
        
        [Fact]
        public async Task NoText()
        {
            await TestIt("IGNORED", null);
            await TestIt("IGNORED", "");
        }

        [Fact]
        public async Task SkipsBotMessages()
        {
            var payload = new SlackEventAPIPayload
            {
                Event = new Event
                {
                    Channel = "CGWGZ90KV",  // private channel #bottestsmore
                    Bot_Id = "123",
                    Text = "OLD! https://www.aftenposten.no/norge/i/L08awV/Haper-pa-mer-enn-ti-tusen-barn-og-unge-i-norske-klimastreiker?utm_source=my-unit-test"
                }
            };

            var body = JsonConvert.SerializeObject(payload, JsonSettings.SlackSettings);
        
            var request = new APIGatewayProxyRequest
            {
                Body = body
            };

            var validateOldness = new OldnessValidator();

            var response = await validateOldness.Validate(request, new TestLambdaContext());
            Assert.Equal(200, response.StatusCode);
            Assert.Equal("BOT", response.Body);
        }

        /// Scenario in chronological order:
        /// * user posts a message
        /// * slack search indexes the message
        /// * the event about the message post is sent to the lambda
        /// * => the message is marked as old, since the search returns a search hit 
        [Fact]
        public async Task DoesNotOldIfSearchReturnsEventMessage() 
        {
            var newMessage = new Event
            {
                Channel = "CGWGZ90KV", // private channel #bottestsmore
                Text = "SomeMessage containing an URL http://db.no",
                Ts = "1552671375.000200"
            };
            var payload = new SlackEventAPIPayload
            {
                Event = newMessage
            };
            
            var body = JsonConvert.SerializeObject(payload, JsonSettings.SlackSettings);
        
            var request = new APIGatewayProxyRequest
            {
                Body = body
            };

            var mockClient = new MockClient();
            mockClient.SetSearchResponse(newMessage);
            
            var validateOldness = new OldnessValidator(mockClient);

            var response = await validateOldness.Validate(request, new TestLambdaContext());
            Assert.Equal(200, response.StatusCode);
            Assert.Equal("NEW", response.Body);
        }

        private static async Task TestIt(string expected, string slackMessage)
        {
            var payload = new SlackEventAPIPayload
            {
                Event = new Event
                {
                    Channel = "CGWGZ90KV", // private channel #bottestsmore
                    Text = slackMessage,
                    Ts = "1552671375.000200"
                }
            };

            var body = JsonConvert.SerializeObject(payload, JsonSettings.SlackSettings);

            var request = new APIGatewayProxyRequest
            {
                Body = body
            };

            var validateOldness = new OldnessValidator();

            var response = await validateOldness.Validate(request, new TestLambdaContext());
            Assert.Equal(200, response.StatusCode);
            Assert.Equal(expected, response.Body);
        }
        
        [Fact]
        public async Task Reacts() 
        {
            var newMessage = new Event
            {
                Channel = "CGY1XJRM1", // public channel #tests
                Text = "SomeMessage containing an URL https://twitter.com/AukeHoekstra/status/1104095189117911040?s=09",
                Ts = "1553108688.002800" // https://smarteboka.slack.com/messages/CGY1XJRM1/convo/CGY1XJRM1-1553108688.002800/
            };
            var payload = new SlackEventAPIPayload
            {
                Event = newMessage
            };
            
            var body = JsonConvert.SerializeObject(payload, JsonSettings.SlackSettings);
        
            var request = new APIGatewayProxyRequest
            {
                Body = body
            };

            var appToken = Environment.GetEnvironmentVariable("OldBot_SlackApiKey_SlackApp");
            var botToken = Environment.GetEnvironmentVariable("OldBot_SlackApiKey_BotUser");

            var mockClient = new SlackTaskClientExtensions(appToken,botToken);
            
            var validateOldness = new OldnessValidator(mockClient);

            var response = await validateOldness.Validate(request, new TestLambdaContext());
            Assert.Equal(200, response.StatusCode);
            Assert.Equal("ALREADY-OLDED", response.Body);
        }

        [Fact]
        public void TestFinders()
        {
            AssertChannelRegex("tests", "say this is a thing <#CGY1XJRM1|tests> with other things");
        }

        [Fact]
        public void TestUrlFinder()
        {
            var expedtedUrl = "https://itunes.apple.com/no/podcast/272-build-tech-anett-andreassen-digitalisering-i-byggebransjen/id1434899825?i=1000431627999&amp;l=nb&amp;mt=2";
            var message = "https://itunes.apple.com/no/podcast/272-build-tech-anett-andreassen-digitalisering-i-byggebransjen/id1434899825?i=1000431627999&amp;l=nb&amp;mt=2";
            AssertUrlRegex(expedtedUrl, message);
        }

        private static void AssertChannelRegex(string expected, string input)
        {
            Assert.Equal(expected, RegexHelper.FindChannelName(input));
        }
        
        private static void AssertUrlRegex(string expected, string input)
        {
            Assert.Equal(expected, RegexHelper.FindStringURl(input));
        }
    }
    

        public class MockClient : ISlackClient
        {
            public MockClient()
            {
                
            }
            
            public Task<SearchResponseMessages> SearchMessagesAsync(string query, SearchSort? sorting = null, SearchSortDirection? direction = null, bool enableHighlights = false, int? count = null, int? page = null)
            {
                return Task.FromResult(SearchResponse);
            }

            public SearchResponseMessages SearchResponse { get; set; }

            public Task<HttpResponseMessage> SendMessage(string channel, string message, string eventTs, string permalink)
            {
                var httpResponseMessage = new HttpResponseMessage
                {
                    Content = new StringContent("MockResponse")
                };
                return Task.FromResult(httpResponseMessage);
            }

            public Task<HttpResponseMessage[]> AddReactions(string channelId, string thread_ts)
            {
                var httpResponseMessage = new []
                { 
                    new HttpResponseMessage
                    {
                        Content = new StringContent("MockResponse")
                    }
                };
                return Task.FromResult(httpResponseMessage);
            }

            public void SetSearchResponse(Event newMessage)
            {
                SearchResponse = new SearchResponseMessages
                {
                    messages = new SearchResponseMessagesContainer
                    {
                        matches = new[]
                        {
                            new ContextMessage
                            {
                                text = newMessage.Text,
                                ts = newMessage.Ts
                            }
                        }
                    }
                };
            }
        }
}
