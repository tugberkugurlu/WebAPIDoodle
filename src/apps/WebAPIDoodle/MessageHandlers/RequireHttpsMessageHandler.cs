using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebAPIDoodle.Messages;

namespace WebAPIDoodle.MessageHandlers {

    public class RequireHttpsMessageHandler : DelegatingHandler {

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {

            if (request.RequestUri.Scheme != Uri.UriSchemeHttps) {

                return CachedSSLRequiredResponseMessageItem.SSLRequiredForbiddenResponseMessage;
            }
   
            return base.SendAsync(request, cancellationToken);
        }

        private static class CachedSSLRequiredResponseMessageItem {

            public static Task<HttpResponseMessage> SSLRequiredForbiddenResponseMessage = GetSSLRequiredForbiddenResponseMessage();

            private static Task<HttpResponseMessage> GetSSLRequiredForbiddenResponseMessage() {

                return TaskHelpers.FromResult<HttpResponseMessage>(
                    new HttpResponseMessageWithStringContent("SSL required", HttpStatusCode.Forbidden));
            }
        }
    }
}
