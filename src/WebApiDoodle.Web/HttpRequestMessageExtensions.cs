using System;
using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Dependencies;

namespace WebApiDoodle.Web {
    
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class HttpRequestMessageExtensions {

        /// <summary>
        /// Gets the value of <typeparamref name="TService"/> through the registered 
        /// <see cref="IDependencyScope"/> instance for the <see cref="System.Net.Http.HttpRequestMessage"/> instance.
        /// </summary>
        /// <typeparam name="TService">The type of dependency</typeparam>
        /// <param name="request">The <see cref="HttpRequestMessage"/> instance</param>
        public static TService GetService<TService>(this HttpRequestMessage request) {

            if (request == null) {
                throw Error.ArgumentNull("request");
            }

            IDependencyScope dependencyScope = request.GetDependencyScope();
            TService service = (TService)dependencyScope.GetService(typeof(TService));

            return service;
        }

        /// <summary>
        /// Gets the value of <typeparamref name="T"/> associated with the specified key through the Properties dictionary 
        /// of the <see cref="HttpRequestMessage"/> instance or <c>default</c> value if either the key is 
        /// not present or the value is not of type <typeparamref name="T"/>. 
        /// </summary>
        /// <typeparam name="T">The type of the value associated with the specified key.</typeparam>
        /// <param name="request">The <see cref="HttpRequestMessage"/> instance</param>
        /// <param name="key">The key whose value to get.</param>
        /// <returns>The object of type <typeparamref name="T"/> if key was found, value is non-null, and value is of type <typeparamref name="T"/>; otherwise <c>null</c>.</returns>
        public static T GetProperty<T>(this HttpRequestMessage request, string key) {

            if (request == null) {
                throw Error.ArgumentNull("request");
            }

            T value;
            request.Properties.TryGetValue(key, out value);
            return value;
        }

        // privates and internals
        internal static HttpResponseMessage CreateErrorResponse(this HttpRequestMessage request, HttpStatusCode statusCode, string message, string messageDetail) {

            HttpError httpError = new HttpError(message);
            return request.CreateErrorResponse(statusCode, includeErrorDetail => includeErrorDetail ? httpError.AddAndReturn(HttpErrorConstants.MessageDetailKey, messageDetail) : httpError);
        }

        private static HttpResponseMessage CreateErrorResponse(this HttpRequestMessage request, HttpStatusCode statusCode, Func<bool, HttpError> errorCreator) {

            HttpConfiguration configuration = request.GetConfiguration();

            // CreateErrorResponse should never fail, even if there is no configuration associated with the request
            // In that case, use the default HttpConfiguration to con-neg the response media type
            if (configuration == null) {

                using (HttpConfiguration defaultConfig = new HttpConfiguration()) {

                    HttpError error = errorCreator(defaultConfig.ShouldIncludeErrorDetail(request));
                    return request.CreateResponse<HttpError>(statusCode, error, defaultConfig);
                }
            }
            else {

                HttpError error = errorCreator(configuration.ShouldIncludeErrorDetail(request));
                return request.CreateResponse<HttpError>(statusCode, error, configuration);
            }
        }
    }
}