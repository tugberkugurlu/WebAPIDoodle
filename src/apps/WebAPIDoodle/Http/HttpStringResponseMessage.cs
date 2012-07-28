using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Net;

namespace WebAPIDoodle.Http {

    public class HttpStringResponseMessage : HttpResponseMessage {

        public HttpStringResponseMessage(string content) {

            base.Content = new StringContent(content);
        }

        public HttpStringResponseMessage(string content, HttpStatusCode statusCode) : this(content) {

            base.StatusCode = statusCode;
        }
    }
}