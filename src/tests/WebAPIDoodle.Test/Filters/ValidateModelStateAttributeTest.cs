using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web.Http;
using WebAPIDoodle.Filters;
using Xunit;

namespace WebAPIDoodle.Test.Filters {
    
    public class ValidateModelStateAttributeTest {

        [Fact]
        public void InvalidModelStateFilterAttribute_ShouldSetThe400ResponseIfTheModelStateIsNotValid() {

            //Arange
            var validateModelStateFilter = new InvalidModelStateFilterAttribute();
            var request = new HttpRequestMessage();
            var actionContext = ContextUtil.GetHttpActionContext(request);
            actionContext.ModelState.AddModelError("foo", "foo is invalid.");

            //Act
            validateModelStateFilter.OnActionExecuting(actionContext);

            //Assert
            Assert.NotNull(actionContext.Response);
            Assert.Equal(actionContext.Response.StatusCode, HttpStatusCode.BadRequest);
        }

        [Fact]
        public void InvalidModelStateFilterAttribute_ShouldNotSetTheResponseIfTheModelStateIsValid() {

            //Arange
            var validateModelStateFilter = new InvalidModelStateFilterAttribute();
            var request = new HttpRequestMessage();
            var actionContext = ContextUtil.GetHttpActionContext(request);

            //Act
            validateModelStateFilter.OnActionExecuting(actionContext);

            //Assert
            Assert.Null(actionContext.Response);
        }

        [Fact]
        public void InvalidModelStateFilterAttribute_ShouldReturnTheResponseWithProperContentTypeIfTheModelStateIsNotValid() {

            //NOTE: This test might seems that we are here testing the framework
            //      stuff but we are not. We just make sure here that InvalidModelStateFilterAttribute
            //      really honors the conneg.

            //Arange
            var validateModelStateFilter = new InvalidModelStateFilterAttribute();
            var request = new HttpRequestMessage();
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var actionContext = ContextUtil.GetHttpActionContext(request);
            actionContext.ModelState.AddModelError("foo", "foo is invalid.");

            //NOTE: Here, the response is being returned through the CreateErrorResponse extension
            //      method of the HttpRequestMessage object. What this basically does is
            //      to pass an HttpError instance to another extension method, CreateResponse<T>.
            //      The CreateResponse<T> method looks at the configuration instance 
            //      (yes, config shouldn't be null) and gets the IContentNegotiator service 
            //      through Services. If we create a HttpConfiguration object with its
            //      parameterless ctor, the negotiator will be the type of DefaultContentNegotiator.
            //      DefaultContentNegotiator should negotiate properly here.

            //Act
            validateModelStateFilter.OnActionExecuting(actionContext);

            //Assert
            Assert.NotNull(actionContext.Response);
            Assert.True(actionContext.Response.Content.Headers.ContentType.MediaType.Equals("application/json", StringComparison.OrdinalIgnoreCase));
        }
    }
}
