using Newtonsoft.Json.Linq;

namespace JsonTransform.Core
{
    public interface IObjectTransform
    {
        void Apply<T>(ref T source)
            where T : JToken;
    }
}