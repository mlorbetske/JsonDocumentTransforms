using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace JsonTransform
{
    public class CompositeTransform : IObjectTransform
    {
        private readonly IEnumerable<IObjectTransform> _transforms;

        public CompositeTransform(IEnumerable<IObjectTransform> transforms)
        {
            _transforms = transforms;
        }

        public void Apply(JToken source)
        {
            foreach (var transform in _transforms)
            {
                transform.Apply(source);
            }
        }
    }
}