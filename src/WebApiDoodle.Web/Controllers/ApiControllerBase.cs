using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace WebApiDoodle.Web.Controllers {

    /// <summary>
    /// Base APiController class which provides aditional two methods that run before and after the controller execution
    /// </summary>
    public abstract class ApiControllerBase : ApiController, IApiControllerBase {

        public override Task<HttpResponseMessage> ExecuteAsync(HttpControllerContext controllerContext, CancellationToken cancellationToken) {

            //We cannot run the Initialize method here to prevent any inconsistency
            //because ExecuteAsync method doesn't like Request to be not null.
            //For example, Initialize method sets up some public Properties such as 
            //ControllerContext, Request and Configuration. If the user tries to reach out to them
            //inside the OnControllerExecuting method and we don't run the Initialize, there 
            //will be NullReferenceExceptions which is very bad. But we cannot set Request
            //as well. So, set ControllerContext and Configuration at least
            ControllerContext = controllerContext;
            Configuration = controllerContext.Configuration;

            HttpControllerExecutingContext controllerExecutingContext = new HttpControllerExecutingContext(controllerContext);

            try {

                //If the user try to reach out to the public Request property,
                //that property will be null because we cannot set that value before
                //the ExecuteAsync method is run. So, it shouldn't be used.
                OnControllerExecuting(controllerExecutingContext);
            }
            catch (Exception e) {

                OnControllerExecuted(null);
                return TaskHelpers.FromError<HttpResponseMessage>(e);
            }

            //Set the response message if the response coming from OnControllerExecuting is not null
            //and OnControllerExecuting is successfully completed
            if (controllerExecutingContext.Response != null) {

                OnControllerExecuted(controllerExecutingContext.Response);
                return TaskHelpers.FromResult<HttpResponseMessage>(controllerExecutingContext.Response);
            }

            //TODO: Handle this better. E.g: What happens when an exception is thrown?
            return base.ExecuteAsync(controllerContext, cancellationToken).ContinueWith(task => {
                
                var response = task.Result;
                OnControllerExecuted(response);
                return response;
            });
        }

        /// <summary>
        /// The method which will run just before the controller execution.
        /// If this method sets HttpControllerExecutingContext.Response property to a non-null
        /// value, then the controller execution will be terminated and the given response
        /// message will be pass through.
        /// </summary>
        /// <param name="controllerExecutingContext"></param>
        public virtual void OnControllerExecuting(HttpControllerExecutingContext controllerExecutingContext) { }

        /// <summary>
        /// The method which will run just after the controller execution. If the 
        /// returned Task{HttpResponseMessage} object is not in the RanToCompletion state,
        /// the povided HttpResponseMessage parameter will be null.
        /// </summary>
        /// <param name="response"></param>
        public virtual void OnControllerExecuted(HttpResponseMessage response) { }
    }
}