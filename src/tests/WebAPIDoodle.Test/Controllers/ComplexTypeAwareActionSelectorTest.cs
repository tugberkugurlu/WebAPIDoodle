using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Http.Controllers;
using WebAPIDoodle.Controllers;
using WebAPIDoodle.Test.Controllers.Apis;
using System.Web.Http;
using Xunit;
using System.Net;
using Moq;
using System.ComponentModel;

namespace WebAPIDoodle.Test.Controllers {
    
    public class ComplexTypeAwareActionSelectorTest {

        //TODO: Test simple underlying types (e.g: Nullable types)
        //TODO: Test complex type GET and POST methods at the same controller

        [Fact]
        public void ComplexTypeUriParamFriendlyActionSelector_SelectAction_With_DifferentExecutionContexts() {

            HttpControllerContext GetControllerContext = CreateControllerContext("Users", typeof(UsersController));
            GetControllerContext.Request = new HttpRequestMessage { 
                Method = HttpMethod.Get
            };

            HttpControllerContext PostControllerContext = CreateControllerContext("Users", typeof(UsersController));
            PostControllerContext.Request = new HttpRequestMessage {
                Method = HttpMethod.Post
            };

            HttpActionDescriptor getActionDescriptor = GetControllerContext.ControllerDescriptor.Configuration.Services.GetActionSelector().SelectAction(GetControllerContext);
            HttpActionDescriptor postActionDescriptor = PostControllerContext.ControllerDescriptor.Configuration.Services.GetActionSelector().SelectAction(PostControllerContext);

            Assert.Equal("Get", getActionDescriptor.ActionName);
            Assert.Equal("Post", postActionDescriptor.ActionName);
        }

        [Fact]
        public void ComplexTypeUriParamFriendlyActionSelector_SelectAction_Throws_IfContextIsNull() {

            ComplexTypeAwareActionSelector actionSelector = new ComplexTypeAwareActionSelector();
            Assert.Throws<ArgumentNullException>(() => actionSelector.SelectAction(null));
        }

        [Fact]
        public void ComplexTypeUriParamFriendlyActionSelector_SelectAction_With_ActionSelectorParams() {

            HttpControllerContext getControllerContext = CreateControllerContext("Cars1", typeof(Cars1Controller));
            getControllerContext.Request = new HttpRequestMessage { 
                RequestUri = new Uri("http://localhost/api/cars?foo=fooVal")
            };

            HttpActionDescriptor getActionDescriptor = getControllerContext.ControllerDescriptor.Configuration.Services.GetActionSelector().SelectAction(getControllerContext);

            Assert.Equal("GetCarsForCmd1", getActionDescriptor.ActionName);
        }

        [Fact]
        public void ComplexTypeUriParamFriendlyActionSelector_SelectAction_With_ActionSelectorParams_And_OneValidSimpleTypeActionParams_Should_Select() {

            HttpControllerContext getControllerContext = CreateControllerContext("Cars2", typeof(Cars2Controller));
            getControllerContext.Request = new HttpRequestMessage {
                RequestUri = new Uri("http://localhost/api/cars?foo=fooVal&bar=barVal")
            };

            HttpActionDescriptor getActionDescriptor = getControllerContext.ControllerDescriptor.Configuration.Services.GetActionSelector().SelectAction(getControllerContext);

            Assert.Equal("GetCarsForCmd2", getActionDescriptor.ActionName);
        }

        [Fact]
        public void ComplexTypeUriParamFriendlyActionSelector_SelectAction_With_ActionSelectorParams_And_MultipleValidSimpleTypeActionParams_Should_Select() {

            HttpControllerContext getControllerContext = CreateControllerContext("Cars3", typeof(Cars3Controller));
            getControllerContext.Request = new HttpRequestMessage {
                RequestUri = new Uri("http://localhost/api/cars?foo=fooVal&bar=barVal")
            };

            HttpActionDescriptor getActionDescriptor = getControllerContext.ControllerDescriptor.Configuration.Services.GetActionSelector().SelectAction(getControllerContext);

            Assert.Equal("GetCarsForCmd2", getActionDescriptor.ActionName);
        }

