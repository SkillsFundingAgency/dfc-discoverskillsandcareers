using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Dfc.DiscoverSkillsAndCareers.Models.Extensions
{
    [ExcludeFromCodeCoverage]
    public class JsonSettings
    {
        public static JsonSerializerSettings Instance = new JsonSerializerSettings
        {
            DateParseHandling = DateParseHandling.DateTimeOffset
        };
    }
    
    [ExcludeFromCodeCoverage]
    public static class JsonExtensions
    {
        static Regex ReplaceRegex = new Regex(@"\{prop:(\w*)\}");

        public static async Task<T> FromJson<T>(this Task<string> source)
        {
            var data = await source;
            return JsonConvert.DeserializeObject<T>(data, JsonSettings.Instance);
        }
        
        public static T FromJson<T>(this string source)
        {
            var data = source;
            return JsonConvert.DeserializeObject<T>(data, JsonSettings.Instance);
        }

        public static void InterpolateProperties(this JObject source)
        {
            foreach (var property in source.Properties())
            {
                
                if (property.Value.Type == JTokenType.String)
                {
                    var propValue = property.Value.Value<string>();
                    var hasReplacements = false;
                    foreach (Match match in ReplaceRegex.Matches(propValue))
                    {
                        var group = match.Groups[1];
                        if (source.ContainsKey(group.Value))
                        {
                            var newValue = source.Value<string>(group.Value);
                            propValue = propValue.Replace(match.Value, newValue);
                            hasReplacements = true;
                        }
                    }

                    if(hasReplacements) {
                        source.SelectToken(property.Name).Replace(propValue);
                    }
                }
            }
        }
    }
}