using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using RestSharp;
using System;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using WireMock.Settings;

namespace MSTest.Example
{
    [TestClass]
    public class ExampleWithStub
    {
        private static FluentMockServer stub;
        private static string baseUrl;

        [ClassInitialize]
        public static void PrepareClass(TestContext context)
        {
            if (context is null){throw new ArgumentNullException(nameof(context));}

            var port = new Random().Next(5000, 6000);
            baseUrl = "http://localhost:" + port;

            stub = FluentMockServer.Start(new FluentMockServerSettings
            {
                Urls = new[] { "http://+:" + port },
                ReadStaticMappings = true
            });
        }

        [ClassCleanup]
        public static void TearDown()
        {
            stub.Stop();
        }

        [TestMethod]
        public void Test()
        {
            var bodyContent = new[] {
                                new {id = 1, description = "Book A" },
                                new {id = 2, description = "Book B" }
                            };

            stub.Given(
                Request
                .Create()
                    .WithPath("/api/products"))
                .RespondWith(
                    Response.Create()
                        .WithStatusCode(200)
                        .WithHeader("Content-Type", "aplication/json")
                        .WithBodyAsJson(bodyContent));

            var client = new RestClient(baseUrl);
            var request = new RestRequest("/api/products");

            var response = client.Execute(request);
            Assert.AreEqual(200, (int)response.StatusCode);
            Assert.AreEqual(JsonConvert.SerializeObject(bodyContent), response.Content);
        }


    }
}
