using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace System.Net.Http {
    
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class HttpRequestMessageExtensions {

        public static Task<HttpResponseMessage> InvokeServerAsync(this HttpRequestMessage request, HttpConfiguration configuration) {

            using (var httpServer = new HttpServer(configuration))
            using (var httpClient = HttpClientFactory.Create(httpServer)) {

                return httpClient.SendAsync(request);
            }
        }

        public static Task<TResult> InvokeServerAsync<TResult>(this HttpRequestMessage request, HttpConfiguration configuration, Func<HttpResponseMessage, bool> successor) {

            TaskCompletionSource<TResult> tcs = new TaskCompletionSource<TResult>();
            return request.InvokeServerAsync(configuration).Then<HttpResponseMessage, TResult>(response => {

                // if the response is the one that is expected, get the object out of it
                if (successor(response)) {

                    response.Content.ReadAsAsync<TResult>().Then(result => {

                        tcs.SetResult(result);
                    }, runSynchronously: true).Catch(info => {

                        tcs.SetException(info.Exception);
                        return new CatchInfoBase<Task>.CatchResult { Task = tcs.Task };
                    });
                }
                else {

                    // TODO: Figure out if this is the best way here.
                    // Maybe throw a specific exception here...
                    tcs.SetResult(default(TResult));
                }

                return tcs.Task;

            }, runSynchronously: true).Catch<TResult>(info => {

                tcs.SetException(info.Exception);
                return new CatchInfoBase<Task<TResult>>.CatchResult { Task = tcs.Task };
            });
        }
    }
}