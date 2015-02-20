using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;

namespace JsonTransform.Core.ValuePredicates
{
    public class ValueMatch : IMatchPredicate
    {
        private string _pattern;

        public void Configure(JToken configuration)
        {
            _pattern = configuration.ToString();
        }

        public bool IsMatch(JToken check)
        {
            return _pattern != null && check.Type == JTokenType.String && Regex.IsMatch(check.ToString(), _pattern);
        }
    }
}
