using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace WebApiDoodle.Web.WebHostEx.MessageHandlers {

    /// <summary>
    /// A delegating handler which removes the 'Server' header from the response.
    /// </summary>
    /// <remarks>
    /// Register this as early as possible inside the message handler pipeline because
    /// if any unhandled expection occurs during the pipiline and isn't converted into a 
    /// HttpResponseMessage, this handler won't be able to remove 'Server' header 
    /// from the response.
    /// </remarks>
    public class RemoveServerHeaderMessageHandler : DelegatingHandler {

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {

            return base.SendAsync(request, cancellationToken).Then(response => {

                // See if RequestMessage is present or not.
                // Another message handler may have set a new HttpResponseMessage and didn't set the 
                // currenct request object to response.RequestMessage.
                if (response.RequestMessage != null) {

                    var httpContext = response.RequestMessage.Properties[HttpWebHostPropertyKeys.HttpContextBaseKey] as HttpContextWrapper;
                    if (httpContext != null)
                        httpContext.Response.Headers.Remove("Server");
                }
                else {

                    // Try your lock on removing the header from actual request property.
                    var httpContext = request.Properties[HttpWebHostPropertyKeys.HttpContextBaseKey] as HttpContextWrapper;
                    if (httpContext != null)
                        httpContext.Response.Headers.Remove("Server");
                }

                return response;
            });
        }
    }
}