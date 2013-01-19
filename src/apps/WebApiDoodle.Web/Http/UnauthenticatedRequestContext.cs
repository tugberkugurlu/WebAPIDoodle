using System.Net.Http;

namespace WebApiDoodle.Web.Http {
    
    public class UnauthenticatedRequestContext {

        public HttpRequestMessage Request { get; private set; }
        public HttpResponseMessage Response { get; set; }

        public UnauthenticatedRequestContext(HttpRequestMessage request) {

            Request = request;
        }
    }
}