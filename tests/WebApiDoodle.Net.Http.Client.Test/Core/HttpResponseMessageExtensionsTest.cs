using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace WebApiDoodle.Net.Http.Client.Test.Core {
    
    public class HttpResponseMessageExtensionsTest {

        public class GetHttpApiResponseAsync_For_HttpResponseMessage_Task {

            [Fact]
            public Task GetHttpApiResponseAsync_For_HttpResponseMessage_Task_Should_Directly_Return_The_Response_For_Success() {

                // Arrange
                HttpStatusCode statusCode = HttpStatusCode.NoContent;
                HttpResponseMessage response = GetDummyResponse(HttpMethod.Delete, new ByteArrayContent(new byte[0]), statusCode);
                IEnumerable<MediaTypeFormatter> formatters = new MediaTypeFormatterCollection();
                Task<HttpResponseMessage> responseTask = RunDelayed(40, () => response);

                // Act
                return responseTask.GetHttpApiResponseAsync(formatters)

                    // Assert
                    .ContinueWith(task => {

                        Assert.Equal(TaskStatus.RanToCompletion, task.Status);
                        Assert.NotNull(task.Result.Response);
                        Assert.Equal(statusCode, task.Result.Response.StatusCode);
                    });
            }

            [Fact]
            public Task GetHttpApiResponseAsync_For_HttpResponseMessage_Task_Should_Deserialize_To_HttpApiError_For_400_Json() {

                // Arrange
                HttpResponseMessage response = GetDummy400JsonResponse();
                IEnumerable<MediaTypeFormatter> formatters = new MediaTypeFormatterCollection();
                Task<HttpResponseMessage> responseTask = RunDelayed(40, () => response);

                // Act
                return responseTask.GetHttpApiResponseAsync(formatters)

                    // Assert
                    .ContinueWith(task => {

                        Assert.Equal(TaskStatus.RanToCompletion, task.Status);
                        Assert.NotNull(task.Result.HttpError);
                    });
            }

            [Fact]
            public Task GetHttpApiResponseAsync_For_HttpResponseMessage_Task_Should_Deserialize_To_HttpApiError_For_400_Xml() {

                // Arrange
                HttpResponseMessage response = GetDummy400XmlResponse();
                IEnumerable<MediaTypeFormatter> formatters = new MediaTypeFormatterCollection();
                Task<HttpResponseMessage> responseTask = RunDelayed(40, () => response);

                // Act
                return responseTask.GetHttpApiResponseAsync(formatters)

                    // Assert
                    .ContinueWith(task => {

                        Assert.Equal(TaskStatus.RanToCompletion, task.Status);
                        Assert.NotNull(task.Result.HttpError);
                    });
            }

            [Fact]
            public Task GetHttpApiResponseAsync_For_HttpResponseMessage_Task_Should_Throw_While_Deserializing_To_HttpApiError_For_400_With_No_Proper_Formatter() {

                // Arrange
                HttpResponseMessage response = GetDummy400XmlResponse();
                IEnumerable<MediaTypeFormatter> formatters = new List<MediaTypeFormatter> { new JsonMediaTypeFormatter() };
                Task<HttpResponseMessage> responseTask = RunDelayed(40, () => response);

                // Act
                return responseTask.GetHttpApiResponseAsync(formatters)

                    // Assert
                    .ContinueWith(task => {

                        Assert.Equal(TaskStatus.Faulted, task.Status);
                        Assert.IsType<InvalidOperationException>(task.Exception.GetBaseException());
                    });
            }

            [Fact]
            public Task GetHttpApiResponseAsync_For_HttpResponseMessage_Task_Should_Return_The_Wraped_Direct_Response_For_Any_NonSuccess_Other_Than_400() {

                // Arrange
                HttpStatusCode httpStatusCode = HttpStatusCode.InternalServerError;
                HttpResponseMessage response = GetDummyResponse(HttpMethod.Post, new ByteArrayContent(new byte[0]), httpStatusCode);
                IEnumerable<MediaTypeFormatter> formatters = new MediaTypeFormatterCollection();
                Task<HttpResponseMessage> responseTask = RunDelayed(40, () => response);

                // Act
                return responseTask.GetHttpApiResponseAsync(formatters)

                    // Assert
                    .ContinueWith(task => {

                        Assert.Equal(TaskStatus.RanToCompletion, task.Status);
                        Assert.NotNull(task.Result.Response);
                        Assert.Equal(httpStatusCode, task.Result.Response.StatusCode);
                    });
            }

            [Fact]
            public Task GetHttpApiResponseAsync_For_HttpResponseMessage_Task_Should_Propagate_Back_The_Exception_From_ResponseTask() { 

                // Arrange
                IEnumerable<MediaTypeFormatter> formatters = new MediaTypeFormatterCollection();
                Task<HttpResponseMessage> responseTask = RunDelayed<HttpResponseMessage>(40, () => {

                    // This will throw DivideByZeroException
                    int left = 1, right = 0;
                    int result = left / right;
                    return default(HttpResponseMessage);
                });

                // Act
                return responseTask.GetHttpApiResponseAsync(formatters)

                    // Assert
                    .ContinueWith(task => {

                        Assert.Equal(TaskStatus.Faulted, task.Status);
                        Assert.IsType<DivideByZeroException>(task.Exception.GetBaseException());
                    });
            }
        }

        public class GetHttpApiResponseAsync_For_HttpResponseMessage_Of_TEntity_Task {

            [DataContract(Namespace = "", Name = "Car")]
            private class Car {

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

            [Fact]
            public Task GetHttpApiResponseAsync_For_HttpResponseMessage_Of_TEntity_Task_Should_Deserialize_To_TEntity_For_Success_Json() {

                // Arrange
                HttpResponseMessage response = GetDummy200JsonResponse();
                IEnumerable<MediaTypeFormatter> formatters = new MediaTypeFormatterCollection();
                Task<HttpResponseMessage> responseTask = RunDelayed(40, () => response);

                // Act
                return responseTask.GetHttpApiResponseAsync<IEnumerable<Car>>(formatters)

                    // Assert
                    .ContinueWith(task => {

                        Assert.Equal(TaskStatus.RanToCompletion, task.Status);
                        Assert.NotNull(task.Result.Model);
                    });
            }

            [Fact]
            public Task GetHttpApiResponseAsync_For_HttpResponseMessage_Of_TEntity_Task_Should_Deserialize_To_TEntity_For_Success_Xml() {

                // Arrange
                HttpResponseMessage response = GetDummy200XmlResponse();
                IEnumerable<MediaTypeFormatter> formatters = new MediaTypeFormatterCollection();
                Task<HttpResponseMessage> responseTask = RunDelayed(40, () => response);
                
                // Act
                return responseTask.GetHttpApiResponseAsync<List<Car>>(formatters)

                    // Assert
                    .ContinueWith(task => {

                        Assert.Equal(TaskStatus.RanToCompletion, task.Status);
                        Assert.NotNull(task.Result.Model);
                    });
            }

            [Fact]
            public Task GetHttpApiResponseAsync_For_HttpResponseMessage_Of_TEntity_Task_Should_Deserialize_To_HttpApiError_For_400_Json() {

                // Arrange
                HttpResponseMessage response = GetDummy400JsonResponse();
                IEnumerable<MediaTypeFormatter> formatters = new MediaTypeFormatterCollection();
                Task<HttpResponseMessage> responseTask = RunDelayed(40, () => response);

                // Act
                return responseTask.GetHttpApiResponseAsync<IEnumerable<Car>>(formatters)

                    // Assert
                    .ContinueWith(task => {

                        Assert.Equal(TaskStatus.RanToCompletion, task.Status);
                        Assert.Null(task.Result.Model);
                        Assert.NotNull(task.Result.HttpError);
                    });
            }

            [Fact]
            public Task GetHttpApiResponseAsync_For_HttpResponseMessage_Of_TEntity_Task_Should_Deserialize_To_HttpApiError_For_400_Xml() {

                // Arrange
                HttpResponseMessage response = GetDummy400XmlResponse();
                IEnumerable<MediaTypeFormatter> formatters = new MediaTypeFormatterCollection();
                Task<HttpResponseMessage> responseTask = RunDelayed(40, () => response);

                // Act
                return responseTask.GetHttpApiResponseAsync<List<Car>>(formatters)

                    // Assert
                    .ContinueWith(task => {

                        Assert.Equal(TaskStatus.RanToCompletion, task.Status);
                        Assert.NotNull(task.Result.HttpError);
                    });
            }

            [Fact]
            public Task GetHttpApiResponseAsync_For_HttpResponseMessage_Of_TEntity_Task_Should_Throw_While_Deserializing_To_TEntity_For_Success_With_No_Proper_Formatter() {

                // Arrange
                HttpResponseMessage response = GetDummy200XmlResponse();
                IEnumerable<MediaTypeFormatter> formatters = new List<MediaTypeFormatter> { new JsonMediaTypeFormatter() };
                Task<HttpResponseMessage> responseTask = RunDelayed(40, () => response);

                // Act
                return responseTask.GetHttpApiResponseAsync<List<Car>>(formatters)

                    // Assert
                    .ContinueWith(task => {

                        Assert.Equal(TaskStatus.Faulted, task.Status);
                        Assert.IsType<InvalidOperationException>(task.Exception.GetBaseException());
                    });
            }

            [Fact]
            public Task GetHttpApiResponseAsync_For_HttpResponseMessage_Of_TEntity_Task_Should_Return_The_Wraped_Direct_Response_For_Any_NonSuccess_Other_Than_400() {

                // Arrange
                HttpStatusCode httpStatusCode = HttpStatusCode.InternalServerError;
                HttpResponseMessage response = GetDummyResponse(HttpMethod.Post, new ByteArrayContent(new byte[0]), httpStatusCode);
                IEnumerable<MediaTypeFormatter> formatters = new MediaTypeFormatterCollection();
                Task<HttpResponseMessage> responseTask = RunDelayed(40, () => response);

                // Act
                return responseTask.GetHttpApiResponseAsync<List<Car>>(formatters)

                    // Assert
                    .ContinueWith(task => {

                        Assert.Equal(TaskStatus.RanToCompletion, task.Status);
                        Assert.NotNull(task.Result.Response);
                        Assert.Equal(httpStatusCode, task.Result.Response.StatusCode);
                    });
            }

            [Fact]
            public Task GetHttpApiResponseAsync_For_HttpResponseMessage_Of_TEntity_Task_Should_Propagate_Back_The_Exception_From_ResponseTask() {

                // Arrange
                IEnumerable<MediaTypeFormatter> formatters = new MediaTypeFormatterCollection();
                Task<HttpResponseMessage> responseTask = RunDelayed<HttpResponseMessage>(40, () => {

                    // This will throw DivideByZeroException
                    int left = 1, right = 0;
                    int result = left / right;
                    return default(HttpResponseMessage);
                });

                // Act
                return responseTask.GetHttpApiResponseAsync<IEnumerable<Car>>(formatters)

                    // Assert
                    .ContinueWith(task => {

                        Assert.Equal(TaskStatus.Faulted, task.Status);
                        Assert.IsType<DivideByZeroException>(task.Exception.GetBaseException());
                    });
            }
        }

        public class GetHttpApiResponseAsync_For_HttpResponseMessage {

            [Fact]
            public Task GetHttpApiResponseAsync_For_HttpResponseMessage_Should_Directly_Return_The_Response_For_Success() {

                // Arrange
                HttpStatusCode statusCode = HttpStatusCode.NoContent;
                HttpResponseMessage response = GetDummyResponse(HttpMethod.Delete, new ByteArrayContent(new byte[0]), statusCode);
                IEnumerable<MediaTypeFormatter> formatters = new MediaTypeFormatterCollection();

                // Act
                return response.GetHttpApiResponseAsync(formatters)

                    // Assert
                    .ContinueWith(task => {

                        Assert.Equal(TaskStatus.RanToCompletion, task.Status);
                        Assert.NotNull(task.Result.Response);
                        Assert.Equal(statusCode, task.Result.Response.StatusCode);
                    });
            }

            [Fact]
            public Task GetHttpApiResponseAsync_For_HttpResponseMessage_Should_Deserialize_To_HttpApiError_For_400_Json() {

                // Arrange
                HttpResponseMessage response = GetDummy400JsonResponse();
                IEnumerable<MediaTypeFormatter> formatters = new MediaTypeFormatterCollection();

                // Act
                return response.GetHttpApiResponseAsync(formatters)

                    // Assert
                    .ContinueWith(task => {

                        Assert.Equal(TaskStatus.RanToCompletion, task.Status);
                        Assert.NotNull(task.Result.HttpError);
                    });
            }

            [Fact]
            public Task GetHttpApiResponseAsync_For_HttpResponseMessage_Should_Deserialize_To_HttpApiError_For_400_Xml() {

                // Arrange
                HttpResponseMessage response = GetDummy400XmlResponse();
                IEnumerable<MediaTypeFormatter> formatters = new MediaTypeFormatterCollection();

                // Act
                return response.GetHttpApiResponseAsync(formatters)

                    // Assert
                    .ContinueWith(task => {

                        Assert.Equal(TaskStatus.RanToCompletion, task.Status);
                        Assert.NotNull(task.Result.HttpError);
                    });
            }

            [Fact]
            public Task GetHttpApiResponseAsync_For_HttpResponseMessage_Should_Throw_While_Deserializing_To_HttpApiError_For_400_With_No_Proper_Formatter() {

                // Arrange
                HttpResponseMessage response = GetDummy400XmlResponse();
                IEnumerable<MediaTypeFormatter> formatters = new List<MediaTypeFormatter> { new JsonMediaTypeFormatter() };

                // Act
                return response.GetHttpApiResponseAsync(formatters)

                    // Assert
                    .ContinueWith(task => {

                        Assert.Equal(TaskStatus.Faulted, task.Status);
                        Assert.IsType<InvalidOperationException>(task.Exception.GetBaseException());
                    });
            }

            [Fact]
            public Task GetHttpApiResponseAsync_For_HttpResponseMessage_Should_Return_The_Wraped_Direct_Response_For_Any_NonSuccess_Other_Than_400() {

                // Arrange
                HttpStatusCode httpStatusCode = HttpStatusCode.InternalServerError;
                HttpResponseMessage response = GetDummyResponse(HttpMethod.Post, new ByteArrayContent(new byte[0]), httpStatusCode);
                IEnumerable<MediaTypeFormatter> formatters = new MediaTypeFormatterCollection();

                // Act
                return response.GetHttpApiResponseAsync(formatters)

                    // Assert
                    .ContinueWith(task => {

                        Assert.Equal(TaskStatus.RanToCompletion, task.Status);
                        Assert.NotNull(task.Result.Response);
                        Assert.Equal(httpStatusCode, task.Result.Response.StatusCode);
                    });
            }
        }

        public class GetHttpApiResponseAsync_For_HttpResponseMessage_Of_TEntity {

            [DataContract(Namespace = "", Name = "Car")]
            private class Car {

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

            [Fact]
            public Task GetHttpApiResponseAsync_For_HttpResponseMessage_Of_TEntity_Should_Deserialize_To_TEntity_For_Success_Json() {

                // Arrange
                HttpResponseMessage response = GetDummy200JsonResponse();
                IEnumerable<MediaTypeFormatter> formatters = new MediaTypeFormatterCollection();

                // Act
                return response.GetHttpApiResponseAsync<IEnumerable<Car>>(formatters)

                    // Assert
                    .ContinueWith(task => {

                        Assert.Equal(TaskStatus.RanToCompletion, task.Status);
                        Assert.NotNull(task.Result.Model);
                    });
            }

            [Fact]
            public Task GetHttpApiResponseAsync_For_HttpResponseMessage_Of_TEntity_Should_Deserialize_To_TEntity_For_Success_Xml() {

                // Arrange
                HttpResponseMessage response = GetDummy200XmlResponse();
                IEnumerable<MediaTypeFormatter> formatters = new MediaTypeFormatterCollection();

                // Act
                return response.GetHttpApiResponseAsync<List<Car>>(formatters)

                    // Assert
                    .ContinueWith(task => {

                        Assert.Equal(TaskStatus.RanToCompletion, task.Status);
                        Assert.NotNull(task.Result.Model);
                    });
            }

            [Fact]
            public Task GetHttpApiResponseAsync_For_HttpResponseMessage_Of_TEntity_Should_Deserialize_To_HttpApiError_For_400_Json() {

                // Arrange
                HttpResponseMessage response = GetDummy400JsonResponse();
                IEnumerable<MediaTypeFormatter> formatters = new MediaTypeFormatterCollection();

                // Act
                return response.GetHttpApiResponseAsync<IEnumerable<Car>>(formatters)

                    // Assert
                    .ContinueWith(task => {

                        Assert.Equal(TaskStatus.RanToCompletion, task.Status);
                        Assert.Null(task.Result.Model);
                        Assert.NotNull(task.Result.HttpError);
                    });
            }

            [Fact]
            public Task GetHttpApiResponseAsync_For_HttpResponseMessage_Of_TEntity_Should_Deserialize_To_HttpApiError_For_400_Xml() {

                // Arrange
                HttpResponseMessage response = GetDummy400XmlResponse();
                IEnumerable<MediaTypeFormatter> formatters = new MediaTypeFormatterCollection();

                // Act
                return response.GetHttpApiResponseAsync<List<Car>>(formatters)

                    // Assert
                    .ContinueWith(task => {

                        Assert.Equal(TaskStatus.RanToCompletion, task.Status);
                        Assert.NotNull(task.Result.HttpError);
                    });
            }

            [Fact]
            public Task GetHttpApiResponseAsync_For_HttpResponseMessage_Of_TEntity_Should_Throw_While_Deserializing_To_TEntity_For_Success_With_No_Proper_Formatter() {

                // Arrange
                HttpResponseMessage response = GetDummy200XmlResponse();
                IEnumerable<MediaTypeFormatter> formatters = new List<MediaTypeFormatter> { new JsonMediaTypeFormatter() };

                // Act
                return response.GetHttpApiResponseAsync<List<Car>>(formatters)

                    // Assert
                    .ContinueWith(task => {

                        Assert.Equal(TaskStatus.Faulted, task.Status);
                        Assert.IsType<InvalidOperationException>(task.Exception.GetBaseException());
                    });
            }

            [Fact]
            public Task GetHttpApiResponseAsync_For_HttpResponseMessage_Of_TEntity_Should_Return_The_Wraped_Direct_Response_For_Any_NonSuccess_Other_Than_400() { 

                // Arrange
                HttpStatusCode httpStatusCode = HttpStatusCode.InternalServerError;
                HttpResponseMessage response = GetDummyResponse(HttpMethod.Post, new ByteArrayContent(new byte[0]), httpStatusCode);
                IEnumerable<MediaTypeFormatter> formatters = new MediaTypeFormatterCollection();

                // Act
                return response.GetHttpApiResponseAsync<List<Car>>(formatters)

                    // Assert
                    .ContinueWith(task => {

                        Assert.Equal(TaskStatus.RanToCompletion, task.Status);
                        Assert.NotNull(task.Result.Response);
                        Assert.Equal(httpStatusCode, task.Result.Response.StatusCode);
                    });
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

        private static Task<T> RunDelayed<T>(int millisecondsDelay, Func<T> func) {

            if (func == null) {
                throw new ArgumentNullException("func");
            }
            if (millisecondsDelay < 0) {
                throw new ArgumentOutOfRangeException("millisecondsDelay");
            }

            var taskCompletionSource = new TaskCompletionSource<T>();

            var timer = new Timer(self => {
                ((Timer)self).Dispose();
                try {
                    var result = func();
                    taskCompletionSource.SetResult(result);
                }
                catch (Exception exception) {
                    taskCompletionSource.SetException(exception);
                }
            });
            timer.Change(millisecondsDelay, millisecondsDelay);

            return taskCompletionSource.Task;
        }
    }
}