using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace WebAPIDoodle.Http {

    public class RemoveServerHeaderMessageHandler : DelegatingHandler {

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken) {

            return base.SendAsync(request, cancellationToken).Then(response => {

                // See if RequestMessage is present or not.
                // Another message handler may have set a new HttpResponseMessage and didn't set the 
                // Request object to response.RequestMessage
                if (response.RequestMessage != null) {

                    var httpContext = response.RequestMessage.Properties[Constants.HttpContextBaseKey] as HttpContextWrapper;
                    if (httpContext != null)
                        httpContext.Response.Headers.Remove("Server");
                }
                else {

                    // Try your lock on removing the header on actual request property
                    var httpContext = request.Properties[Constants.HttpContextBaseKey] as HttpContextWrapper;
                    if (httpContext != null)
                        httpContext.Response.Headers.Remove("Server");
                }

                return response;
            });
        }
    }
}