using Newtonsoft.Json;
using RestSharp;
using System;
using WireMock.Server;
using WireMock.Settings;
using Xunit;

namespace XUnit.Example
{
    public class ExampleWithFile : IDisposable
    {
        private readonly FluentMockServer stub;
        private readonly string baseUrl;
        public ExampleWithFile()
        {
            var port = new Random().Next(5000, 6000);
            baseUrl = "http://localhost:" + port;
            
            stub = FluentMockServer.Start(new FluentMockServerSettings
            {
                Urls = new[] { "http://+:" + port },
                ReadStaticMappings = true
            }); ;
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

            var client = new RestClient(baseUrl);
            var request = new RestRequest("/api/products");

            var response = client.Execute(request);
            Assert.Equal(200, (int)response.StatusCode);
            Assert.Equal(JsonConvert.SerializeObject(bodyContent), response.Content);
        }



    }
}
