using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Http.Controllers;

namespace WebAPIDoodle {

    public interface IApiControllerBase {

        void OnActionExecuting(HttpControllerContext controllerContext);
        void OnActionExecuted(HttpResponseMessage response);
    }
}
