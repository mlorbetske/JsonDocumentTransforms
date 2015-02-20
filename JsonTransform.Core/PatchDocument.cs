using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json.Linq;

namespace JsonTransform.Core
{
    public class PatchDocument : IObjectTransform
    {
        private readonly IEnumerable<PatchTransform> _transforms;

        public PatchDocument(IEnumerable<PatchTransform> transforms)
        {
            _transforms = transforms;
        }

        public void Apply<T>(ref T source)
            where T : JToken
        {
            foreach (var transform in _transforms)
            {
                transform.Apply(ref source);
            }
        }

        public static IObjectTransform Load(string filePath)
        {
            var fileText = File.ReadAllText(filePath);
            var rawTransformObject = JArray.Parse(fileText);
            var transforms = new List<IObjectTransform>();

            foreach (var transform in rawTransformObject)
            {
                var patch = PatchTransform.Create(transform);
                
                if (patch != null)
                {
                    transforms.Add(patch);
                }
            }

            return new CompositeTransform(transforms);
        }
    }
}