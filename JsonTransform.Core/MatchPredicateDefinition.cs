using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JsonTransform.Core
{
    public class MatchPredicateDefinition
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("configuration")]
        public JToken Configuration { get; set; }
    }
}