        [Fact]
        public void ComplexTypeUriParamFriendlyActionSelector_SelectAction_With_ActionSelectorParams_And_OneInvalidSimpleTypeActionParams_Should_FallBackToTheBestPossibleAction() {

            HttpControllerContext getControllerContext = CreateControllerContext("Cars4", typeof(Cars4Controller));
            getControllerContext.Request = new HttpRequestMessage {
                RequestUri = new Uri("http://localhost/api/cars?foo=fooVal&bar=barVal")
            };

            HttpActionDescriptor getActionDescriptor = getControllerContext.ControllerDescriptor.Configuration.Services.GetActionSelector().SelectAction(getControllerContext);

            Assert.Equal("GetCarsForCmd1", getActionDescriptor.ActionName);
        }

        [Fact]
        public void ComplexTypeUriParamFriendlyActionSelector_SelectAction_With_ActionSelectorParams_And_OneInvalidSimpleTypeActionParams_Should_Throw_If_APossibleActionFallbackIsNotAvailable() {

            HttpControllerContext getControllerContext = CreateControllerContext("Cars5", typeof(Cars5Controller));
            getControllerContext.Request = new HttpRequestMessage {
                RequestUri = new Uri("http://localhost/api/cars?foo=fooVal&bar=barVal")
            };

            Assert.Throws<HttpResponseException>(
                () => getControllerContext.ControllerDescriptor.Configuration.Services.GetActionSelector().SelectAction(getControllerContext));
        }

        [Fact]
        public void ComplexTypeUriParamFriendlyActionSelector_SelectAction_With_ActionSelectorParams_And_NonPubliclySetableSimpleTypeInsideTheComplexTypeActionParam_ShouldNotBeConsidered() {

            HttpControllerContext getControllerContext = CreateControllerContext("Cars6", typeof(Cars6Controller));
            getControllerContext.Request = new HttpRequestMessage {
                RequestUri = new Uri("http://localhost/api/cars?foo=fooVal&bar=barVal")
            };

            HttpActionDescriptor getActionDescriptor = getControllerContext.ControllerDescriptor.Configuration.Services.GetActionSelector().SelectAction(getControllerContext);

            Assert.Equal("GetCarsForCmd21", getActionDescriptor.ActionName);
        }

        [Fact]
        public void ComplexTypeUriParamFriendlyActionSelector_SelectAction_With_ActionSelectorParams_And_NonPubliclySetableSimpleTypeInsideTheComplexTypeActionParam_ShouldNotBeConsidered_And_Throws() {

            HttpControllerContext getControllerContext = CreateControllerContext("Cars7", typeof(Cars7Controller));
            getControllerContext.Request = new HttpRequestMessage {
                RequestUri = new Uri("http://localhost/api/cars?foo=fooVal&bar=barVal")
            };

            Assert.Throws<InvalidOperationException>(
                () => getControllerContext.ControllerDescriptor.Configuration.Services.GetActionSelector().SelectAction(getControllerContext));
        }

        [Fact]
        public void ComplexTypeUriParamFriendlyActionSelector_SelectAction_With_ActionSelectorParams_And_SimpleTypePropertyWithNoBindingAttributeInsideTheComplexTypeActionParam_ShouldNotBeConsidered() {

            HttpControllerContext getControllerContext = CreateControllerContext("Cars8", typeof(Cars8Controller));
            getControllerContext.Request = new HttpRequestMessage {
                RequestUri = new Uri("http://localhost/api/cars?foo=fooVal&bar=barVal")
            };

            HttpActionDescriptor getActionDescriptor = getControllerContext.ControllerDescriptor.Configuration.Services.GetActionSelector().SelectAction(getControllerContext);

            Assert.Equal("GetCarsForCmd23", getActionDescriptor.ActionName);
        }

