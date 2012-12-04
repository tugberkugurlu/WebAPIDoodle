using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebAPIDoodle.Http;
using Xunit;

namespace WebAPIDoodle.Test.MessageHandlers {

    public class BasicAuthenticationHandlerTest {

        private const string UserName = "foo";
        private const string Password = "bar";

        [Fact, NullUpCurrentPrincipal, GCForce]
        public Task ReturnsUnauthorizedIfAuthorizationHeaderIsNotSupplied() { 

            //Arange
            var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost");
            var customBasicAuthHandler = new CustomBasicAuthHandler();

            //Act
            return TestHelper.InvokeMessageHandler(request, customBasicAuthHandler)

                .ContinueWith(task => { 

                    //Assert
                    Assert.Equal(TaskStatus.RanToCompletion, task.Status);
                    Assert.Equal(HttpStatusCode.Unauthorized, task.Result.StatusCode);
                });
        }

        [Fact, NullUpCurrentPrincipal, GCForce]
        public Task ReturnsUnauthorizedWhenAuthorizationHeaderIsNotVerified() { 

            //Arange
            string usernameAndPassword = string.Format("{0}:{1}", "guydy", "efuıry");
            var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost");
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", EncodeToBase64(usernameAndPassword));
            var customBasicAuthHandler = new CustomBasicAuthHandler();

            //Act
            return TestHelper.InvokeMessageHandler(request, customBasicAuthHandler)

                .ContinueWith(task => {

                    //Assert
                    Assert.Equal(TaskStatus.RanToCompletion, task.Status);
                    Assert.Equal(HttpStatusCode.Unauthorized, task.Result.StatusCode);
                });
        }

        [Fact, NullUpCurrentPrincipal, GCForce]
        public Task Returns200WhenAuthorizationHeaderIsVerified() {

            //Arange
            string usernameAndPassword = string.Format("{0}:{1}", UserName, Password);
            var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost");
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", EncodeToBase64(usernameAndPassword));
            var customBasicAuthHandler = new CustomBasicAuthHandler();

            //Act
            return TestHelper.InvokeMessageHandler(request, customBasicAuthHandler)

                .ContinueWith(task => {

                    //Assert
                    Assert.Equal(TaskStatus.RanToCompletion, task.Status);
                    Assert.Equal(HttpStatusCode.OK, task.Result.StatusCode);
                });
        }

        [Fact, NullUpCurrentPrincipal, GCForce]
        public Task SetsCurrentThreadPrincipalWhenAuthorizationHeaderIsVerified() {

            //Arange
            string usernameAndPassword = string.Format("{0}:{1}", UserName, Password);
            var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost");
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", EncodeToBase64(usernameAndPassword));
            var customBasicAuthHandler = new CustomBasicAuthHandler();

            //Act
            return TestHelper.InvokeMessageHandler(request, customBasicAuthHandler)

                .ContinueWith(task => {

                    //Assert
                    Assert.Equal(TaskStatus.RanToCompletion, task.Status);
                    Assert.NotNull(Thread.CurrentPrincipal);
                    Assert.IsType<GenericPrincipal>(Thread.CurrentPrincipal);
                });
        }

        [Fact, NullUpCurrentPrincipal, GCForce]
        public Task SuppressesTheAuthIfAlreadyAuthenticatedAndSuppressIsRequested() {

            //Arange
            Thread.CurrentPrincipal = new GenericPrincipal(new GenericIdentity(UserName), null);
            var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost");
            var suppressedCustomBasicAuthHandler = new SuppressedCustomBasicAuthHandler();

            //Act
            return TestHelper.InvokeMessageHandler(request, suppressedCustomBasicAuthHandler)

                .ContinueWith(task => {

                    Assert.Equal(TaskStatus.RanToCompletion, task.Status);
                    Assert.False(suppressedCustomBasicAuthHandler.IsAuthenticateUserCalled);
                    Assert.Equal(HttpStatusCode.OK, task.Result.StatusCode);
                });
        }

