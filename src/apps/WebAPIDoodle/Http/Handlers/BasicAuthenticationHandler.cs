using System;
using System.Net;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Http;

namespace WebApiDoodle.Web.Http {

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

        /// <summary>
        /// Indicates whether the authenticated should be suppressed if 
        /// the request is already authenticated
        /// </summary>
        public bool SuppressIfAlreadyAuthenticated { get; private set; }

        /// <summary>
        /// Parameterless constructor. Sets the SuppressIfAlreadyAuthenticated to false.
        /// </summary>
        public BasicAuthenticationHandler() : this(false) { }

        /// <summary>
        /// Constructor to supply the SuppressIfAlreadyAuthenticated value.
        /// </summary>
        /// <param name="suppressIfAlreadyAuthenticated">Indicates whether the authentication should be suppressed if the request is already authenticated.</param>
        public BasicAuthenticationHandler(bool suppressIfAlreadyAuthenticated) {

            SuppressIfAlreadyAuthenticated = suppressIfAlreadyAuthenticated;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {

            if (!Thread.CurrentPrincipal.Identity.IsAuthenticated || !SuppressIfAlreadyAuthenticated) {

                if (request.Headers.Authorization != null && request.Headers.Authorization.Scheme == _httpBasicSchemeName) {

                    string username, password;
                    if (TryExtractBasicAuthCredentialsFromHeader(request.Headers.Authorization.Parameter, out username, out password)) {

                        return AuthenticateUserAsync(request, username, password, cancellationToken).Then<IPrincipal, HttpResponseMessage>(principal => {

                            // Check if the user has been authenticated successfully
                            if (principal != null) {

                                Thread.CurrentPrincipal = principal;
                                return base.SendAsync(request, cancellationToken);
                            }

                            // Request is not authanticated. Handle unauthenticated request.
                            return HandleUnauthenticatedRequestImpl(request, cancellationToken);
                        }, runSynchronously: true);
                    }
                }

                // Request is not authanticated. Handle unauthenticated request.
                return HandleUnauthenticatedRequestImpl(request, cancellationToken);
            }
            else {

                // The request is already authenticated and it is requested 
                // to suppress the authentication in this case
                return base.SendAsync(request, cancellationToken);
            }
        }

        /// <summary>
        /// The method which is responsable for authenticating the user based on the provided credentials.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="username">The username value extracted from BasicAuth header.</param>
        /// <param name="password">The password value extracted from BasicAuth header.</param>
        /// <param name="cancellationToken">A cancellation token to cancel operation.</param>
        /// <returns></returns>
        protected abstract Task<IPrincipal> AuthenticateUserAsync(HttpRequestMessage request, string username, string password, CancellationToken cancellationToken);

        /// <summary>
        /// Called when the request is unauthenticated.
        /// </summary>
        /// <param name="context">Context object which carries the HTTP request message to send to the server and the empty HTTP response property.</param>
        protected virtual void HandleUnauthenticatedRequest(UnauthenticatedRequestContext context) {

            HttpResponseMessage unauthorizedResponseMessage = context.Request.CreateResponse(HttpStatusCode.Unauthorized);
            unauthorizedResponseMessage.Headers.Add("WWW-Authenticate", _httpBasicSchemeName);
            context.Response = unauthorizedResponseMessage;
        }

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

        private void EnsureRequestMessageExistence(HttpResponseMessage response, HttpRequestMessage request) {

            if (response.RequestMessage == null) {

                response.RequestMessage = request;
            }
        }

        private Task<HttpResponseMessage> HandleUnauthenticatedRequestImpl(HttpRequestMessage request, CancellationToken cancellationToken) {

            try {

                var unauthanticatedRequestContext = new UnauthenticatedRequestContext(request);
                HandleUnauthenticatedRequest(unauthanticatedRequestContext);

                if (unauthanticatedRequestContext.Response != null) {

                    EnsureRequestMessageExistence(unauthanticatedRequestContext.Response, request);
                    return TaskHelpers.FromResult<HttpResponseMessage>(unauthanticatedRequestContext.Response);
                }

                return base.SendAsync(request, cancellationToken);
            }
            catch (Exception e) {

                return TaskHelpers.FromError<HttpResponseMessage>(e);
            }
        }
    }
}