using System;
using Newtonsoft.Json.Linq;

namespace JsonTransform.Core
{
    public class PatchMoveTransform : PatchTransform
    {
        private readonly WalkPlan _source;
        private readonly WalkPlan _target;

        private PatchMoveTransform(WalkPlan source, WalkPlan target)
        {
            _source = source;
            _target = target;
        }

        public override void Apply<T>(ref T source)
        {
            var value = _source.Walk(source);

            if (value.Item2 == null)
            {
                throw new Exception("Could not locate the value to move");
            }

            var item = value.Item2.DeepClone();

            var parent = value.Item2.Parent as JProperty;
            if (parent != null)
            {
                ((JObject) value.Item1).Remove(parent.Name);
            }
            else
            {
                value.Item2.Remove();
            }
            
            var add = new PatchAddTransform(_target, item);
            add.Apply(ref source);
        }

        public static PatchMoveTransform Create(JObject jObject, string path)
        {
            JToken from;
            if (!jObject.TryGetValue("from", out from) || from == null || from.Type != JTokenType.String)
            {
                return null;
            }

            return new PatchMoveTransform(JsonPointer.Parse(from.Value<string>()), JsonPointer.Parse(path));
        }
    }
}