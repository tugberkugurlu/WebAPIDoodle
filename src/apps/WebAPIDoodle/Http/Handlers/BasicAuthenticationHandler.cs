using System;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;

namespace WebAPIDoodle.Http {

    //reference: https://github.com/davidfowl/MessengR/blob/master/MessengR/Account/Login.ashx.cs
    //         : https://github.com/WebApiContrib/WebAPIContrib/blob/master/src/WebApiContrib/MessageHandlers/BasicAuthenticationHandler.cs

    /// <summary>
    /// HTTP Basic Authentication abstraction message handler
    /// </summary>
    public abstract class BasicAuthenticationHandler : DelegatingHandler {

        // HTTP 1.1 Authorization header
        private const string _httpAuthorizationHeader = "Authorization";
        // HTTP 1.1 Basic Challenge Scheme Name
        private const string _httpBasicSchemeName = "Basic";
        // HTTP 1.1 Credential username and password separator
        private const char _httpCredentialSeparator = ':';

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {

            if (request.Headers.Authorization != null && request.Headers.Authorization.Scheme == _httpBasicSchemeName) { 
                
                string username;
                string password;

                if (TryExtractBasicAuthCredentialsFromHeader(request.Headers.Authorization.Parameter, out username, out password)) {

                    IPrincipal principal;

                    try {

                        //BasicAuth credentials has been extracted.
                        //Authenticate the user now
                        principal = AuthenticateUser(request, username, password, cancellationToken);
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
            }

            var unauthorizedResponseMessage = request.CreateResponse(HttpStatusCode.Unauthorized);
            unauthorizedResponseMessage.Headers.Add("WWW-Authenticate", _httpBasicSchemeName);
            return TaskHelpers.FromResult<HttpResponseMessage>(unauthorizedResponseMessage);
        }

        /// <summary>
        /// The method which is responsable for authenticating the user based on the provided credentials.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="username">The username value extracted from BasicAuth header</param>
        /// <param name="password">The password value extracted from BasicAuth header</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        protected abstract IPrincipal AuthenticateUser(HttpRequestMessage request, string username, string password, CancellationToken cancellationToken);

        private static bool TryExtractBasicAuthCredentialsFromHeader(string authorizationHeader, out string username, out string password) {

            username = null;
            password = null;

            if (string.IsNullOrEmpty(authorizationHeader)) {

                return false;
            }

            // Decode the base 64 encoded credential payload 
            byte[] credentialBase64DecodedArray = Convert.FromBase64String(authorizationHeader);

            string decodedAuthorizationHeader = Encoding.UTF8.GetString(credentialBase64DecodedArray, 0, credentialBase64DecodedArray.Length);

            // get the username, password, and realm 
            int separatorPosition = decodedAuthorizationHeader.IndexOf(_httpCredentialSeparator);

            if (separatorPosition <= 0) {
                return false;
            }

            username = decodedAuthorizationHeader.Substring(0, separatorPosition).Trim();
            password = decodedAuthorizationHeader.Substring(separatorPosition + 1).Trim();

            if (string.IsNullOrEmpty(username)) {

                return false;
            }

            return true;
        }
    }
}