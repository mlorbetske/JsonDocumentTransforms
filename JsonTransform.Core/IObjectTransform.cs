using Newtonsoft.Json.Linq;

namespace JsonTransform.Core
{
    public interface IObjectTransform
    {
        void Apply(JToken source);
    }
}