using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.IO;
using Microsoft.JSON.Core.Schema;

namespace JsonTransform.VisualStudio
{
    [Export(typeof(IJSONSchemaSelector))]
    internal class SchemaExporter : IJSONSchemaSelector
    {
        public string GetSchemaFor(string fileLocation)
        {
            if (Path.GetExtension(fileLocation).Equals(".json-patch", StringComparison.OrdinalIgnoreCase))
            {
                return "http://json.schemastore.org/json-patch";
            }

            return null;
        }

        public IEnumerable<string> GetAvailableSchemas()
        {
            return null;
        }
    }
}