        [Fact]
        public void ComplexTypeUriParamFriendlyActionSelector_SelectAction_With_ActionSelectorParams_And_SimpleTypePropertyWithNoBindingAttributeInsideTheComplexTypeActionParam_ShouldNotBeConsidered_And_Throws() {

            HttpControllerContext getControllerContext = CreateControllerContext("Cars9", typeof(Cars9Controller));
            getControllerContext.Request = new HttpRequestMessage {
                RequestUri = new Uri("http://localhost/api/cars?foo=fooVal&bar=barVal")
            };

            Assert.Throws<InvalidOperationException>(
                () => getControllerContext.ControllerDescriptor.Configuration.Services.GetActionSelector().SelectAction(getControllerContext));
        }

        private HttpControllerContext CreateControllerContext(string controllerName, Type controllerType) {

            ComplexTypeAwareActionSelector actionSelector = new ComplexTypeAwareActionSelector();
            HttpControllerContext controllerContext = ContextUtil.CreateControllerContext();
            HttpControllerDescriptor controllerDescriptor = new HttpControllerDescriptor(controllerContext.Configuration, controllerName, controllerType);
            controllerDescriptor.Configuration.Services.Replace(typeof(IHttpActionSelector), actionSelector);
            controllerContext.ControllerDescriptor = controllerDescriptor;

            return controllerContext;
        }

        public class Cmd1 {

            public string Foo { get; set; }
        }

        public class Cmd2 {

            public string Foo { get; set; }
            public string Bar { get; set; }
        }

        public class Cars1Controller : ApiController {

            public HttpResponseMessage GetCarsForCmd1([FromUri]Cmd1 cmd) {

                return new HttpResponseMessage(HttpStatusCode.OK) {
                    Content = new StringContent("Default Car for Cmd1")
                };
            }

            public HttpResponseMessage GetCarsForCmd2([FromUri]Cmd2 cmd) {

                return new HttpResponseMessage(HttpStatusCode.OK) {
                    Content = new StringContent("Default Car for Cmd2")
                };
            }

            public HttpResponseMessage Post() {
                return new HttpResponseMessage(HttpStatusCode.OK) {
                    Content = new StringContent("Car Posted")
                };
            }
        }

        public class Cars2Controller : ApiController {

            public HttpResponseMessage GetCarsForCmd1([FromUri]Cmd1 cmd) {

                return new HttpResponseMessage(HttpStatusCode.OK) {
                    Content = new StringContent("Default Car for Cmd1")
                };
            }

            public HttpResponseMessage GetCarsForCmd2(string bar, [FromUri]Cmd2 cmd) {

                return new HttpResponseMessage(HttpStatusCode.OK) {
                    Content = new StringContent("Default Car for Cmd2")
                };
            }
        }

        public class Cars3Controller : ApiController {

            public HttpResponseMessage GetCarsForCmd1([FromUri]Cmd1 cmd) {

                return new HttpResponseMessage(HttpStatusCode.OK) {
                    Content = new StringContent("Default Car for Cmd1")
                };
            }

            public HttpResponseMessage GetCarsForCmd2(string foo, int bar, [FromUri]Cmd2 cmd) {

                return new HttpResponseMessage(HttpStatusCode.OK) {
                    Content = new StringContent("Default Car for Cmd2")
                };
            }
        }

        public class Cars4Controller : ApiController {

            public HttpResponseMessage GetCarsForCmd1([FromUri]Cmd1 cmd) {

                return new HttpResponseMessage(HttpStatusCode.OK) {
                    Content = new StringContent("Default Car for Cmd1")
                };
            }

            public HttpResponseMessage GetCarsForCmd2(string foo, int bar, string baz, [FromUri]Cmd2 cmd) {

                return new HttpResponseMessage(HttpStatusCode.OK) {
                    Content = new StringContent("Default Car for Cmd2")
                };
            }
        }

        public class Cars5Controller : ApiController {

