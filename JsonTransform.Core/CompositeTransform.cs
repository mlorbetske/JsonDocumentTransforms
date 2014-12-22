using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;

namespace JsonTransform.Core
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

        public static IObjectTransform Load(string filePath)
        {
            var fileText = File.ReadAllText(filePath);
            var rawTransformObject = JObject.Parse(fileText);
            var transforms = new List<IObjectTransform>();

            foreach (var property in rawTransformObject.Properties())
            {
                transforms.Add(new Transform(property));
            }

            return new CompositeTransform(transforms);
        }
    }
}