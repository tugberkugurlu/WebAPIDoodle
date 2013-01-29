using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace WebApiDoodle.Net.Http.Client.Test.Core {
    
    public class HttpResponseMessageExtensionsTest {

        public class GetHttpApiResponseAsync_For_HttpResponseMessage {

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
                        Assert.NotNull(task.Result);
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
                        Assert.NotNull(task.Result);
                        Assert.NotNull(task.Result.HttpError);
                    });
            }
        }

        private static Task<HttpResponseMessage> GetDummy400JsonResponseAsync() {

            return RunDelayed(50, () => GetDummy400JsonResponse());
        }

        private static Task<HttpResponseMessage> GetDummy400XmlResponseAsync() {

            return RunDelayed(50, () => GetDummy400XmlResponse());
        }

        private static HttpResponseMessage GetDummy400JsonResponse() {

            string jsonError = "{\"Message\":\"The request is invalid.\",\"ModelState\":{\"Page\":[\"The Page field value must be minimum 1.\"],\"Take\":[\"The Take field value must be minimum 1.\"]}}";
            HttpContent content = new StringContent(jsonError, Encoding.UTF8, "application/json");

            return GetDummy400Response(content);
        }

        private static HttpResponseMessage GetDummy400XmlResponse() { 

            string xmlError = "<Error><Message>The request is invalid.</Message><ModelState><car.Make>The field Make must be a string with a maximum length of 20.</car.Make><car.Model>The Model field is required.</car.Model></ModelState></Error>";
            HttpContent content = new StringContent(xmlError, Encoding.UTF8, "application/xml");

            return GetDummy400Response(content);
        }

        private static HttpResponseMessage GetDummy400Response(HttpContent content) {

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "https://localhost/api/cars");
            HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.BadRequest) {
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