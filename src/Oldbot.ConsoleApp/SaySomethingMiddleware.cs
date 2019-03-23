using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Noobot.Core.MessagingPipeline.Middleware;
using Noobot.Core.MessagingPipeline.Middleware.ValidHandles;
using Noobot.Core.MessagingPipeline.Request;
using Noobot.Core.MessagingPipeline.Response;
using Oldbot.Utilities;
using Oldbot.Utilities.SlackAPI.Extensions;

namespace Oldbot.ConsoleApp
{
    public class SaySomethingMiddleware:  MiddlewareBase
    {
            public SaySomethingMiddleware(IMiddleware next) : base(next)
            {
                HandlerMappings = new[]
                {
                    new HandlerMapping
                    {
                        ValidHandles = new IValidHandle[] { new StartsWithHandle("say") },
                        Description = "checks",
                        EvaluatorFunc = RespondIfLinkFound,
                        MessageShouldTargetBot = true,
                        ShouldContinueProcessing = false,
                        VisibleInHelp = false
                    }
                };
            }

            private IEnumerable<ResponseMessage> RespondIfLinkFound(IncomingMessage arg1, IValidHandle arg2)
            {
                var textToSend = arg1.RawText.Replace("say", "");

                var channel = "testss"; // default
                
                var regexed = RegexHelper.FindChannelName(arg1.RawText);
                if (!string.IsNullOrEmpty(regexed))
                {
                    channel = regexed;
                }

                textToSend = RegexHelper.RemoveChannel(textToSend);
                textToSend = RegexHelper.RemoveUser(textToSend);

                yield return ResponseMessage.ChannelMessage(channel, $"{textToSend}", new List<Attachment>());
            }
    }
}