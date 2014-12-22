using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JsonTransform
{
    [JsonObject]
    public class TransformAction
    {
        [JsonProperty("type")]
        public ActionType Type { get; set; }

        [JsonProperty("value")]
        public JToken Value { get; set; }

        public void Apply(JToken target, JToken definition)
        {
            var inst = definition.DeepClone();
            InjectSelfRefs(target, inst);

            switch (Type)
            {
                case ActionType.Replace:
                    target.Replace(inst);
                    break;
                case ActionType.Insert:
                    JArray arr = target as JArray;

                    if (arr != null)
                    {
                        if (inst.Type != JTokenType.Array)
                        {
                            arr.Add(inst);
                        }
                        else
                        {
                            foreach (var item in (JArray) inst)
                            {
                                arr.Add(item);
                            }
                        }
                    }
                    break;
            }
        }

        private static void InjectSelfRefs(JToken target, JToken definition)
        {
            switch (definition.Type)
            {
                case JTokenType.Array:
                    for(var i = 0; i < ((JArray)definition).Count; ++i)
                    {
                        InjectSelfRefs(target, ((JArray)definition)[i]);
                    }
                    break;
                case JTokenType.Object:
                    JToken result;
                    if (((JObject)definition).TryGetValue("$existing", out result))
                    {
                        definition.Replace(target.SelectToken(result.ToString(), true).DeepClone());
                        break;
                    }

                    foreach (var property in ((JObject) definition).Properties())
                    {
                        InjectSelfRefs(target, property.Value);
                    }

                    break;
            }
        }
    }
}