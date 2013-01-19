using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using WebApiDoodle.Web.Http;
using Xunit;

namespace WebApiDoodle.Web.Test.MessageHandlers {

    public class RequireHttpsMessageHandlerTest {

        [GCForce, Fact]
        public Task RequireHttpsMessageHandler_ReturnsForbidden403StatusCodeWhenTheRequestIsNotOverHTTPS() { 
            
            //Arange
            var request = new HttpRequestMessage(HttpMethod.Get, "http://localhost:8080");

            //Act
            return TestHelper.InvokeMessageHandler(request, new RequireHttpsMessageHandler())
                
                .ContinueWith(task => {

                    //Assert
                    Assert.Equal(TaskStatus.RanToCompletion, task.Status);
                    Assert.Equal(HttpStatusCode.Forbidden, task.Result.StatusCode);
                });
        }

        [GCForce, Fact]
        public Task RequireHttpsMessageHandler_ReturnsDelegatedStatusCodeWhenTheRequestIsOverHTTPS() {

            //Arange
            var request = new HttpRequestMessage(HttpMethod.Get, "https://localhost:8080");

            //Act
            return TestHelper.InvokeMessageHandler(request, new RequireHttpsMessageHandler())

                .ContinueWith(task => {

                    //Assert
                    Assert.Equal(TaskStatus.RanToCompletion, task.Status);
                    Assert.Equal(HttpStatusCode.OK, task.Result.StatusCode);
                });
        }
    }
}