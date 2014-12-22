using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;

namespace JsonTransform.Core
{
    [JsonObject]
    public class TransformAction
    {
        public TransformAction()
        {
            MatchPredicates = Enumerable.Empty<IMatchPredicate>();
        }

        [JsonProperty("type")]
        public ActionType Type { get; set; }

        [JsonProperty("value")]
        public JToken Value { get; set; }

        [JsonProperty("matchPredicates")]
        public List<MatchPredicateDefinition> MatchPredicateDefinitions
        {
            get { return null; }
            set
            {
                var predicates = new List<IMatchPredicate>();
                MatchPredicates = predicates;

                if (value == null)
                {
                    return;
                }

                foreach (var definition in value)
                {
                    var type = System.Type.GetType(definition.Type);
                    if (typeof (IMatchPredicate).IsAssignableFrom(type))
                    {
                        var instance = (IMatchPredicate) Activator.CreateInstance(type);
                        instance.Configure(definition.Configuration);
                        predicates.Add(instance);
                    }
                }
            }
        }

        [JsonIgnore]
        public IEnumerable<IMatchPredicate> MatchPredicates { get; private set; }

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

    public class MatchPredicateDefinition
    {
        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("configuration")]
        public JToken Configuration { get; set; }
    }
}