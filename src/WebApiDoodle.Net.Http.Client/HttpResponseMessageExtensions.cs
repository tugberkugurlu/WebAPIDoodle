using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebApiDoodle.Net.Http.Client {

    [EditorBrowsable(EditorBrowsableState.Never)]
    internal static class HttpResponseMessageExtensions {

        internal static Task<HttpApiResponseMessage<TEntity>> GetHttpApiResponseAsync<TEntity>(this Task<HttpResponseMessage> responseTask) {

            TaskCompletionSource<HttpApiResponseMessage<TEntity>> tcs = new TaskCompletionSource<HttpApiResponseMessage<TEntity>>();
            return responseTask.Then<HttpResponseMessage, HttpApiResponseMessage<TEntity>>(response => {

                return response.GetHttpApiResponseAsync<TEntity>().Then<HttpApiResponseMessage<TEntity>, HttpApiResponseMessage<TEntity>>(apiResponse => {

                    try {

                        tcs.SetResult(apiResponse);
                    }
                    catch (Exception ex) {

                        tcs.SetException(ex);
                    }

                    return tcs.Task;

                }, runSynchronously: true, continueOnCapturedContext: false).Catch<HttpApiResponseMessage<TEntity>>(info => {

                    tcs.SetException(info.Exception);
                    return new CatchInfoBase<Task<HttpApiResponseMessage<TEntity>>>.CatchResult { Task = tcs.Task };

                }, continueOnCapturedContext: false);

            }, runSynchronously: true, continueOnCapturedContext: false).Catch<HttpApiResponseMessage<TEntity>>(info => {

                tcs.SetException(info.Exception);
                return new CatchInfoBase<Task<HttpApiResponseMessage<TEntity>>>.CatchResult { Task = tcs.Task };

            }, continueOnCapturedContext: false);
        }

        internal static Task<HttpApiResponseMessage> GetHttpApiResponseAsync(this Task<HttpResponseMessage> responseTask) {

            TaskCompletionSource<HttpApiResponseMessage> tcs = new TaskCompletionSource<HttpApiResponseMessage>();
            return responseTask.Then<HttpResponseMessage, HttpApiResponseMessage>(response => {

                return response.GetHttpApiResponseAsync().Then<HttpApiResponseMessage, HttpApiResponseMessage>(apiResponse => {

                    try {

                        tcs.SetResult(apiResponse);
                    }
                    catch (Exception ex) {

                        tcs.SetException(ex);
                    }

                    return tcs.Task;

                }, runSynchronously: true, continueOnCapturedContext: false).Catch<HttpApiResponseMessage>(info => {

                    tcs.SetException(info.Exception);
                    return new CatchInfoBase<Task<HttpApiResponseMessage>>.CatchResult { Task = tcs.Task };

                }, continueOnCapturedContext: false);

            }, runSynchronously: true, continueOnCapturedContext: false).Catch<HttpApiResponseMessage>(info => {

                tcs.SetException(info.Exception);
                return new CatchInfoBase<Task<HttpApiResponseMessage>>.CatchResult { Task = tcs.Task };

            }, continueOnCapturedContext: false);
        }

        internal static Task<HttpApiResponseMessage<TEntity>> GetHttpApiResponseAsync<TEntity>(this HttpResponseMessage response) {

            // TODO: We might not get a success status code here
            // but it might still be a reasonable response. For example, 400 response 
            // which possiblly contains a message body.
            // Structure the HttpApiResponseMessage and HttpApiResponseMessage<T> that way
            // and handle it here accordingly.

            // TODO: Test-1: For any success status codes.
            //       Test-2: For BadRequest status code.
            //       Test-3: Any other status codes.

            if (response.IsSuccessStatusCode) {

                return response.Content.ReadAsAsync<TEntity>().Then<TEntity, HttpApiResponseMessage<TEntity>>(
                    entity => response.GetHttpApiResponse(entity), runSynchronously: true, continueOnCapturedContext: false);
            }
            else if (response.StatusCode == HttpStatusCode.BadRequest) {

                return response.Content.ReadAsAsync<JToken>().Then<JToken, HttpApiResponseMessage<TEntity>>(
                    jToken => response.GetHttpApiResponse<TEntity>(jToken), runSynchronously: true, continueOnCapturedContext: false);
            }

            return TaskHelpers.FromResult(response.GetHttpApiResponse<TEntity>());
        }

        internal static Task<HttpApiResponseMessage> GetHttpApiResponseAsync(this HttpResponseMessage response) {

            // TODO: Test-1: For any success status codes.
            //       Test-2: For BadRequest status code.
            //       Test-3: Any other status codes.

            if (response.StatusCode == HttpStatusCode.BadRequest) {

                return response.Content.ReadAsAsync<JToken>().Then<JToken, HttpApiResponseMessage>(
                    jToken => response.GetHttpApiResponse(jToken), runSynchronously: true, continueOnCapturedContext: false);
            }

            return TaskHelpers.FromResult(new HttpApiResponseMessage(response));
        }

        internal static HttpApiResponseMessage<TEntity> GetHttpApiResponse<TEntity>(this HttpResponseMessage response) {

            return new HttpApiResponseMessage<TEntity>(response);
        }

        internal static HttpApiResponseMessage<TEntity> GetHttpApiResponse<TEntity>(this HttpResponseMessage response, TEntity entity) {

            return new HttpApiResponseMessage<TEntity>(response, entity);
        }

        internal static HttpApiResponseMessage<TEntity> GetHttpApiResponse<TEntity>(this HttpResponseMessage response, JToken httpError) {

            return new HttpApiResponseMessage<TEntity>(response, httpError);
        }
    }
}