using System;
using System.IO;
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
            transform.Apply(obj);
            Assert.IsTrue(obj["a"]["b"] is JObject);
        }

        [TestMethod]
        public void TestMethod2()
        {
            var transform = CompositeTransform.Load("Transform2.json");
            var instanceText = File.ReadAllText("instance.json");
            var obj = JObject.Parse(instanceText);
            Assert.IsTrue(obj["a"] is JObject);
            transform.Apply(obj);
            Assert.IsTrue(obj["a"] is JArray);
        }

        [TestMethod]
        public void TestMethod3()
        {
            var transform = CompositeTransform.Load("Transform3.json");
            var instanceText = File.ReadAllText("instance.json");
            var obj = JObject.Parse(instanceText);
            Assert.IsTrue(obj["a"]["b"].Type == JTokenType.Integer);
            transform.Apply(obj);
            Assert.IsTrue(obj["a"]["b"] is JArray);
        }
    }
}
