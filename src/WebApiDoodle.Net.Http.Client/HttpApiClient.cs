using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;

namespace WebApiDoodle.Net.Http.Client {

    /// <summary>
    /// Base generic HttpClient class for the more specified clients.
    /// </summary>
    /// <typeparam name="TResult">Type of the result type which is expected.</typeparam>
    /// <typeparam name="TId">Type of the id parameter for this client.</typeparam>
    public abstract class HttpApiClient<TResult, TId> {

        private readonly HttpClient _httpClient;
        private readonly string _baseUri;

        public HttpApiClient(HttpClient httpClient) {

            if (httpClient == null) {
                throw new ArgumentNullException("httpClient");
            }

            _httpClient = httpClient;
            _baseUri = httpClient.BaseAddress.ToString().TrimEnd('/').ToLowerInvariant();
        }
    }
}