        [Fact, NullUpCurrentPrincipal, GCForce]
        public Task HonorsTheOverriddenHandleUnauthenticatedRequestAndSetsTheResponse() {

            //Arange
            string usernameAndPassword = string.Format("{0}:{1}", UserName, Password);
            var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost");
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", EncodeToBase64(usernameAndPassword));
            var customBasicAuthHandlerWithUnauthImpl = new CustomBasicAuthHandlerWithUnauthImpl();

            //Act
            return TestHelper.InvokeMessageHandler(request, customBasicAuthHandlerWithUnauthImpl)

                .ContinueWith(task => {

                    //Assert
                    Assert.Equal(TaskStatus.RanToCompletion, task.Status);
                    Assert.Equal(HttpStatusCode.Ambiguous, task.Result.StatusCode);
                });
        }

        [Fact, NullUpCurrentPrincipal, GCForce]
        public Task HonorsTheOverriddenHandleUnauthenticatedRequestAndCallsTheInnerHandler() {

            //Arange
            string usernameAndPassword = string.Format("{0}:{1}", UserName, Password);
            var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost");
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", EncodeToBase64(usernameAndPassword));
            var customBasicAuthHandlerWithEmptyUnauthImpl = new CustomBasicAuthHandlerWithEmptyUnauthImpl();

            //Act
            return TestHelper.InvokeMessageHandler(request, customBasicAuthHandlerWithEmptyUnauthImpl)

                .ContinueWith(task => {

                    //Assert
                    Assert.Equal(TaskStatus.RanToCompletion, task.Status);
                    Assert.Equal(HttpStatusCode.OK, task.Result.StatusCode);
                    Assert.False(Thread.CurrentPrincipal.Identity.IsAuthenticated);
                });
        }

        private static string EncodeToBase64(string value) {

            byte[] toEncodeAsBytes = Encoding.UTF8.GetBytes(value);
            return Convert.ToBase64String(toEncodeAsBytes);
        }

        public class CustomBasicAuthHandler : BasicAuthenticationHandler {

            protected override Task<IPrincipal> AuthenticateUserAsync(HttpRequestMessage request, string username, string password, CancellationToken cancellationToken) {

                if (username == UserName && password == Password) {

                    var identity = new GenericIdentity(username);
                    return TaskHelpers.FromResult<IPrincipal>(new GenericPrincipal(identity, null));
                }

                return TaskHelpers.FromResult<IPrincipal>(null);
            }
        }

        public class SuppressedCustomBasicAuthHandler : BasicAuthenticationHandler {

            public SuppressedCustomBasicAuthHandler() 
                : base(suppressIfAlreadyAuthenticated: true) { 
            }

            public bool IsAuthenticateUserCalled { get; private set; }

            protected override Task<IPrincipal> AuthenticateUserAsync(HttpRequestMessage request, string username, string password, CancellationToken cancellationToken) {

                IsAuthenticateUserCalled = true;
                return TaskHelpers.FromResult<IPrincipal>(null);
            }
        }

        public class CustomBasicAuthHandlerWithUnauthImpl : BasicAuthenticationHandler {

            protected override Task<IPrincipal> AuthenticateUserAsync(HttpRequestMessage request, string username, string password, CancellationToken cancellationToken) {

                return TaskHelpers.FromResult<IPrincipal>(null);
            }

            protected override void HandleUnauthenticatedRequest(
                UnauthenticatedRequestContext context) {

                context.Response = new HttpResponseMessage(HttpStatusCode.Ambiguous);
            }
        }

        public class CustomBasicAuthHandlerWithEmptyUnauthImpl : BasicAuthenticationHandler {

            protected override Task<IPrincipal> AuthenticateUserAsync(
                HttpRequestMessage request, string username, string password, 
                CancellationToken cancellationToken) {

                return TaskHelpers.FromResult<IPrincipal>(null);
            }

            protected override void HandleUnauthenticatedRequest(
                UnauthenticatedRequestContext context) { }
        }
    }
}