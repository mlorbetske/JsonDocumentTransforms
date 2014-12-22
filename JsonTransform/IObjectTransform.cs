using Newtonsoft.Json.Linq;

namespace JsonTransform
{
    public interface IObjectTransform
    {
        void Apply(JToken source);
    }
}