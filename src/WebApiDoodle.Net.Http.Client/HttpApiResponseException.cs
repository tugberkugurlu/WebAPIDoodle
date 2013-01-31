using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebApiDoodle.Net.Http.Client {
 
    [Serializable]
    public class HttpApiResponseException : Exception {

        public HttpApiResponseException(
            string message,
            HttpApiResponseMessage httpApiResponseMessage) : base(message) {

            ApiResponse = httpApiResponseMessage;
        }

        public HttpApiResponseMessage ApiResponse { get; private set; }
    }
}