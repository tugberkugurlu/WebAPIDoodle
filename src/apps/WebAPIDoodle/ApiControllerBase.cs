using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace WebAPIDoodle {

    public abstract class ApiControllerBase : ApiController, IApiControllerBase {

        public virtual void OnActionExecuting(HttpControllerContext controllerContext) { }
        public virtual void OnActionExecuted(HttpResponseMessage response) { }

        protected override void Initialize(HttpControllerContext controllerContext) {
            
            base.Initialize(controllerContext);
            OnActionExecuting(controllerContext);
        }

        public override Task<HttpResponseMessage> ExecuteAsync(HttpControllerContext controllerContext, CancellationToken cancellationToken) {
            
            //TODO: Handle this better. E.g: What happens when an exception is thrown?
            return base.ExecuteAsync(controllerContext, cancellationToken).ContinueWith(task => {

                var response = task.Result;
                OnActionExecuted(response);
                return response;
            });
        }
    }
}