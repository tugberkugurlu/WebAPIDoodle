using System;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace WebApiDoodle.Web.Filters {

    public class RequireHttpsAttribute : AuthorizationFilterAttribute {

        public override void OnAuthorization(HttpActionContext actionContext) {

            if (actionContext.Request.RequestUri.Scheme != Uri.UriSchemeHttps) {

                actionContext.Response = new HttpResponseMessage(HttpStatusCode.Forbidden) {
                    ReasonPhrase = "Forbidden (SSL Required)"
                };
            }
        }
    }
}