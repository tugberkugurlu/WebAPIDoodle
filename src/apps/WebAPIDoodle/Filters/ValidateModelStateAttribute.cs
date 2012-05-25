using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Filters;
using System.Web.Http.Controllers;
using System.Net.Http;
using System.Net;

namespace WebAPIDoodle.Filters {

    public class ValidateModelStateAttribute : ActionFilterAttribute {

        public override void OnActionExecuting(HttpActionContext actionContext) {

            var modelState = actionContext.ModelState;

            //check if modelstate is valid
            if (!modelState.IsValid) {

                //iterating through the model state collection
                var errors = modelState.Keys
                    .Where(key => modelState[key].Errors.Any())
                    .Select(key => new Dictionary<string, string> { 
                        { key, modelState[key].Errors.First().ErrorMessage }
                    });

                //creating the response this way ensures that 
                //the conneg will be done right by the server
                var response = actionContext.Request.CreateResponse<IEnumerable<Dictionary<string, string>>>(
                    HttpStatusCode.BadRequest, errors
                );

                //set the custom response message
                actionContext.Response = response;
            }
        }
    }
}
