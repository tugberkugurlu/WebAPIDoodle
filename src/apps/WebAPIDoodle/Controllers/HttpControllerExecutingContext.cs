using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Http.Controllers;

namespace WebApiDoodle.Web.Controllers {

    public class HttpControllerExecutingContext {

        private HttpControllerContext _controllerContext;

        public HttpControllerExecutingContext(HttpControllerContext controllerContext) {

            if (controllerContext == null) {

                throw new ArgumentNullException("controllerContext");
            }

            _controllerContext = controllerContext;
        }

        public HttpControllerContext ControllerContext {

            get { return _controllerContext; }
            set {
                if (value == null) {
                    throw new ArgumentNullException("value");
                }

                _controllerContext = value;
            }
        }

        public HttpResponseMessage Response { get; set; }
    }
}
