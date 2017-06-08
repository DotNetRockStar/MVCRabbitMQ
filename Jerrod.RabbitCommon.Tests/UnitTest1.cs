using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jerrod.RabbitCommon.Management;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Jerrod.RabbitCommon.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void GetQueues()
        {
            var client = new ManagementApiClient();
            var queues = client.GetQueues();
        }

        [TestMethod]
        public void Test()
        {
            TestClass2 x = new TestClass2() { Id = "1", Height = "something" };
            string str = JsonConvert.SerializeObject(x);

            JsonConvert.DeserializeObject<TestClass1>(str, new JsonSerializerSettings()
            {
                Error = delegate(object sender, ErrorEventArgs args)
                {
                    if (args.ErrorContext.Error.GetType().FullName == typeof(JsonReaderException).FullName)
                        args.ErrorContext.Handled = true;
                }
            });
        }
    }

    public class TestClass1
    {
        public string Id { get; set; }
        public int Height { get; set; }
    }

    public class TestClass2
    {
        public string Id { get; set; }
        public string Height { get; set; }
    }
}
