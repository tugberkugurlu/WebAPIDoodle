using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebAPIDoodle.Test {

    internal static class TestHelper {

        internal static Task<HttpResponseMessage> InvokeMessageHandler(HttpRequestMessage request, DelegatingHandler handler, CancellationToken cancellationToken = default(CancellationToken)) {

            handler.InnerHandler = new DummyHandler();
            var invoker = new HttpMessageInvoker(handler);
            return invoker.SendAsync(request, cancellationToken);
        }

        private class DummyHandler : DelegatingHandler {

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {

                var response = new HttpResponseMessage(HttpStatusCode.OK);
                return TaskHelpers.FromResult(response);
            }
        }

        //private static Task<HttpResponseMessage> InvokeImpl(HttpRequestMessage request, DelegatingHandler handler, CancellationToken cancellationToken = default(CancellationToken), TaskStatus taskStatus = TaskStatus.RanToCompletion) {

        //    var invoker = new InvokerHandler();
        //    return invoker.Invoke(request, handler, cancellationToken, taskStatus);
        //}

        //private class InvokerHandler : DelegatingHandler {

        //    public Task<HttpResponseMessage> Invoke(HttpRequestMessage request, DelegatingHandler handler, CancellationToken cancellationToken, TaskStatus taskStatus) {

        //        handler.InnerHandler = new DummyHandler(taskStatus);
        //        InnerHandler = handler;
        //        return SendAsync(request, cancellationToken);
        //    }

        //    private class DummyHandler : DelegatingHandler {

        //        TaskStatus _taskStatus;

        //        public DummyHandler(TaskStatus taskStatus) {
        //            _taskStatus = taskStatus;
        //        }

        //        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {

        //            var response = new HttpResponseMessage(HttpStatusCode.OK);

        //            switch (_taskStatus) {

        //                case TaskStatus.Canceled:
        //                    return TaskHelpers.Canceled<HttpResponseMessage>();

        //                case TaskStatus.Faulted:
        //                    return TaskHelpers.FromError<HttpResponseMessage>(new NotImplementedException());

        //                case TaskStatus.RanToCompletion:
        //                    return TaskHelpers.FromResult(response);

        //                default:
        //                    return TaskHelpers.FromResult(response);
        //            }
        //        }
        //    }
        //}
    }
}