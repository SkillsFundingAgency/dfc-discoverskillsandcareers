using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;

namespace Dfc.DiscoverSkillsAndCareers.SupportApp.Models
{
    public class WorkflowStep
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public Action Action { get; set; }
        public string ContentType { get; set; }
        public JObject Data { get; set; }
        
        public Relation[] Relates { get; set; }
    }
}