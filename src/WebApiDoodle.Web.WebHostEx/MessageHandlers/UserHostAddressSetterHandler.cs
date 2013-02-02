using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace WebApiDoodle.Web.WebHostEx.MessageHandlers {

    /// <summary>
    /// A delegating handler which sets the user's IP Address inside the Properties dictionaty of the
    /// current <see cref="HttpRequestMessage" /> instance.
    /// </summary>
    public class UserHostAddressSetterHandler : DelegatingHandler {

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {

            request.Properties[HttpCommonPropertyKeys.UserHostAddressKey] = request.GetUserHostAddress();
            return base.SendAsync(request, cancellationToken);
        }
    }
}