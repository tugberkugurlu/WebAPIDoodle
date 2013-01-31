using System;
using System.Net.Http;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using WebApiDoodle.Web.Http;

namespace WebApiDoodle.Net.Http.Client.Sample45.MessageHandlers {
    
    public class AuthMessageHandler : BasicAuthenticationHandler {

        protected override Task<IPrincipal> AuthenticateUserAsync(
            HttpRequestMessage request, string username, string password, CancellationToken cancellationToken) {

            if (username.Equals(password, StringComparison.InvariantCultureIgnoreCase)) { 

                IPrincipal principal = new GenericPrincipal(new GenericIdentity(username), null);
                return Task.FromResult(principal);
            }

            return Task.FromResult<IPrincipal>(null);
        }
    }
}