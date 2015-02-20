using System;
using Newtonsoft.Json.Linq;

namespace JsonTransform.Core
{
    public class PatchTestTransform : PatchTransform
    {
        private WalkPlan _plan;
        private JToken _value;

        private PatchTestTransform(WalkPlan plan, JToken value)
        {
            _plan = plan;
            _value = value;
        }

        public override void Apply<T>(ref T source)
        {
            var target = _plan.Walk(source);

            if (target.Item2 == null)
            {
                throw new Exception("Could not locate the value to test");
            }

            if (!JToken.DeepEquals(target.Item2, _value))
            {
                throw new Exception("Test failed");
            }
        }

        public static PatchTestTransform Create(JObject jObject, string path)
        {
            JToken value;
            if (!jObject.TryGetValue("value", out value) || value == null)
            {
                return null;
            }

            return new PatchTestTransform(JsonPointer.Parse(path), value);
        }
    }
}