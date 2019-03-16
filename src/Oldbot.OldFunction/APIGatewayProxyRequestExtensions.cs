using Amazon.Lambda.APIGatewayEvents;
using Oldbot.Utilities;

namespace Oldbot.OldFunction
{
    public static class APIGatewayProxyRequestExtensions
    {
        public static bool IsSlackChallengeRequest(this APIGatewayProxyRequest request)
        {
            return !string.IsNullOrEmpty(request.Body.As<ChallengeRequest>().Challenge);
        }
    }
}