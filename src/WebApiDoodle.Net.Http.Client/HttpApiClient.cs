using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;
using WebApiDoodle.Net.Http.Client.Internal;
using WebApiDoodle.Net.Http.Client.Model;

namespace WebApiDoodle.Net.Http.Client {

    /// <summary>
    /// Base generic HttpClient class for the more specified clients.
    /// </summary>
    /// <typeparam name="TResult">Type of the result type which is expected.</typeparam>
    /// <typeparam name="TId">Type of the id parameter for this client.</typeparam>
    public abstract class HttpApiClient<TResult, TId> where TResult : IDto {

        // TODO: Make it possible to inject _writerMediaTypeFormatter

        private readonly HttpClient _httpClient;
        private readonly string _baseUri;
        private readonly IEnumerable<MediaTypeFormatter> _formatters;
        private readonly MediaTypeFormatter _writerMediaTypeFormatter = new JsonMediaTypeFormatter();

        /// <summary>
        /// Initializes a new instance of the WebApiDoodle.Net.Http.Client.HttpApiClient
        /// with a specific System.Net.Http.HttpClient.
        /// </summary>
        /// <remarks>
        /// The specified System.Net.Http.HttpClient is not going to be disposed by the 
        /// WebApiDoodle.Net.Http.Client.HttpApiClient instance as a System.Net.Http.HttpClient 
        /// instance can be used throughout the application lifecycle.
        /// </remarks>
        /// <param name="httpClient">The <see cref="System.Net.Http.HttpClient"/> instance to use for handling requests.</param>
        public HttpApiClient(HttpClient httpClient) 
            : this(httpClient, new MediaTypeFormatterCollection()) {
        }

        /// <summary>
        /// Initializes a new instance of the WebApiDoodle.Net.Http.Client.HttpApiClient
        /// with a specific System.Net.Http.HttpClient.
        /// </summary>
        /// <remarks>
        /// The specified System.Net.Http.HttpClient is not going to be disposed by the 
        /// WebApiDoodle.Net.Http.Client.HttpApiClient instance as a System.Net.Http.HttpClient 
        /// instance can be used throughout the application lifecycle.
        /// </remarks>
        /// <param name="httpClient">The <see cref="System.Net.Http.HttpClient"/> instance to use for handling requests.</param>
        /// <param name="formatters">The collection of <see cref="System.Net.Http.MediaTypeFormatter"/> instances to use.</param>
        public HttpApiClient(HttpClient httpClient, IEnumerable<MediaTypeFormatter> formatters) {

            if (httpClient == null) {
                throw new ArgumentNullException("httpClient");
            }

            if (formatters == null) {
                throw new ArgumentNullException("formatters");
            }

            _httpClient = httpClient;
            _formatters = formatters;
            _baseUri = httpClient.BaseAddress.ToString().TrimEnd('/').ToLowerInvariant();
        }

        // GET Requests (Collection)

        public Task<HttpApiResponseMessage<PaginatedDto<TResult>>> GetAsync(string uriTemplate) {

            return GetAsync(uriTemplate, CancellationToken.None);
        }

        public Task<HttpApiResponseMessage<PaginatedDto<TResult>>> GetAsync(string uriTemplate, CancellationToken cancellationToken) {

            return GetAsync(uriTemplate, null, cancellationToken);
        }

        public Task<HttpApiResponseMessage<PaginatedDto<TResult>>> GetAsync(string uriTemplate, object uriParameters) {

            return GetAsync(uriTemplate, uriParameters, CancellationToken.None);
        }

        public Task<HttpApiResponseMessage<PaginatedDto<TResult>>> GetAsync(string uriTemplate, object uriParameters, CancellationToken cancellationToken) {

            string requestUri = UriUtil.BuildRequestUri<TId>(_baseUri, uriTemplate, uriParameters: uriParameters);
            return _httpClient.GetAsync(requestUri, cancellationToken).GetHttpApiResponseAsync<PaginatedDto<TResult>>(_formatters);
        }

        // GET Requests (Single)

        public Task<HttpApiResponseMessage<TResult>> GetSingleAsync(string uriTemplate, TId id) {

            return GetSingleAsync(uriTemplate, id, CancellationToken.None);
        }

        public Task<HttpApiResponseMessage<TResult>> GetSingleAsync(string uriTemplate, TId id, CancellationToken cancellationToken) {

            return GetSingleAsync(uriTemplate, id, null, cancellationToken);
        }

