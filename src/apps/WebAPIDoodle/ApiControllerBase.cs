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

    /// <summary>
    /// Base APiController class which provides aditional two methods that run before and after the controller execution
    /// </summary>
    public abstract class ApiControllerBase : ApiController, IApiControllerBase {

        private Task<HttpResponseMessage> _intermediateResponseMessage;

        protected override void Initialize(HttpControllerContext controllerContext) {
            
            base.Initialize(controllerContext);
            
            HttpControllerExecutingContext controllerExecutingContext = new HttpControllerExecutingContext(controllerContext);

            try {

                OnControllerExecuting(controllerExecutingContext);
            }
            catch (Exception e) {

                _intermediateResponseMessage = TaskHelpers.FromError<HttpResponseMessage>(e);
            }

            //Set the response message if the response coming from OnControllerExecuting is not null
            if (controllerExecutingContext.Response != null) {

                _intermediateResponseMessage = TaskHelpers.FromResult<HttpResponseMessage>(controllerExecutingContext.Response);
            }
        }

        public override Task<HttpResponseMessage> ExecuteAsync(HttpControllerContext controllerContext, CancellationToken cancellationToken) {

            if (_intermediateResponseMessage != null) {

                OnControllerExecuted(_intermediateResponseMessage.Result);
                return _intermediateResponseMessage;
            }

            //TODO: Handle this better. E.g: What happens when an exception is thrown?
            return base.ExecuteAsync(controllerContext, cancellationToken).ContinueWith(task => {

                var response = task.Result;
                OnControllerExecuted(response);
                return response;
            });
        }

        public virtual void OnControllerExecuting(HttpControllerExecutingContext controllerExecutingContext) { }
        public virtual void OnControllerExecuted(HttpResponseMessage response) { }
    }
}