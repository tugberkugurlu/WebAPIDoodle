using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Http.Filters;
using System.Web.Http.Controllers;
using System.Net;
using WebApiDoodle.Web.Http;
using WebApiDoodle.Net.Http;

namespace WebApiDoodle.Web.Filters {

    public class RequireHttpsAttribute : AuthorizationFilterAttribute {

        public override void OnAuthorization(HttpActionContext actionContext) {

            if (actionContext.Request.RequestUri.Scheme != Uri.UriSchemeHttps) {

                actionContext.Response = new HttpStringResponseMessage("SSL required", HttpStatusCode.Forbidden);
            }
        }
    }
}