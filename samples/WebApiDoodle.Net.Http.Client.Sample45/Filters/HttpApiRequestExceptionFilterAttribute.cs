using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net.Http;
using System.Web.Http.Filters;
using System.Net;

namespace WebApiDoodle.Net.Http.Client.Sample45.Filters {

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class HttpApiRequestExceptionFilterAttribute : ExceptionFilterAttribute {

        public override void OnException(HttpActionExecutedContext actionExecutedContext) {

            var apiException = actionExecutedContext.Exception as HttpApiRequestException;
            if(apiException != null) {

                actionExecutedContext.Response = 
                    actionExecutedContext.Request.CreateResponse(HttpStatusCode.NotFound);
            }
        }
    }
}