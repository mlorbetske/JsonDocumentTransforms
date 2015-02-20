using System;
using Newtonsoft.Json.Linq;

namespace JsonTransform.Core
{
    public class WalkPlan
    {
        public static readonly WalkPlan Identity = new WalkPlan("");

        private readonly string _path;

        public WalkPlan(string path)
        {
            _path = path;
        }

        private static JToken Step(string path, JToken current)
        {
            if (current == null || path == null)
            {
                return null;
            }

            if (current.Type == JTokenType.Array)
            {
                int index;
                if (!int.TryParse(path, out index))
                {
                    return null;
                }

                var arr = (JArray) current;

                if (arr.Count > index)
                {
                    return ((JArray) current)[index];
                }

                return null;
            }

            if (current.Type == JTokenType.Object)
            {
                var obj = (JObject) current;
                JToken result;
                
                if (!obj.TryGetValue(path, out result))
                {
                    return null;
                }

                return result;
            }

            return null;
        }

        public Tuple<JToken, JToken, WalkPlan> Walk(JToken document)
        {
            var current = this;
            var token = document;
            var parent = (JToken) null;

            while (token != null && current != null)
            {
                //always skip the first entry as it'll always refer to the document
                current = current.Next;

                if (current == null)
                {
                    continue;
                }

                var newToken = Step(current._path, token);
                parent = token;
                token = newToken;
            }

            return new Tuple<JToken, JToken, WalkPlan>(parent, token, current);
        }

        public WalkPlan Next { get; internal set; }
        
        public string Path { get { return _path; } }
    }
}