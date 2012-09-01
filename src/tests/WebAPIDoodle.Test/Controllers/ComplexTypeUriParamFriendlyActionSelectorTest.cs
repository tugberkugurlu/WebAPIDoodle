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

namespace WebAPIDoodle.Test.Controllers {
    
    public class ComplexTypeUriParamFriendlyActionSelectorTest {

        [Fact]
        public void ComplexTypeUriParamFriendlyActionSelector_SelectAction_With_DifferentExecutionContexts() {

            ComplexTypeUriParamFriendlyActionSelector actionSelector = new ComplexTypeUriParamFriendlyActionSelector();
            HttpControllerContext GetControllerContext = ContextUtil.CreateControllerContext();
            HttpControllerDescriptor usersControllerDescriptor = new HttpControllerDescriptor(GetControllerContext.Configuration, "Users", typeof(UsersController));
            usersControllerDescriptor.Configuration.Services.Replace(typeof(IHttpActionSelector), actionSelector);
            GetControllerContext.ControllerDescriptor = usersControllerDescriptor;
            GetControllerContext.Request = new HttpRequestMessage { 
                Method = HttpMethod.Get
            };

            HttpControllerContext PostControllerContext = ContextUtil.CreateControllerContext();
            usersControllerDescriptor.Configuration.Services.Replace(typeof(IHttpActionSelector), actionSelector);
            PostControllerContext.ControllerDescriptor = usersControllerDescriptor;
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

            ComplexTypeUriParamFriendlyActionSelector actionSelector = new ComplexTypeUriParamFriendlyActionSelector();
            Assert.Throws<ArgumentNullException>(() => actionSelector.SelectAction(null));
        }
    }
}