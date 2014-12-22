using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace JsonTransform.Core
{
    public class Transform : IObjectTransform
    {
        private readonly MatchStateMachine _matcher;
        private readonly TransformAction _action;

        private void GetObjectPaths(JObject obj, Dictionary<string[], JToken> results)
        {
            foreach (var property in obj.Properties())
            {
                var path = property.Name;
                var childPaths = GetPaths(property.Value);
                results[new[] {path}] = property.Value;

                foreach (var child in childPaths)
                {
                    if (child.Key.Length == 0)
                    {
                        continue;
                    }

                    var expandedPart = new string[child.Key.Length + 1];
                    expandedPart[0] = path;
                    Array.Copy(child.Key, 0, expandedPart, 1, child.Key.Length);
                    results[expandedPart] = child.Value;
                }
            }
        }

        private void GetArrayPaths(JArray arr, Dictionary<string[], JToken> results)
        {
            for (var i = 0; i < arr.Count; ++i)
            {
                var path = i + "";
                var childPaths = GetPaths(arr[i]);
                results[new[] { path }] = arr[i];

                foreach (var child in childPaths)
                {
                    if (child.Key.Length == 0)
                    {
                        continue;
                    }

                    var expandedPart = new string[child.Key.Length + 1];
                    expandedPart[0] = path;
                    Array.Copy(child.Key, 0, expandedPart, 1, child.Key.Length);
                    results[expandedPart] = child.Value;
                }
            }
        }

        private Dictionary<string[], JToken> GetPaths(JToken token)
        {
            var results = new Dictionary<string[], JToken>
            {
                {new string[0], token}
            };

            switch (token.Type)
            {
                case JTokenType.Object:
                    GetObjectPaths((JObject) token, results);
                    break;
                case JTokenType.Array:
                    GetArrayPaths((JArray) token, results);
                    break;
            }
             
            return results;
        }

        public Transform(JProperty property)
        {
            var pattern = property.Name;
            _matcher = new MatchStateMachine(pattern.Split('/'));
            var transform = (JObject)property.Value;
            _action = transform["action"].ToObject<TransformAction>();
        }

        public void Apply(JToken source)
        {
            var allPaths = GetPaths(source);

            foreach (var path in allPaths)
            {
                if (IsMatch(path.Key, path.Value))
                {
                    _action.Apply(path.Value, _action.Value);
                }
            }
        }

        private bool IsMatch(string[] key, JToken value)
        {
            return _matcher.IsMatch(key) && _action.MatchPredicates.All(x => x.IsMatch(value));
        }
    }
}