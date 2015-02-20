using System;
using Newtonsoft.Json.Linq;

namespace JsonTransform.Core
{
    public class PatchAddTransform : PatchTransform
    {
        private readonly JToken _tokenToAdd;
        private readonly WalkPlan _plan;

        internal PatchAddTransform(WalkPlan plan, JToken tokenToAdd)
        {
            _plan = plan;
            _tokenToAdd = tokenToAdd;
        }

        public static PatchAddTransform Create(JObject jObject, string path)
        {
            JToken value;
            if (!jObject.TryGetValue("value", out value) || value == null)
            {
                return null;
            }

            return new PatchAddTransform(JsonPointer.Parse(path), value);
        }

        public override void Apply<T>(ref T source)
        {
            var target = _plan.Walk(source);

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

            if (target.Item3 != null && target.Item3.Next != null)
            {
                //We couldn't process the whole path
                throw new Exception(string.Format("Couldn't process the whole path, couldn't navigate to {0} of {1}", target.Item3.Path, target.Item1.ToString()));
            }

            //Check to see if we're in non-array replace mode (located item is non-null, the parent exists and is not an array)
            if (target.Item2 != null && target.Item1 != null && target.Item1.Type != JTokenType.Array)
            {
                //We're in replace mode
                target.Item2.Replace(_tokenToAdd);
                return;
            }

            var lastPathPart = _plan;

            while (lastPathPart.Next != null)
            {
                lastPathPart = lastPathPart.Next;
            }

            //We're in add mode
            if (target.Item1.Type == JTokenType.Array)
            {
                if (target.Item2 != null)
                {
                    //Since the parent is an array, we're looking at an array insert with a valid index
                    var index = int.Parse(lastPathPart.Path);
                    ((JArray) target.Item1).Insert(index, _tokenToAdd);
                    return;
                }

                if (lastPathPart.Path != "-")
                {
                    //We're looking at an out of bounds value
                    throw new Exception(string.Format("Index {0} is out of range (or otherwise not valid) for array {1}", lastPathPart.Path, target.Item1));
                }

                ((JArray)target.Item1).Add(_tokenToAdd);
            }

            if (target.Item1.Type == JTokenType.Object)
            {
                //Since replacements have already been handled, we need to add a member to the array

                if (target.Item3 == null)
                {
                    throw new Exception(string.Format("Don't know what to name the member on object {0} for the value {1}", target.Item1, _tokenToAdd));
                }

                ((JObject)target.Item1).Add(lastPathPart.Path, _tokenToAdd);
            }
        }
    }
}