        public Task<HttpApiResponseMessage<TResult>> GetSingleAsync(string uriTemplate, TId id, object uriParameters) {

            return GetSingleAsync(uriTemplate, id, uriParameters, CancellationToken.None);
        }

        public Task<HttpApiResponseMessage<TResult>> GetSingleAsync(string uriTemplate, TId id, object uriParameters, CancellationToken cancellationToken) {

            string requestUri = UriUtil.BuildRequestUri<TId>(_baseUri, uriTemplate, id: id);
            return _httpClient.GetAsync(requestUri, cancellationToken).GetHttpApiResponseAsync<TResult>(_formatters);
        }

        // POST Requests

        public Task<HttpApiResponseMessage<TResult>> PostAsync<TRequestModel>(string uriTemplate, TRequestModel requestModel) {

            return PostAsync(uriTemplate, requestModel, CancellationToken.None);
        }

        public Task<HttpApiResponseMessage<TResult>> PostAsync<TRequestModel>(string uriTemplate, TRequestModel requestModel, CancellationToken cancellationToken) {

            return PostAsync(uriTemplate, requestModel, null, cancellationToken);
        }

        public Task<HttpApiResponseMessage<TResult>> PostAsync<TRequestModel>(string uriTemplate, TRequestModel requestModel, object uriParameters) {

            return PostAsync(uriTemplate, requestModel, CancellationToken.None);
        }

        public Task<HttpApiResponseMessage<TResult>> PostAsync<TRequestModel>(string uriTemplate, TRequestModel requestModel, object uriParameters, CancellationToken cancellationToken) {

            string requestUri = UriUtil.BuildRequestUri<TId>(_baseUri, uriTemplate, uriParameters: uriParameters);
            return _httpClient.PostAsync<TRequestModel>(requestUri, requestModel, _writerMediaTypeFormatter, cancellationToken)
                .GetHttpApiResponseAsync<TResult>(_formatters);
        }

        // PUT Requests

        public Task<HttpApiResponseMessage<TResult>> PutAsync<TRequestModel>(string uriTemplate, TId id, TRequestModel requestModel) {

            return PutAsync(uriTemplate, id, requestModel, CancellationToken.None);
        }

        public Task<HttpApiResponseMessage<TResult>> PutAsync<TRequestModel>(string uriTemplate, TId id, TRequestModel requestModel, CancellationToken cancellationToken) {

            return PutAsync(uriTemplate, id, requestModel, null, cancellationToken);
        }

        public Task<HttpApiResponseMessage<TResult>> PutAsync<TRequestModel>(string uriTemplate, TId id, TRequestModel requestModel, object uriParameters) {

            return PutAsync(uriTemplate, id, requestModel, uriParameters, CancellationToken.None);
        }

        public Task<HttpApiResponseMessage<TResult>> PutAsync<TRequestModel>(string uriTemplate, TId id, TRequestModel requestModel, object uriParameters, CancellationToken cancellationToken) {

            string requestUri = UriUtil.BuildRequestUri<TId>(_baseUri, uriTemplate, id: id, uriParameters: uriParameters);
            return _httpClient.PutAsync<TRequestModel>(requestUri, requestModel, _writerMediaTypeFormatter, cancellationToken)
                              .GetHttpApiResponseAsync<TResult>(_formatters);
        }

        // DELETE Requests

        public Task<HttpApiResponseMessage> DeleteAsync(string uriTemplate, TId id) {

            return DeleteAsync(uriTemplate, id, CancellationToken.None);
        }

        public Task<HttpApiResponseMessage> DeleteAsync(string uriTemplate, TId id, CancellationToken cancellationToken) {

            return DeleteAsync(uriTemplate, id, null, cancellationToken);
        }

        public Task<HttpApiResponseMessage> DeleteAsync(string uriTemplate, TId id, object uriParameters) {

            return DeleteAsync(uriTemplate, id, uriParameters, CancellationToken.None);
        }

        public Task<HttpApiResponseMessage> DeleteAsync(string uriTemplate, TId id, object uriParameters, CancellationToken cancellationToken) {

            string requestUri = UriUtil.BuildRequestUri<TId>(_baseUri, uriTemplate, id: id, uriParameters: uriParameters);
            return _httpClient.DeleteAsync(requestUri, cancellationToken).GetHttpApiResponseAsync(_formatters);
        }
    }
}