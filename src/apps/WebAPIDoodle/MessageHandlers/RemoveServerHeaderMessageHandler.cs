using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace WebAPIDoodle.MessageHandlers {

    public class RemoveServerHeaderMessageHandler : DelegatingHandler {

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken) {

            return base.SendAsync(request, cancellationToken).ContinueWith(task => {

                var response = task.Result;

                var httpContext = response.RequestMessage.Properties[Constants.HttpContextBaseKey] as HttpContextWrapper;
                if (httpContext != null)
                    httpContext.Response.Headers.Remove("Server");

                return response;
            });
        }
    }
}