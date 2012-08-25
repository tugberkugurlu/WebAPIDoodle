using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebAPIDoodle.Http;
using WebAPIDoodle.Http.Handlers;
using Xunit;

namespace WebAPIDoodle.Test.MessageHandlers {
    
    public class ApiKeyAuthenticationHandlerTest {

        private const string FakeApiKey = "fb0ab544-232e-46cf-be06-ee3619e61a42";
        private const string DesignatedApiKeyQueryStringParameter = "apiKey";
        private const string BaseRequestUri = "http://localhost/";

        [Fact, GCForce]
        public Task ApiKeyAuthenticationHandler_ReturnsUnauthorizedIfApiKeyQueryStringParameterIsNotSupplied() {

            //Arange
            var request = new HttpRequestMessage(HttpMethod.Get, BaseRequestUri);
            var customApiKeyAuthHandler = new CustomApiKeyAuthHandler(DesignatedApiKeyQueryStringParameter);

            //Act
            return TestHelper.InvokeMessageHandler(request, customApiKeyAuthHandler)

                .ContinueWith(task => {

                    //Assert
                    Assert.Equal(TaskStatus.RanToCompletion, task.Status);
                    Assert.Equal(HttpStatusCode.Unauthorized, task.Result.StatusCode);
                });
        }

        [Fact, GCForce]
        public Task ApiKeyAuthenticationHandler_ReturnsUnauthorizedIfApiKeyQueryStringValueIsNotValid() {

            //Arange
            var requestUri = string.Format("{0}?{1}={2}", BaseRequestUri, DesignatedApiKeyQueryStringParameter, "invalid-api-key");
            var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            var customApiKeyAuthHandler = new CustomApiKeyAuthHandler(DesignatedApiKeyQueryStringParameter);

            //Act
            return TestHelper.InvokeMessageHandler(request, customApiKeyAuthHandler)

                .ContinueWith(task => {

                    //Assert
                    Assert.Equal(TaskStatus.RanToCompletion, task.Status);
                    Assert.Equal(HttpStatusCode.Unauthorized, task.Result.StatusCode);
                });
        }

        [Fact, GCForce]
        public Task ApiKeyAuthenticationHandler_Returns200IfApiKeyQueryStringValueIsValid() {

            //Arange
            var requestUri = string.Format("{0}?{1}={2}", BaseRequestUri, DesignatedApiKeyQueryStringParameter, FakeApiKey);
            var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            var customApiKeyAuthHandler = new CustomApiKeyAuthHandler(DesignatedApiKeyQueryStringParameter);

            //Act
            return TestHelper.InvokeMessageHandler(request, customApiKeyAuthHandler)

                .ContinueWith(task => {

                    //Assert
                    Assert.Equal(TaskStatus.RanToCompletion, task.Status);
                    Assert.Equal(HttpStatusCode.OK, task.Result.StatusCode);
                });
        }

        [Fact, GCForce]
        public Task ApiKeyAuthenticationHandler_SetsCurrentThreadPrincipalWhenApiKeyIsValid() {

            //Arange
            var requestUri = string.Format("{0}?{1}={2}", BaseRequestUri, DesignatedApiKeyQueryStringParameter, FakeApiKey);
            var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
            var customApiKeyAuthHandler = new CustomApiKeyAuthHandler(DesignatedApiKeyQueryStringParameter);

            //Act
            return TestHelper.InvokeMessageHandler(request, customApiKeyAuthHandler)

                .ContinueWith(task => {

                    //Assert
                    Assert.Equal(TaskStatus.RanToCompletion, task.Status);
                    Assert.Equal(HttpStatusCode.OK, task.Result.StatusCode);
                    Assert.IsType<GenericPrincipal>(Thread.CurrentPrincipal);
                });
        }

        public class CustomApiKeyAuthHandler : ApiKeyAuthenticationHandler {

            public CustomApiKeyAuthHandler(string apiKeyQueryParameter) : base(apiKeyQueryParameter) { }

            protected override IPrincipal AuthenticateUser(string apiKey, HttpRequestMessage request, CancellationToken cancellationToken) {

                if (apiKey.Equals(FakeApiKey, StringComparison.OrdinalIgnoreCase)) {

                    var identity = new GenericIdentity("fakeuser");
                    return new GenericPrincipal(identity, null);
                }

                return null;
            }
        }
    }
}