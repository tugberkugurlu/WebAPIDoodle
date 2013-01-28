using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace WebApiDoodle.Net.Http.Client {
    
    public class HttpApiResponseMessage {

        internal const string ModelStateKey = "ModelState";

        public HttpApiResponseMessage(HttpResponseMessage response, HttpApiError httpError)
            : this(response) {

            if (httpError == null) {

                throw new ArgumentNullException("httpError");
            }

            HttpApiError modelState = httpError[ModelStateKey] as HttpApiError;

            if (modelState != null) {

                ModelState = httpError[ModelStateKey] as Dictionary<string, string[]>;
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

        /// <summary>
        /// Represents the ModelState if the response has "400 Bad Request" status code and ModelState is available.
        /// </summary>
        public Dictionary<string, string[]> ModelState { get; private set; }
    }
}