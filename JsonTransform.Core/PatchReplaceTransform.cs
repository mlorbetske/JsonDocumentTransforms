using System;
using Newtonsoft.Json.Linq;

namespace JsonTransform.Core
{
    public class PatchReplaceTransform : PatchTransform
    {
        private readonly JToken _tokenToAdd;
        private readonly WalkPlan _plan;

        private PatchReplaceTransform(WalkPlan plan, JToken tokenToAdd)
        {
            _plan = plan;
            _tokenToAdd = tokenToAdd;
        }

        public static PatchReplaceTransform Create(JObject jObject, string path)
        {
            JToken value;
            if (!jObject.TryGetValue("value", out value) || value == null)
            {
                return null;
            }

            return new PatchReplaceTransform(JsonPointer.Parse(path), value);
        }

        public override void Apply<T>(ref T source)
        {
            var target = _plan.Walk(source);

            if (target.Item3 != null)
            {
                throw new Exception("Could not locate the element to replace");
            }

            if (target.Item1 == null)
            {
                //We're looking at the root of the document, replace the whole thing if the walk completed

                if (target.Item3 == null)
                {
                    source = (T)_tokenToAdd;
                    return;
                }

                throw new Exception(string.Format("Couldn't process the whole path, couldn't navigate to {0} of {1}", target.Item3.Path, target.Item2.ToString()));
            }

            target.Item2.Replace(_tokenToAdd);
        }
    }
}