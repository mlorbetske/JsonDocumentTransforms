using System.Linq;

namespace JsonTransform.Core
{
    public static class JsonPointer
    {
        public static WalkPlan Parse(string path)
        {
            var parts = path.Split('/').ToList();

            if (parts.Count == 0)
            {
                return WalkPlan.Identity;
            }

            if (parts[0] == string.Empty || parts[0] == "#")
            {
                parts.RemoveAt(0);
            }

            if (path.EndsWith("/"))
            {
                parts.RemoveAt(parts.Count - 1);
            }

            var start = new WalkPlan("");
            var current = start;

            for (var i = 0; i < parts.Count; ++i)
            {
                current.Next = new WalkPlan(parts[i]);
                current = current.Next;
            }

            return start;
        }
    }
}