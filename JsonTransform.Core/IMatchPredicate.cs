using Newtonsoft.Json.Linq;

namespace JsonTransform.Core
{
    public interface IMatchPredicate
    {
        void Configure(JToken configuration);

        bool IsMatch(JToken check);
    }
}
