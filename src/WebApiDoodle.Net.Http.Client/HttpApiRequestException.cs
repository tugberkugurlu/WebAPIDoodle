using System;
using System.Net;
using System.Net.Http;

namespace WebApiDoodle.Net.Http.Client {
 
    [Serializable]
    public class HttpApiRequestException : HttpRequestException {

        public HttpApiRequestException(
            string message,
            HttpStatusCode httpStatusCode) : base(message) {

            StatusCode = httpStatusCode;
        }

        public HttpApiRequestException(
            string message,
            HttpStatusCode httpStatusCode, 
            HttpApiError httpError) : base(message) {

            StatusCode = httpStatusCode;
            HttpError = httpError;
        }

        public HttpStatusCode StatusCode { get; private set; }
        public HttpApiError HttpError { get; private set; }
    }
}