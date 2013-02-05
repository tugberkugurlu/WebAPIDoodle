using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading;
using System.Threading.Tasks;
using WebApiDoodle.Net.Http.Client.Formatting;
using WebApiDoodle.Net.Http.Client.Internal;
using WebApiDoodle.Net.Http.Client.Model;

namespace WebApiDoodle.Net.Http.Client {

    /// <summary>
    /// Generic base class for the .NET HTTP clients.
    /// </summary>
    /// <typeparam name="TResult">Type of the result type which is expected.</typeparam>
    public abstract class HttpApiClient<TResult> where TResult : IDto {

        private readonly HttpClient _httpClient;
        private readonly string _baseUri;
        private readonly IEnumerable<MediaTypeFormatter> _formatters;
        private readonly MediaTypeFormatter _writerMediaTypeFormatter;

        /// <summary>
        /// Initializes a new instance of the WebApiDoodle.Net.Http.Client.HttpApiClient
        /// with a specific System.Net.Http.HttpClient.
        /// </summary>
        /// <remarks>
        /// The specified System.Net.Http.HttpClient is not going to be disposed by the 
        /// WebApiDoodle.Net.Http.Client.HttpApiClient instance as a System.Net.Http.HttpClient 
        /// instance can be used throughout the application lifecycle.
        /// </remarks>
        /// <param name="httpClient">The <see cref="HttpClient"/> instance to use for handling requests.</param>
        public HttpApiClient(HttpClient httpClient)
            : this(httpClient, DefaultMediaTypeFormatterCollection.Instance, DefaultWriterMediaTypeFormatter.Instance) {
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
        /// <param name="httpClient">The <see cref="HttpClient"/> instance to use for handling requests.</param>
        /// <param name="formatters">The collection of <see cref="MediaTypeFormatter"/> instances to use.</param>
        public HttpApiClient(HttpClient httpClient, IEnumerable<MediaTypeFormatter> formatters)
            : this(httpClient, formatters, DefaultWriterMediaTypeFormatter.Instance) { 
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
        /// <param name="httpClient">The <see cref="HttpClient"/> instance to use for handling requests.</param>
        /// <param name="formatters">The collection of <see cref="MediaTypeFormatter"/> instances to use.</param>
        /// <param name="writerMediaTypeFormatter">The writer formatter which is of type <see cref="MediaTypeFormatter"/> will be used to serialize the request body.</param>
        public HttpApiClient(HttpClient httpClient, IEnumerable<MediaTypeFormatter> formatters, MediaTypeFormatter writerMediaTypeFormatter) {

            if (httpClient == null) {
                throw new ArgumentNullException("httpClient");
            }

            if (formatters == null) {
                throw new ArgumentNullException("formatters");
            }

            if (writerMediaTypeFormatter == null) {
                throw new ArgumentNullException("writerMediaTypeFormatter");
            }

            _httpClient = httpClient;
            _formatters = formatters;
            _writerMediaTypeFormatter = writerMediaTypeFormatter;
            _baseUri = httpClient.BaseAddress.ToString().TrimEnd('/').ToLowerInvariant();
        }

        // GET Requests (Collection)

        protected Task<HttpApiResponseMessage<PaginatedDto<TResult>>> GetAsync(string uriTemplate) {

            return GetAsync(uriTemplate, null, CancellationToken.None);
        }

        protected Task<HttpApiResponseMessage<PaginatedDto<TResult>>> GetAsync(string uriTemplate, CancellationToken cancellationToken) {

            return GetAsync(uriTemplate, null, cancellationToken);
        }

        protected Task<HttpApiResponseMessage<PaginatedDto<TResult>>> GetAsync(string uriTemplate, object uriParameters) {

            return GetAsync(uriTemplate, uriParameters, CancellationToken.None);
        }

        protected Task<HttpApiResponseMessage<PaginatedDto<TResult>>> GetAsync(string uriTemplate, object uriParameters, CancellationToken cancellationToken) {

            string requestUri = UriUtil.BuildRequestUri(_baseUri, uriTemplate, uriParameters: uriParameters);
            return _httpClient.GetAsync(requestUri, cancellationToken).GetHttpApiResponseAsync<PaginatedDto<TResult>>(_formatters);
        }

        // GET Requests (Single)

        protected Task<HttpApiResponseMessage<TResult>> GetSingleAsync(string uriTemplate) {

            return GetSingleAsync(uriTemplate, null, CancellationToken.None);
        }

        protected Task<HttpApiResponseMessage<TResult>> GetSingleAsync(string uriTemplate, CancellationToken cancellationToken) {

            return GetSingleAsync(uriTemplate, null, cancellationToken);
        }

        protected Task<HttpApiResponseMessage<TResult>> GetSingleAsync(string uriTemplate, object uriParameters) {

            return GetSingleAsync(uriTemplate, uriParameters, CancellationToken.None);
        }

        protected Task<HttpApiResponseMessage<TResult>> GetSingleAsync(string uriTemplate, object uriParameters, CancellationToken cancellationToken) {

            string requestUri = UriUtil.BuildRequestUri(_baseUri, uriTemplate, uriParameters: uriParameters);
            return _httpClient.GetAsync(requestUri, cancellationToken).GetHttpApiResponseAsync<TResult>(_formatters);
        }

        // POST Requests

        protected Task<HttpApiResponseMessage<TResult>> PostAsync<TRequestModel>(string uriTemplate, TRequestModel requestModel) {

            return PostAsync(uriTemplate, requestModel, null, CancellationToken.None);
        }

        protected Task<HttpApiResponseMessage<TResult>> PostAsync<TRequestModel>(string uriTemplate, TRequestModel requestModel, CancellationToken cancellationToken) {

            return PostAsync(uriTemplate, requestModel, null, cancellationToken);
        }

        protected Task<HttpApiResponseMessage<TResult>> PostAsync<TRequestModel>(string uriTemplate, TRequestModel requestModel, object uriParameters) {

            return PostAsync(uriTemplate, requestModel, CancellationToken.None);
        }

        protected Task<HttpApiResponseMessage<TResult>> PostAsync<TRequestModel>(string uriTemplate, TRequestModel requestModel, object uriParameters, CancellationToken cancellationToken) {

            string requestUri = UriUtil.BuildRequestUri(_baseUri, uriTemplate, uriParameters: uriParameters);
            return _httpClient.PostAsync<TRequestModel>(requestUri, requestModel, _writerMediaTypeFormatter, cancellationToken)
                .GetHttpApiResponseAsync<TResult>(_formatters);
        }

        // PUT Requests

        protected Task<HttpApiResponseMessage<TResult>> PutAsync<TRequestModel>(string uriTemplate, TRequestModel requestModel) {

            return PutAsync(uriTemplate, requestModel, null, CancellationToken.None);
        }

        protected Task<HttpApiResponseMessage<TResult>> PutAsync<TRequestModel>(string uriTemplate, TRequestModel requestModel, CancellationToken cancellationToken) {

            return PutAsync(uriTemplate, requestModel, null, cancellationToken);
        }

        protected Task<HttpApiResponseMessage<TResult>> PutAsync<TRequestModel>(string uriTemplate, TRequestModel requestModel, object uriParameters) {

            return PutAsync(uriTemplate, requestModel, uriParameters, CancellationToken.None);
        }

        protected Task<HttpApiResponseMessage<TResult>> PutAsync<TRequestModel>(string uriTemplate, TRequestModel requestModel, object uriParameters, CancellationToken cancellationToken) {

            string requestUri = UriUtil.BuildRequestUri(_baseUri, uriTemplate, uriParameters: uriParameters);
            return _httpClient.PutAsync<TRequestModel>(requestUri, requestModel, _writerMediaTypeFormatter, cancellationToken)
                              .GetHttpApiResponseAsync<TResult>(_formatters);
        }

        // DELETE Requests

        protected Task<HttpApiResponseMessage> DeleteAsync(string uriTemplate) {

            return DeleteAsync(uriTemplate, null, CancellationToken.None);
        }

        protected Task<HttpApiResponseMessage> DeleteAsync(string uriTemplate, CancellationToken cancellationToken) {

            return DeleteAsync(uriTemplate, null, cancellationToken);
        }

        protected Task<HttpApiResponseMessage> DeleteAsync(string uriTemplate, object uriParameters) {

            return DeleteAsync(uriTemplate, uriParameters, CancellationToken.None);
        }

        protected Task<HttpApiResponseMessage> DeleteAsync(string uriTemplate, object uriParameters, CancellationToken cancellationToken) {

            string requestUri = UriUtil.BuildRequestUri(_baseUri, uriTemplate, uriParameters: uriParameters);
            return _httpClient.DeleteAsync(requestUri, cancellationToken).GetHttpApiResponseAsync(_formatters);
        }

        // Generic SendAsync Methods

        protected Task<HttpApiResponseMessage> SendAsync(HttpRequestMessage request) {

            return SendAsync(request, HttpCompletionOption.ResponseContentRead, CancellationToken.None);
        }

        protected Task<HttpApiResponseMessage> SendAsync(HttpRequestMessage request, HttpCompletionOption httpCompletionOption) {

            return SendAsync(request, httpCompletionOption, CancellationToken.None);
        }

        protected Task<HttpApiResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {

            return SendAsync(request, HttpCompletionOption.ResponseContentRead, cancellationToken);
        }

        protected Task<HttpApiResponseMessage> SendAsync(HttpRequestMessage request, HttpCompletionOption httpCompletionOption, CancellationToken cancellationToken) {

            return _httpClient.SendAsync(request, httpCompletionOption, cancellationToken)
                .GetHttpApiResponseAsync(_formatters);
        }
    }
}