using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebApiDoodle.Net.Http.Formatting {

    public static class HttpClientExtensions {

        public static Task<HttpResponseMessage> PostAsCsvAsync<T>(this HttpClient client, string requestUri, T value) {

            return client.PostAsCsvAsync(requestUri, value, CancellationToken.None);
        }

        public static Task<HttpResponseMessage> PostAsCsvAsync<T>(this HttpClient client, string requestUri, T value, CancellationToken cancellationToken) {

            return client.PostAsync(requestUri, value, new CSVMediaTypeFormatter(), cancellationToken);
        }
    }
}