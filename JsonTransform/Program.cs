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
            if (args.Length < 2 || args.Length > 4 || args.Any(x => new[]{"/?", "-?", "/help", "-help"}.Contains(x)))
            {
                ShowHelp();
                return;
            }

            var customFlag = args[0];
            var argOffset = 0;

            if (string.Equals(customFlag, "-custom", StringComparison.OrdinalIgnoreCase))
            {
                argOffset = 1;
            }

            var sourceFile = args[argOffset];

            if (!File.Exists(sourceFile))
            {
                ShowHelp(new FileNotFoundException(sourceFile));
                return;
            }

            var transformFile = args[argOffset + 1];

            if (!File.Exists(transformFile))
            {
                ShowHelp(new FileNotFoundException(transformFile));
                return;
            }

            var transform = argOffset == 1 ? CompositeTransform.Load(transformFile) : PatchDocument.Load(transformFile);
            JToken source = LoadFile(sourceFile);
            transform.Apply(ref source);
            var outputText = JsonConvert.SerializeObject(source);

            if (args.Length - argOffset == 3)
            {
                var outputFile = args[args.Length - 1];
                var fullPath = Path.Combine(Environment.CurrentDirectory, outputFile);
                var outputDir = Path.GetDirectoryName(fullPath);
                Directory.CreateDirectory(outputDir);
                File.WriteAllText(fullPath, outputText);
            }
            else
            {
                Console.WriteLine(outputText);
            }
        }

        private static void ShowHelp(Exception ex = null)
        {
            Console.WriteLine(Environment.CommandLine);
            Console.WriteLine();

            Console.WriteLine("JsonTransform.exe [-custom] InputFile.json TransformFile.json [Output.json]");
            Console.WriteLine("-custom                  - If specified, indicates that the transform file is not a JSON Patch file");
            Console.WriteLine("InputFile.json           - A JSON document");
            Console.WriteLine("TransformFile.json       - A JSON transform, JSON Patch format is expected unless the -custom option is specified");
            Console.WriteLine("Output.json (Optional)   - The location to write the output");
            Console.WriteLine();

            if (ex != null)
            {
                Console.WriteLine(ex.Message);
                return;
            }

            Console.WriteLine("JSON Patch");
            Console.WriteLine("==========");
            Console.WriteLine("Official JSON Patch website: http://jsonpatch.com/");
            Console.WriteLine("Official JSON Patch JSON schema: http://json.schemastore.org/json-patch");
            Console.WriteLine("JSON Patch IETF Standard (RFC 6902): http://tools.ietf.org/html/rfc6902");
            Console.WriteLine();

            Console.WriteLine("Custom");
            Console.WriteLine("======");
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

            Environment.Exit(-1);
        }

        private static JObject LoadFile(string file)
        {
            var text = File.ReadAllText(file);
            return JObject.Parse(text);
        }
    }
}
