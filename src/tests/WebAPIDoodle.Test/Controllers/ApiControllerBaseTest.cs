using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Routing;
using Moq;
using WebAPIDoodle.Controllers;
using Xunit;

namespace WebAPIDoodle.Test.Controllers {

    public class ApiControllerBaseTest {

        [Fact, GCForce]
        public Task ApiControllerBase_InvokesOnControllerExecuting() {

            //Arrange
            var fakeController = new FakeOKController();
            var controllerContext = ContextUtil.CreateControllerContext(fakeController, "FakeOk", typeof(FakeOKController));

            //Act
            return fakeController.ExecuteAsync(controllerContext, CancellationToken.None)

            //Assert
                .ContinueWith(task => {
                    Assert.True(fakeController.OnControllerExecutingInvoked);
                });
        }

        [Fact, GCForce]
        public Task ApiControllerBase_InvokesOnControllerExecuted() {

            //Arrange
            var fakeController = new FakeOKController();
            var controllerContext = ContextUtil.CreateControllerContext(fakeController, "FakeOk", typeof(FakeOKController));

            //Act
            return fakeController.ExecuteAsync(controllerContext, CancellationToken.None)

            //Assert
                .ContinueWith(task => {
                    Assert.True(fakeController.OnControllerExecutedInvoked);
                });
        }

        [Fact, GCForce]
        public Task ApiControllerBase_InvokesActionIfOnControllerExecutingDoesNotTerminate() {

            //Arrange
            var fakeController = new FakeOKController();
            var controllerContext = ContextUtil.CreateControllerContext(fakeController, "FakeOk", typeof(FakeOKController));

            //Act
            return fakeController.ExecuteAsync(controllerContext, CancellationToken.None)

            //Assert
                .ContinueWith(task => {
                    Assert.Equal(HttpStatusCode.OK, task.Result.StatusCode);
                    Assert.Equal("Hello World", ((StringContent)task.Result.Content).ReadAsStringAsync().Result);
                });
        }

        [Fact, GCForce]
        public Task ApiControllerBase_DoesNotInvokeActionIfOnControllerExecutingTerminates() {

            //Arrange
            var fakeController = new FakeTerminatedController();
            var controllerContext = ContextUtil.CreateControllerContext(fakeController, "FakeTerminated", typeof(FakeTerminatedController));

            //Act
            return fakeController.ExecuteAsync(controllerContext, CancellationToken.None)

            //Assert
                .ContinueWith(task => {
                    Assert.True(fakeController.OnControllerExecutingInvoked);
                    Assert.True(fakeController.OnControllerExecutedInvoked);
                    Assert.False(fakeController.IsActionInvoked);
                    Assert.Equal(HttpStatusCode.NotAcceptable, task.Result.StatusCode);
                });
        }

        [Fact, GCForce]
        public Task ApiControllerBase_DoesNotInvokeActionIfOnControllerExecutingThrowsException() {

            //Arrange
            var fakeController = new FakeExceptionController();
            var controllerContext = ContextUtil.CreateControllerContext(fakeController, "FakeException", typeof(FakeExceptionController));

            //Act
            return fakeController.ExecuteAsync(controllerContext, CancellationToken.None)

            //Assert
                .ContinueWith(task => {
                    Assert.True(fakeController.OnControllerExecutingInvoked);
                    Assert.True(fakeController.OnControllerExecutedInvoked);
                    Assert.False(fakeController.IsActionInvoked);
                    Assert.Equal(TaskStatus.Faulted, task.Status);
                });
        }

        private class FakeOKController : ApiControllerBase {

            private bool _onControllerExecutingInvoked;
            private bool _onControllerExecutedInvoked;

            public bool OnControllerExecutingInvoked { get { return _onControllerExecutingInvoked; } }
            public bool OnControllerExecutedInvoked { get { return _onControllerExecutedInvoked; } }

            public override void OnControllerExecuting(HttpControllerExecutingContext controllerExecutingContext) {

                _onControllerExecutingInvoked = true;
            }

            public override void OnControllerExecuted(HttpResponseMessage response) {

                _onControllerExecutedInvoked = true;
            }

            public HttpResponseMessage Get() {

                return new HttpResponseMessage(HttpStatusCode.OK) { 
                    Content = new StringContent("Hello World")
                };
            }
        }

        private class FakeTerminatedController : ApiControllerBase {

            private bool _onControllerExecutingInvoked;
            private bool _onControllerExecutedInvoked;
            private bool _isActionInvoked;

            public bool OnControllerExecutingInvoked { get { return _onControllerExecutingInvoked; } }
            public bool OnControllerExecutedInvoked { get { return _onControllerExecutedInvoked; } }
            public bool IsActionInvoked { get { return _isActionInvoked; } }

            public override void OnControllerExecuting(HttpControllerExecutingContext controllerExecutingContext) {

                _onControllerExecutingInvoked = true;
                controllerExecutingContext.Response = new HttpResponseMessage(HttpStatusCode.NotAcceptable);
            }

            public override void OnControllerExecuted(HttpResponseMessage response) {

                _onControllerExecutedInvoked = true;
            }

            public HttpResponseMessage Get() {

                _isActionInvoked = true;

                return new HttpResponseMessage(HttpStatusCode.OK) {
                    Content = new StringContent("Hello World")
                };
            }
        }

        private class FakeExceptionController : ApiControllerBase {

            private bool _onControllerExecutingInvoked;
            private bool _onControllerExecutedInvoked;
            private bool _isActionInvoked;

            public bool OnControllerExecutingInvoked { get { return _onControllerExecutingInvoked; } }
            public bool OnControllerExecutedInvoked { get { return _onControllerExecutedInvoked; } }
            public bool IsActionInvoked { get { return _isActionInvoked; } }

            public override void OnControllerExecuting(HttpControllerExecutingContext controllerExecutingContext) {

                _onControllerExecutingInvoked = true;
                throw new NotImplementedException();
            }

            public override void OnControllerExecuted(HttpResponseMessage response) {

                _onControllerExecutedInvoked = true;
            }

            public HttpResponseMessage Get() {

                _isActionInvoked = true;

                return new HttpResponseMessage(HttpStatusCode.OK) {
                    Content = new StringContent("Hello World")
                };
            }
        }
    }
}
