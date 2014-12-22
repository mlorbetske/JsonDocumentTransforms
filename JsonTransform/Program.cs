using System;
using System.IO;
using System.Linq;
using JsonTransform.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JsonTransform
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length < 2 || args.Length > 3 || args.Any(x => new[]{"/?", "-?", "/help", "-help"}.Contains(x)))
            {
                ShowHelp();
                return;
            }

            var sourceFile = args[0];

            if (!File.Exists(sourceFile))
            {
                ShowHelp(new FileNotFoundException(sourceFile));
                return;
            }

            var transformFile = args[1];

            if (!File.Exists(transformFile))
            {
                ShowHelp(new FileNotFoundException(transformFile));
                return;
            }

            var transform = CompositeTransform.Load(transformFile);
            var source = LoadFile(sourceFile);
            transform.Apply(source);
            var outputText = JsonConvert.SerializeObject(source);

            if (args.Length == 3)
            {
                var outputFile = args[2];
                File.WriteAllText(outputFile, outputText);
            }
            else
            {
                Console.WriteLine(outputText);
            }
        }

        private static void ShowHelp(Exception ex = null)
        {
            Console.WriteLine("JsonTransform.exe InputFile.json TransformFile.json [Output.json]");
            Console.WriteLine("InputFile.json           - A JSON document");
            Console.WriteLine("TransformFile.json       - A JSON transform");
            Console.WriteLine("Output.json (Optional)   - The location to write the output");
            Console.WriteLine();

            if (ex != null)
            {
                Console.WriteLine(ex.Message);
                return;
            }

            Console.WriteLine("Transform documents must have a JSON object at their top level");
            Console.WriteLine("Each property of the top level object must be the path pattern to the element or elements to act upon");
            Console.WriteLine("  Each path segment (separated by the / character) must either be a valid regular expression or the token **");
            Console.WriteLine("  The ** token indicates that zero or more path segments may occur without any restriction on their value");
            Console.WriteLine("  Ex. The path \"foo/**/bar\" will match the paths \"#/foo/bar\", \"#/foo/baz/bar\", \"#/foo/baz/quux/bar\", etc.");
            Console.WriteLine();

            Console.WriteLine("Each pattern's (key) value must be an object describing the action to be taken on the element");
            Console.WriteLine("  The object must contain two properties, type and value");
            Console.WriteLine("  type   - May either be insert or replace");
            Console.WriteLine("     insert  - The single value (or array of values) specified in value will be inserted into the located array");
            Console.WriteLine("     replace - The contents of the located item will be replaced by the value of the value property");
            Console.WriteLine("  value  - The value to insert or replace the located value with");
            Console.WriteLine("     The value of the value property will be explored for properties called $existing");
            Console.WriteLine("     If a property called $existing is found, the object containing it is replaced with the value located by");
            Console.WriteLine("     the path, after the value of the $existing property is processed as a JSON Path (Json.Net) against that");
            Console.WriteLine("     element.");
        }

        private static JObject LoadFile(string file)
        {
            var text = File.ReadAllText(file);
            return JObject.Parse(text);
        }
    }
}
