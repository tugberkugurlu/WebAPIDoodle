using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebAPIDoodle.Http.Handlers {

    public abstract class ApiKeyAuthenticationHandler : DelegatingHandler {

        private readonly string _apiKeyQueryParameter;

        public ApiKeyAuthenticationHandler(string apiKeyQueryParameter) {

            if (string.IsNullOrEmpty(apiKeyQueryParameter)) {

                throw new ArgumentNullException("apiKeyQueryParameter");
            }

            _apiKeyQueryParameter = apiKeyQueryParameter;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {

            var queryStringCollection = request.RequestUri.ParseQueryString();
            if (queryStringCollection.AllKeys.Any(key => key.Equals(_apiKeyQueryParameter, StringComparison.OrdinalIgnoreCase))) {

                var apiKey = queryStringCollection[_apiKeyQueryParameter];
                IPrincipal principal;

                try {

                    //Authenticate the user now
                    principal = AuthenticateUser(apiKey, request, cancellationToken);
                }
                catch (Exception e) {

                    return TaskHelpers.FromError<HttpResponseMessage>(e);
                }

                //check if the user has been authenticated successfully
                if (principal != null) {

                    Thread.CurrentPrincipal = principal;
                    return base.SendAsync(request, cancellationToken);
                }
            }

            return TaskHelpers.FromResult(new HttpResponseMessage(HttpStatusCode.Unauthorized));
        }

        /// <summary>
        /// The method which is responsable for authenticating the user based on the provided API Key and request.
        /// </summary>
        /// <param name="apiKey"></param>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected abstract IPrincipal AuthenticateUser(string apiKey, HttpRequestMessage request, CancellationToken cancellationToken);
    }
}