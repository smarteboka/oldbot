using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;

namespace Oldbot.Utilities
{
    public class UrlCleaner
    {
        private static string[] Verboten = 
        {
            "utm_source",
            "utm_medium",
            "utm_content",
            "utm_campaign", 
            "utm_term", 
            "ei", 
            "gs_l", 
            "si", 
            "fbclid", 
            "igshid", 
            "notif_t", 
            "notif_id", 
            "acontext", 
            "aref"
        };
        
        public static string CleanForTrackingQueryParams(string url)
        {
            return Verboten.Aggregate(url, (current, verbot) => RemoveQueryStringByKey(current, verbot));
        }

        private static string RemoveQueryStringByKey(string url, string key)
        {                   
            var uri = new Uri(url);
            var newQueryString = QueryHelpers.ParseQuery(uri.Query);
            newQueryString.Remove(key);
            var pagePathWithoutQueryString = uri.GetLeftPart(UriPartial.Path);
            
            return newQueryString.Count > 0 ? $"{pagePathWithoutQueryString}?{KeyValues(newQueryString)}" : pagePathWithoutQueryString.TrimEnd('/');
        }

        private static string KeyValues(Dictionary<string,StringValues> newQueryString)
        {
            return newQueryString.Select(c => $"{c.Key}={c.Value}").Aggregate((x, y) => x + "&" + y);
        }
    }
}