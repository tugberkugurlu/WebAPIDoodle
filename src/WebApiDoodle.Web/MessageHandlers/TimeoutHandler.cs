using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace WebApiDoodle.Web.MessageHandlers {

    public class TimeoutHandler : DelegatingHandler {

        private readonly int _milliseconds;
        private static readonly TimerCallback s_timerCallback = new TimerCallback(TimerCallbackLogic);

        public TimeoutHandler(int milliseconds) {

            if (milliseconds < -1) {

                throw new ArgumentOutOfRangeException("milliseconds");
            }

            _milliseconds = milliseconds;
        }

        public int Timeout {

            get {
                return _milliseconds;
            }
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {

            var cts = new CancellationTokenSource();
            var linkedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cts.Token, cancellationToken);
            var linkedToken = linkedTokenSource.Token;
            var timer = new Timer(s_timerCallback, cts, -1, -1);

            request.RegisterForDispose(timer);
            request.RegisterForDispose(cts);
            request.RegisterForDispose(linkedTokenSource);

            timer.Change(_milliseconds, -1);

            return base.SendAsync(request, linkedToken).ContinueWith(task => 
                request.CreateResponse(HttpStatusCode.RequestTimeout), TaskContinuationOptions.ExecuteSynchronously | TaskContinuationOptions.OnlyOnCanceled);
        }

        private static void TimerCallbackLogic(object obj) {

            CancellationTokenSource cancellationTokenSource = (CancellationTokenSource)obj;
            cancellationTokenSource.Cancel();
        }
    }
}