using System;
using Newtonsoft.Json.Linq;

namespace JsonTransform.Core
{
    public abstract class PatchTransform : IObjectTransform
    {
        public static PatchTransform Create(JToken transform)
        {
            var obj = transform as JObject;

            if (obj == null)
            {
                return null;
            }

            JToken op, path;
            if (!obj.TryGetValue("op", out op) || op == null || op.Type != JTokenType.String || !obj.TryGetValue("path", out path) || path == null || path.Type != JTokenType.String)
            {
                return null;
            }

            string opString = op.Value<string>();
            string pathString = path.Value<string>();

            switch (opString)
            {
                case "add":
                    return PatchAddTransform.Create(obj, pathString);
                case "replace":
                    return PatchReplaceTransform.Create(obj, pathString);
                case "test":
                    return PatchTestTransform.Create(obj, pathString);
                case "remove":
                    return PatchRemoveTransform.Create(obj, pathString);
                case "move":
                    return PatchMoveTransform.Create(obj, pathString);
                case "copy":
                    return PatchCopyTransform.Create(obj, pathString);
            }

            throw new Exception("Unknown JSON Patch operator '" + opString + "'");
        }

        public abstract void Apply<T>(ref T source)
            where T : JToken;
    }
}