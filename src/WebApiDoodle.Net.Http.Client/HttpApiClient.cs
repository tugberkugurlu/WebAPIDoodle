using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WebApiDoodle.Net.Http.Client {

    /// <summary>
    /// Base generic HttpClient class for the more specified clients.
    /// </summary>
    /// <typeparam name="TResult">Type of the result type which is expected.</typeparam>
    /// <typeparam name="TId">Type of the id parameter for this client.</typeparam>
    public abstract class HttpApiClient<TResult, TId> {

        private readonly HttpClient _httpClient;
        private readonly string _baseUri;

        /// <summary>
        /// Initializes a new instance of the WebApiDoodle.Net.Http.Client.HttpApiClient
        /// with a specific System.Net.Http.HttpClient.
        /// </summary>
        /// <remarks>
        /// The specified System.Net.Http.HttpClient is not going to be disposed by the 
        /// WebApiDoodle.Net.Http.Client.HttpApiClient instance as a System.Net.Http.HttpClient 
        /// instance can be used throughout the application lifecycle.
        /// </remarks>
        /// <param name="httpClient">
        /// The System.Net.Http.HttpClient instance to use for handling requests.
        /// </param>
        public HttpApiClient(HttpClient httpClient) {

            if (httpClient == null) {
                throw new ArgumentNullException("httpClient");
            }

            _httpClient = httpClient;
            _baseUri = httpClient.BaseAddress.ToString().TrimEnd('/').ToLowerInvariant();
        }
    }
}