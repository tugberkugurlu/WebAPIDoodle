using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace WebApiDoodle.Web.Test.Controllers.Apis {

    public class UsersController : ApiController {

        public HttpResponseMessage Get() {

            return new HttpResponseMessage(HttpStatusCode.OK) {
                Content = new StringContent("Default User")
            };
        }

        public HttpResponseMessage Post() {
            return new HttpResponseMessage(HttpStatusCode.OK) {
                Content = new StringContent("User Posted")
            };
        }

        public HttpResponseMessage Put() {
            return new HttpResponseMessage(HttpStatusCode.OK) {
                Content = new StringContent("User Updated")
            };
        }

        public HttpResponseMessage Delete() {
            return new HttpResponseMessage(HttpStatusCode.OK) {
                Content = new StringContent("User Deleted")
            };
        }
    }
}