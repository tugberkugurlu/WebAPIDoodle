using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Http.Filters;
using System.Web.Http.Controllers;
using System.Net.Http;
using System.Web.Http;
using System.Web;
using System.Net;

namespace WebAPIDoodle.Filters {

    /// <summary>
    /// QueryString Api Key Authorization filter for ASP.NET Web API.
    /// </summary>
    public class ApiKeyAuthAttribute : AuthorizationFilterAttribute {

        private static readonly string[] _emptyArray = new string[0];
        private const string _apiKeyAuthorizerMethodName = "IsAuthorized";

        private readonly string _apiKeyQueryParameter;
        private string _roles;
        private readonly Type _apiKeyAuthorizerType;
        private string[] _rolesSplit = _emptyArray;

        /// <summary>
        /// The comma seperated list of roles which user needs to be in.
        /// </summary>
        public string Roles {

            get {

                return this._roles ?? string.Empty;

            }
            set {

                this._roles = value;
                this._rolesSplit = SplitString(value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="apiKeyQueryParameter">The name of the query string parameter whose value needs to be compared against.</param>
        /// <param name="apiKeyAuthorizerType">Type of Api Key Authorizer which implements TugberkUg.Web.Http.IApiKeyAuthorizer</param>
        public ApiKeyAuthAttribute(string apiKeyQueryParameter, Type apiKeyAuthorizerType) {

            if (string.IsNullOrEmpty(apiKeyQueryParameter))
                throw new ArgumentNullException("apiKeyQueryParameter");

            if (apiKeyAuthorizerType == null)
                throw new ArgumentNullException("apiKeyAuthorizerType");

            if (!IsTypeOfIApiKeyAuthorizer(apiKeyAuthorizerType)) {

                throw new ArgumentException(
                    string.Format(
                        "{0} type has not implemented the TugberkUg.Web.Http.IApiKeyAuthorizer interface",
                        apiKeyAuthorizerType.ToString()
                    ),
                    "apiKeyAuthorizerType"
                );
            }

            _apiKeyQueryParameter = apiKeyQueryParameter;
            _apiKeyAuthorizerType = apiKeyAuthorizerType;
        }

        public override void OnAuthorization(HttpActionContext actionContext) {

            if (actionContext == null)
                throw new ArgumentNullException("actionContext");

            if (this.SkipAuthorization(actionContext))
                return;

            if (!AuthorizeCore(actionContext.Request))
                HandleUnauthorizedRequest(actionContext);
        }

        /// <summary>
        /// Handles the operation on an unauthorized situation
        /// </summary>
        /// <param name="actionContext"></param>
        protected virtual void HandleUnauthorizedRequest(HttpActionContext actionContext) {

            if (actionContext == null) {

                throw new ArgumentNullException("actionContext");
            }
            actionContext.Response = actionContext.ControllerContext.Request.CreateResponse(HttpStatusCode.Unauthorized);
        }

        //private helpers
        private bool IsTypeOfIApiKeyAuthorizer(Type type) {

            foreach (Type interfaceType in type.GetInterfaces()) {

                if (interfaceType == typeof(IApiKeyAuthorizer))
                    return true;
            }

            return false;
        }

        private bool SkipAuthorization(HttpActionContext actionContext) {

            return actionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any<AllowAnonymousAttribute>() ||
                actionContext.ControllerContext.ControllerDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any<AllowAnonymousAttribute>();
        }

        private bool AuthorizeCore(HttpRequestMessage request) {

            var apiKey = HttpUtility.ParseQueryString(request.RequestUri.Query)[_apiKeyQueryParameter];

            return IsAuthorized(apiKey);
        }

        private bool IsAuthorized(string apiKey) {

            object apiKeyAuthorizerClassInstance = Activator.CreateInstance(_apiKeyAuthorizerType);
            object result = null;

            if (_rolesSplit == _emptyArray) {

                result = _apiKeyAuthorizerType.GetMethod(_apiKeyAuthorizerMethodName, new Type[] { typeof(string) }).
                    Invoke(apiKeyAuthorizerClassInstance, new object[] { apiKey });

            }
            else {

                result = _apiKeyAuthorizerType.GetMethod(_apiKeyAuthorizerMethodName, new Type[] { typeof(string), typeof(string[]) }).
                    Invoke(apiKeyAuthorizerClassInstance, new object[] { apiKey, _rolesSplit });
            }

            return (bool)result;
        }

        private static string[] SplitString(string original) {

            if (string.IsNullOrEmpty(original))
                return _emptyArray;

            IEnumerable<string> source =
                from piece in original.Split(new char[] {

					','
				})
                let trimmed = piece.Trim()
                where !string.IsNullOrEmpty(trimmed)
                select trimmed;
            return source.ToArray<string>();
        }
    }
}