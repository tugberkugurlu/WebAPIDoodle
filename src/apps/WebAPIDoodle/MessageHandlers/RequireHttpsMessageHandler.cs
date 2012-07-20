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

                return TaskHelpers.FromResult<HttpResponseMessage>(
                    new HttpResponseMessageWithStringContent("SSL required", HttpStatusCode.Forbidden));
            }
   
            return base.SendAsync(request, cancellationToken);
        }
    }
}
