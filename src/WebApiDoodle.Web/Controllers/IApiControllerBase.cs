using System.Net.Http;

namespace WebApiDoodle.Web.Controllers {

    public interface IApiControllerBase {

        void OnControllerExecuting(HttpControllerExecutingContext controllerExecutingContext);
        void OnControllerExecuted(HttpResponseMessage response);
    }
}
