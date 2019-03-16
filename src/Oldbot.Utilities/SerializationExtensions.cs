using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Oldbot.Utilities
{
    public static class SerializationExtensions
    {
        public static T As<T>(this string body) where T:new()
        {
            return !string.IsNullOrEmpty(body) ? JsonConvert.DeserializeObject<T>(body, JsonSettings.SlackSettings) : new T();
        }
        
        public static string ToSerialized<T>(this T theObj)
        {
            return JsonConvert.SerializeObject(theObj, JsonSettings.SlackSettings);
        }
    }
    
    public class JsonSettings 
    {
        public static readonly JsonSerializerSettings SlackSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Formatting = Formatting.Indented
        };
    }
}