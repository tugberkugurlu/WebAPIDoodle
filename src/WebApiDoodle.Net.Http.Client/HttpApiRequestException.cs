using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;

namespace WebApiDoodle.Net.Http.Client {
 
    [Serializable]
    public class HttpApiRequestException : HttpRequestException {

        public HttpApiRequestException(
            string message,
            HttpStatusCode httpStatusCode) : base(message) {

            StatusCode = httpStatusCode;
        }

        public HttpStatusCode StatusCode { get; private set; }
    }
}