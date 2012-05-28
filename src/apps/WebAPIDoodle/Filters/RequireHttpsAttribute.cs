using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Http.Filters;
using System.Web.Http.Controllers;
using WebAPIDoodle.Messages;
using System.Net;

namespace WebAPIDoodle.Filters {

    public class RequireHttpsAttribute : AuthorizationFilterAttribute {

        public override void OnAuthorization(HttpActionContext actionContext) {

            if (actionContext.Request.RequestUri.Scheme != Uri.UriSchemeHttps) {

                actionContext.Response = new HttpResponseMessageWithStringContent("SSL required", HttpStatusCode.Forbidden);
            }
        }
    }
}