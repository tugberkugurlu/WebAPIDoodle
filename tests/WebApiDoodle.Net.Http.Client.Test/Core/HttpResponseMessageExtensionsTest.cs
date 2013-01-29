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
            public Task GetHttpApiResponseAsync_For_HttpResponseMessage_Should_Deserialize_To_HttpApiError_For_400() {

                // Arrange
                HttpResponseMessage response = GetDummy400Response();
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

        private static Task<HttpResponseMessage> GetDummy400ResponseAsync() {

            return RunDelayed(50, () => GetDummy400Response());
        }

        private static HttpResponseMessage GetDummy400Response() {

            string jsonError = "{\"Message\":\"The request is invalid.\",\"ModelState\":{\"Page\":[\"The Page field value must be minimum 1.\"],\"Take\":[\"The Take field value must be minimum 1.\"]}}";
            HttpContent content = new StringContent(jsonError, Encoding.UTF8, "application/json");
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