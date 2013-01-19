using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Http.Controllers;

namespace WebApiDoodle.Web.Controllers {

    public interface IApiControllerBase {

        void OnControllerExecuting(HttpControllerExecutingContext controllerExecutingContext);
        void OnControllerExecuted(HttpResponseMessage response);
    }
}