            public HttpResponseMessage GetCarsForCmd1(string baz, [FromUri]Cmd1 cmd) {

                return new HttpResponseMessage(HttpStatusCode.OK) {
                    Content = new StringContent("Default Car for Cmd1")
                };
            }

            public HttpResponseMessage GetCarsForCmd2(string foo, int bar, string baz, [FromUri]Cmd2 cmd) {

                return new HttpResponseMessage(HttpStatusCode.OK) {
                    Content = new StringContent("Default Car for Cmd2")
                };
            }
        }

        #region readonly properties

        public class Cmd11 {

            public string Foo { get; set; }
            public string FooBar { get { return Foo.ToLower(); } }
        }

        public class Cmd21 {

            public string Foo { get; set; }
            public string Bar { get; set; }

            public string FooBar { get { return string.Format("{0}{1}", Foo, Bar); } }
        }

        public class Cars6Controller : ApiController {

            public HttpResponseMessage GetCarsForCmd11([FromUri]Cmd11 cmd) {

                return new HttpResponseMessage(HttpStatusCode.OK) {
                    Content = new StringContent("Default Car for Cmd1")
                };
            }

            public HttpResponseMessage GetCarsForCmd21([FromUri]Cmd21 cmd) {

                return new HttpResponseMessage(HttpStatusCode.OK) {
                    Content = new StringContent("Default Car for Cmd2")
                };
            }
        }

        #endregion

        #region readonly properties which throws

        public class Cmd12 {

            public string Foo { get; set; }
        }

        public class Cmd22 {

            public string Foo { get; set; }
            public string Bar { get { return string.Format("_{0}_", Foo); } }
        }

        public class Cars7Controller : ApiController {

            public HttpResponseMessage GetCarsForCmd11([FromUri]Cmd12 cmd) {

                return new HttpResponseMessage(HttpStatusCode.OK) {
                    Content = new StringContent("Default Car for Cmd1")
                };
            }

            public HttpResponseMessage GetCarsForCmd21([FromUri]Cmd22 cmd) {

                return new HttpResponseMessage(HttpStatusCode.OK) {
                    Content = new StringContent("Default Car for Cmd2")
                };
            }
        }

        #endregion

        #region BindingInfo attribute

        public class Cmd13 {

            public string Foo { get; set; }
            
            [BindingInfo(NoBinding = true)]
            public string FooBar { get; set; }
        }

        public class Cmd23 {

            public string Foo { get; set; }
            public string Bar { get; set; }

            [BindingInfo(NoBinding = true)]
            public string FooBar { get; set; }
        }

        public class Cars8Controller : ApiController {

            public HttpResponseMessage GetCarsForCmd13([FromUri]Cmd13 cmd) {

                return new HttpResponseMessage(HttpStatusCode.OK) {
                    Content = new StringContent("Default Car for Cmd1")
                };
            }

            public HttpResponseMessage GetCarsForCmd23([FromUri]Cmd23 cmd) {

                return new HttpResponseMessage(HttpStatusCode.OK) {
                    Content = new StringContent("Default Car for Cmd2")
                };
            }
        }

        #endregion

        #region BindingInfo attribute which throws

        public class Cmd14 {

            public string Foo { get; set; }
        }

        public class Cmd24 {

            public string Foo { get; set; }
            
            [BindingInfo(NoBinding = true)]
            public string Bar { get; set; }
        }

        public class Cars9Controller : ApiController {

            public HttpResponseMessage GetCarsForCmd14([FromUri]Cmd14 cmd) {

                return new HttpResponseMessage(HttpStatusCode.OK) {
                    Content = new StringContent("Default Car for Cmd1")
                };
            }

            public HttpResponseMessage GetCarsForCmd24([FromUri]Cmd24 cmd) {

                return new HttpResponseMessage(HttpStatusCode.OK) {
                    Content = new StringContent("Default Car for Cmd2")
                };
            }
        }

        #endregion
    }
}