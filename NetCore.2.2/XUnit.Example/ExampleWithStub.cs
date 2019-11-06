using Newtonsoft.Json;
using RestSharp;
using System;
using WireMock.RequestBuilders;
using WireMock.ResponseBuilders;
using WireMock.Server;
using WireMock.Settings;
using Xunit;

namespace XUnit.Example
{
    public class ExampleWithStub : IDisposable
    {
        private readonly FluentMockServer stub;
        private readonly string baseUrl;
        public ExampleWithStub()
        {
            var port = new Random().Next(5000, 6000);
            baseUrl = "http://localhost:" + port;
            
            stub = FluentMockServer.Start(new FluentMockServerSettings
            {
                Urls = new[] { "http://+:" + port }
            });
        }

        public void Dispose() => Dispose(true);

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                stub.Stop();
                stub.Dispose();
            }
        }

        [Fact]
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
            Assert.Equal(200, (int)response.StatusCode);
            Assert.Equal(JsonConvert.SerializeObject(bodyContent), response.Content);
        }


    }
}
