using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace WebApiDoodle.Web.MessageHandlers {

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