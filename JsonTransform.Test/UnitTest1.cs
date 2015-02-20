using System;
using System.IO;
using System.Linq;
using JsonTransform.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace JsonTransform.Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var transform = CompositeTransform.Load("Transform1.json");
            var instanceText = File.ReadAllText("instance.json");
            var obj = JObject.Parse(instanceText);
            transform.Apply(ref obj);
            Assert.IsTrue(obj["a"]["b"] is JObject);
        }

        [TestMethod]
        public void TestMethod2()
        {
            var transform = CompositeTransform.Load("Transform2.json");
            var instanceText = File.ReadAllText("instance.json");
            var obj = JObject.Parse(instanceText);
            Assert.IsTrue(obj["a"] is JObject);
            transform.Apply(ref obj);
            Assert.IsTrue(obj["a"] is JArray);
        }

        [TestMethod]
        public void TestMethod3()
        {
            var transform = CompositeTransform.Load("Transform3.json");
            var instanceText = File.ReadAllText("instance.json");
            var obj = JObject.Parse(instanceText);
            Assert.IsTrue(obj["a"]["b"].Type == JTokenType.Integer);
            transform.Apply(ref obj);
            Assert.IsTrue(obj["a"]["b"] is JArray);
        }

        [TestMethod]
        public void PatchTestMethod1()
        {
            var transform = PatchDocument.Load("PatchTransform1.json");
            var instanceText = File.ReadAllText("instance.json");
            var obj = JObject.Parse(instanceText);
            var a = (JObject) obj["a"];

            JToken c;
            Assert.IsFalse(a.TryGetValue("c", out c));

            transform.Apply(ref obj);
            Assert.IsTrue(a.TryGetValue("c", out c));

            Assert.AreEqual(JTokenType.Object, c.Type);
            JToken test;
            Assert.IsTrue(((JObject) c).TryGetValue("test", out test));
            Assert.AreEqual(JTokenType.String, test.Type);
            Assert.AreEqual("value", test.Value<string>());
        }

        [TestMethod]
        public void PatchTestMethod2()
        {
            var transform = PatchDocument.Load("PatchTransform2.json");
            var instanceText = File.ReadAllText("instance.json");
            var obj = JObject.Parse(instanceText);

            JToken test;
            Assert.IsFalse(obj.TryGetValue("test", out test));

            transform.Apply(ref obj);

            Assert.IsTrue(obj.TryGetValue("test", out test));
            Assert.AreEqual(JTokenType.String, test.Type);
            Assert.AreEqual("value", test.Value<string>());
        }

        [TestMethod]
        public void PatchTestMethod3()
        {
            var transform = PatchDocument.Load("PatchTransform3.json");
            var instanceText = File.ReadAllText("instance.json");
            var obj = JObject.Parse(instanceText);

            JToken test;
            Assert.IsFalse(obj.TryGetValue("test", out test));

            transform.Apply(ref obj);

            var b = (JArray) obj["b"];
            Assert.IsTrue(b.Values<int>().SequenceEqual(new[] {0, 1, 2, 3, 4, 5}));
        }

        [TestMethod]
        public void PatchTestMethod4()
        {
            var transform = PatchDocument.Load("PatchTransform4.json");
            var instanceText = File.ReadAllText("instance.json");
            var obj = JObject.Parse(instanceText);

            JToken test;
            Assert.IsFalse(obj.TryGetValue("test", out test));

            transform.Apply(ref obj);

            var b = (JArray) obj["b"];
            Assert.IsTrue(b.Values<int>().SequenceEqual(new[] {1, 3, 4}));
        }

        [TestMethod]
        public void PatchTestMethod5()
        {
            var transform = PatchDocument.Load("PatchTransform5.json");
            var instanceText = File.ReadAllText("instance.json");
            var obj = JObject.Parse(instanceText);

            JToken test;
            Assert.IsFalse(obj.TryGetValue("test", out test));

            transform.Apply(ref obj);

            var b = (JArray) obj["b"];
            Assert.IsTrue(b.Values<int>().SequenceEqual(new[] {1, 1, 3, 4}));
        }

        [TestMethod]
        public void PatchTestMethod6()
        {
            var transform = PatchDocument.Load("PatchTransform6.json");
            var instanceText = File.ReadAllText("instance.json");
            var obj = JObject.Parse(instanceText);

            JToken oldB = obj["b"].DeepClone();
            transform.Apply(ref obj);

            JToken b;
            Assert.IsFalse(obj.TryGetValue("b", out b));

            var a = obj["a"];
            Assert.IsTrue(JToken.DeepEquals(oldB, a));
        }

        [TestMethod]
        public void PatchTestMethod7()
        {
            var transform = PatchDocument.Load("PatchTransform7.json");
            var instanceText = File.ReadAllText("instance.json");
            var obj = JObject.Parse(instanceText);

            JToken oldB = obj["b"].DeepClone();
            transform.Apply(ref obj);

            JToken b;
            Assert.IsTrue(obj.TryGetValue("b", out b));

            var a = obj["a"]["c"];
            Assert.IsTrue(JToken.DeepEquals(oldB, a));
        }

        [TestMethod]
        [ExpectedException(typeof (Exception))]
        public void PatchTestMethod8()
        {
            var transform = PatchDocument.Load("PatchTransform8.json");
            var instanceText = File.ReadAllText("instance.json");
            var obj = JObject.Parse(instanceText);

            transform.Apply(ref obj);
        }

        [TestMethod]
        public void PatchTestMethod9()
        {
            var transform = PatchDocument.Load("PatchTransform9.json");
            var instanceText = File.ReadAllText("instance.json");
            var obj = JObject.Parse(instanceText);

            transform.Apply(ref obj);
        }
    }
}
