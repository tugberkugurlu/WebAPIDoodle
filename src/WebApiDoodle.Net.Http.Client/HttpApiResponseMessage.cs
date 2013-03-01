using System;
using System.Collections.Generic;
using System.Net.Http;

namespace WebApiDoodle.Net.Http.Client {
    
    public class HttpApiResponseMessage : IDisposable {

        internal const string ModelStateKey = "ModelState";

        public HttpApiResponseMessage(HttpResponseMessage response, HttpApiError httpError)
            : this(response) {

            if (httpError == null) {

                throw new ArgumentNullException("httpError");
            }

            HttpError = httpError;
        }

        public HttpApiResponseMessage(HttpResponseMessage response) {

            if (response == null) {

                throw new ArgumentNullException("response");
            }

            Response = response;
        }

        /// <summary>
        /// Represents the HttpResponseMessage for the request.
        /// </summary>
        public HttpResponseMessage Response { get; private set; }

        /// <summary>
        /// Determines if the response is a success or not.
        /// </summary>
        public bool IsSuccess {
            get {
                return Response.IsSuccessStatusCode;
            }
        }

        /// <summary>
        /// Represents the HTTP error message retrieved from the server if the response has "400 Bad Request" status code.
        /// </summary>
        public HttpApiError HttpError { get; private set; }

        public void Dispose() {

            if (Response != null) {

                Response.Dispose();
            }
        }
    }
}