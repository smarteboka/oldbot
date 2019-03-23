using Noobot.Core.Configuration;

namespace Oldbot.ConsoleApp
{
    public class OldbotConfiguration : ConfigurationBase
    {
        public OldbotConfiguration()
        {
            UseMiddleware<SaySomethingMiddleware>();
        }
    }
}