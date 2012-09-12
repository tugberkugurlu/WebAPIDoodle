using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebAPIDoodle.Http {

    public class RequireHttpsMessageHandler : DelegatingHandler {

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {

            if (request.RequestUri.Scheme != Uri.UriSchemeHttps) {

                var forbiddenResponse = request.CreateResponse(HttpStatusCode.Forbidden);
                forbiddenResponse.ReasonPhrase = "SSL Required";
                return TaskHelpers.FromResult<HttpResponseMessage>(forbiddenResponse);
            }
   
            return base.SendAsync(request, cancellationToken);
        }
    }
}
