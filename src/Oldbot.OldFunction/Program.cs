using System;
using System.Threading.Tasks;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.Lambda.RuntimeSupport;
using Amazon.Lambda.Serialization.Json;
using Oldbot.Utilities.SlackAPI.Extensions;

namespace Oldbot.OldFunction
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            //var function = HandlerWrapper.GetHandlerWrapper((Func<string, ILambdaContext, string>)UpperThingy, new JsonSerializer());
            var function = HandlerWrapper.GetHandlerWrapper(FunctionHandler(), new JsonSerializer());
            
            using(var handlerWrapper = function)
            {
                using(var bootstrap = new LambdaBootstrap(handlerWrapper))
                {
                    await bootstrap.RunAsync();
                }
            }
        }
        
        public static Func<APIGatewayProxyRequest,ILambdaContext, Task<APIGatewayProxyResponse>> FunctionHandler()
        {
            var handler = new OldnessValidator();
            return handler.Validate;
        }
    }
}