using Newtonsoft.Json.Linq;

namespace JsonTransform.Core.ValuePredicates
{
    public class TypeMatch : IMatchPredicate
    {
        private JTokenType _type;

        public void Configure(JToken configuration)
        {
            _type = configuration.ToObject<JTokenType>();
        }

        public bool IsMatch(JToken check)
        {
            return check.Type == _type;
        }
    }
}