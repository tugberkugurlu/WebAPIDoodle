using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Net;

namespace WebAPIDoodle.Messages {

    public class HttpResponseMessageWithStringContent : HttpResponseMessage {

        public HttpResponseMessageWithStringContent(string content) {

            base.Content = new StringContent(content);
        }

        public HttpResponseMessageWithStringContent(string content, HttpStatusCode statusCode) : this(content) {

            base.StatusCode = statusCode;
        }
    }
}