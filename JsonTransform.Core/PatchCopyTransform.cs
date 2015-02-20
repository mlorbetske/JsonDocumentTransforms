using System;
using Newtonsoft.Json.Linq;

namespace JsonTransform.Core
{
    public class PatchCopyTransform : PatchTransform
    {
        private readonly WalkPlan _source;
        private readonly WalkPlan _target;

        private PatchCopyTransform(WalkPlan source, WalkPlan target)
        {
            _source = source;
            _target = target;
        }

        public override void Apply<T>(ref T source)
        {
            var value = _source.Walk(source);

            if (value.Item2 == null)
            {
                throw new Exception("Could not locate the value to copy");
            }

            var item = value.Item2;
            var add = new PatchAddTransform(_target, item);
            add.Apply(ref source);
        }

        public static PatchCopyTransform Create(JObject jObject, string path)
        {
            JToken from;
            if (!jObject.TryGetValue("from", out from) || from == null || from.Type != JTokenType.String)
            {
                return null;
            }

            return new PatchCopyTransform(JsonPointer.Parse(from.Value<string>()), JsonPointer.Parse(path));
        }
    }
}