using System;
using Newtonsoft.Json.Linq;

namespace JsonTransform.Core
{
    public class PatchRemoveTransform : PatchTransform
    {
        private readonly WalkPlan _plan;

        private PatchRemoveTransform(WalkPlan plan)
        {
            _plan = plan;
        }

        public static PatchRemoveTransform Create(JObject jObject, string path)
        {
            return new PatchRemoveTransform(JsonPointer.Parse(path));
        }

        public override void Apply<T>(ref T source)
        {
            var target = _plan.Walk(source);

            if (target.Item3 != null)
            {
                throw new Exception("Could not locate the element to remove");
            }

            if (target.Item1 == null)
            {
                //We removed the document...
                source = null;
                return;
            }

            target.Item2.Remove();
        }
    }
}