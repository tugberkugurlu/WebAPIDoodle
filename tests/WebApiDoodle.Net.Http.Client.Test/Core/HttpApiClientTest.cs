using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebApiDoodle.Net.Http.Client;
using WebApiDoodle.Net.Http.Client.Model;
using Xunit;

namespace WebApiDoodle.Net.Http.Client.Test.Core {
    
    public class HttpApiClientTest {

        private const string GetCarsUri = "api/cars";

        [DataContract(Namespace = "", Name = "Car")]
        private class Car : IDto {

            [DataMember]
            public int Id { get; set; }

            [DataMember]
            public string Make { get; set; }

            [DataMember]
            public string Model { get; set; }

            [DataMember]
            public int Year { get; set; }

            [DataMember]
            public float Price { get; set; }
        }

        private class CarsClient : HttpApiClient<Car, int> {

            public CarsClient(HttpClient httpClient) : base(httpClient) { }

            public Task<HttpApiResponseMessage<PaginatedDto<Car>>> GetCarsAsync() {

                return base.GetAsync(GetCarsUri);
            }
        }

        private class FuncInnerHandler : HttpMessageHandler {

            private readonly Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> _handlerFunc;
            public FuncInnerHandler(Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> handlerFunc) {

                _handlerFunc = handlerFunc;
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {

                return _handlerFunc(request, cancellationToken);
            }
        }

        private static HttpResponseMessage GetDummy400JsonResponse() {

            string jsonError = "{\"Message\":\"The request is invalid.\",\"ModelState\":{\"Page\":[\"The Page field value must be minimum 1.\"],\"Take\":[\"The Take field value must be minimum 1.\"]}}";
            HttpContent content = new StringContent(jsonError, Encoding.UTF8, "application/json");

            return GetDummyResponse(HttpMethod.Post, content, HttpStatusCode.BadRequest);
        }

        private static HttpResponseMessage GetDummy400XmlResponse() {

            string xmlError = "<Error><Message>The request is invalid.</Message><ModelState><car.Make>The field Make must be a string with a maximum length of 20.</car.Make><car.Model>The Model field is required.</car.Model></ModelState></Error>";
            HttpContent content = new StringContent(xmlError, Encoding.UTF8, "application/xml");

            return GetDummyResponse(HttpMethod.Post, content, HttpStatusCode.BadRequest);
        }

        private static HttpResponseMessage GetDummy200JsonResponse() {

            string jsonPayload = "[{\"Id\":1,\"Make\":\"Make1\",\"Model\":\"Model1\",\"Year\":2010,\"Price\":10732.2},{\"Id\":2,\"Make\":\"Make2\",\"Model\":\"Model2\",\"Year\":2008,\"Price\":27233.1},{\"Id\":3,\"Make\":\"Make3\",\"Model\":\"Model1\",\"Year\":2009,\"Price\":67437.0}]";
            HttpContent content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            return GetDummyResponse(HttpMethod.Get, content, HttpStatusCode.OK);
        }

        private static HttpResponseMessage GetDummy200XmlResponse() {

            string xmlPayload = "<ArrayOfCar xmlns:i=\"http://www.w3.org/2001/XMLSchema-instance\"><Car><Id>1</Id><Make>Make1</Make><Model>Model1</Model><Price>10732.2</Price><Year>2010</Year></Car><Car><Id>2</Id><Make>Make2</Make><Model>Model2</Model><Price>27233.1</Price><Year>2008</Year></Car><Car><Id>3</Id><Make>Make3</Make><Model>Model1</Model><Price>67437</Price><Year>2009</Year></Car><Car><Id>4</Id><Make>Make4</Make><Model>Model3</Model><Price>78984.2</Price><Year>2007</Year></Car></ArrayOfCar>";
            HttpContent content = new StringContent(xmlPayload, Encoding.UTF8, "application/xml");

            return GetDummyResponse(HttpMethod.Get, content, HttpStatusCode.OK);
        }

        private static HttpResponseMessage GetDummyResponse(HttpMethod httpMethod, HttpContent content, HttpStatusCode statusCode) {

            HttpRequestMessage request = new HttpRequestMessage(httpMethod, "https://localhost/api/cars");
            HttpResponseMessage response = new HttpResponseMessage(statusCode) {
                Content = content,
                RequestMessage = request
            };

            return response;
        }

        [Fact]
        public Task HttpApiClient_GetAsync_Should_Send_A_Get_Request() { 

            // Arrange
            string baseAddress = "https://localhost";
            HttpRequestMessage requestMessage = null;
            HttpMessageHandler innerHandler = new FuncInnerHandler((req, ct) => {
                requestMessage = req;
                return TaskHelpers.FromResult<HttpResponseMessage>(new HttpResponseMessage(HttpStatusCode.OK));
            });
            HttpClient httpClient = new HttpClient(innerHandler) { BaseAddress = new Uri(baseAddress) };
            CarsClient carsClient = new CarsClient(httpClient);

            // Act
            return carsClient.GetCarsAsync()

                // Arrange
                .ContinueWith(task => {

                    Assert.NotNull(requestMessage);
                    Assert.Equal(string.Concat(baseAddress, "/", GetCarsUri), requestMessage.RequestUri.ToString(), StringComparer.InvariantCultureIgnoreCase);
                    Assert.Equal(HttpMethod.Get, requestMessage.Method);
                });
        }

        [Fact(Skip = "Because GetDummy200JsonResponse doesn't return paginated payload for now.")]
        public Task HttpApiClient_GetAsync_Should_Return_The_Proper_Object_For_Success() {

            // Arrange
            string baseAddress = "https://localhost";
            HttpMessageHandler innerHandler = new FuncInnerHandler((req, ct) =>
                TaskHelpers.FromResult<HttpResponseMessage>(GetDummy200JsonResponse())
            );
            HttpClient httpClient = new HttpClient(innerHandler) { BaseAddress = new Uri(baseAddress) };
            CarsClient carsClient = new CarsClient(httpClient);

            // Act
            return carsClient.GetCarsAsync()

                // Arrange
                .ContinueWith(task => {

                    Assert.Equal(TaskStatus.RanToCompletion, task.Status);
                    Assert.NotNull(task.Result);
                    Assert.NotNull(task.Result.Model);
                    Assert.True(task.Result.Model.Items.Count() > 0);
                });
        }
    }
}