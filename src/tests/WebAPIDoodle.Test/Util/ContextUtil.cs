using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using System.Web.Http.Hosting;
using System.Web.Http.Routing;

namespace System.Web.Http {

    internal static class ContextUtil {

        public static HttpControllerContext CreateControllerContext(IHttpController instance, string controllerName, Type controllerType, HttpConfiguration configuration = null, IHttpRouteData routeData = null, HttpRequestMessage request = null) {

            HttpConfiguration config = configuration ?? new HttpConfiguration();
            IHttpRouteData route = routeData ?? new HttpRouteData(new HttpRoute());
            HttpRequestMessage req = request ?? new HttpRequestMessage();
            req.Properties[HttpPropertyKeys.HttpConfigurationKey] = config;
            req.Properties[HttpPropertyKeys.HttpRouteDataKey] = route;

            
            HttpControllerContext context = new HttpControllerContext(config, route, req);
            context.Controller = instance;
            context.ControllerDescriptor = CreateControllerDescriptor(controllerName, controllerType, config);

            return context;
        }

        public static HttpControllerDescriptor CreateControllerDescriptor(string controllerName, Type controllerType, HttpConfiguration config = null) {

            if (config == null) {

                config = new HttpConfiguration();
            }
            return new HttpControllerDescriptor(config, controllerName, controllerType);
        }

        public static HttpControllerDescriptor CreateControllerDescriptor(HttpConfiguration configuration = null) {

            HttpConfiguration config = configuration ?? new HttpConfiguration();
            HttpControllerDescriptor controllerDescriptor = new HttpControllerDescriptor();
            controllerDescriptor.Configuration = configuration;

            return controllerDescriptor;
        }

        public static HttpControllerContext CreateControllerContext(
            HttpConfiguration configuration = null, IHttpController controller = null, IHttpRouteData routeData = null, HttpRequestMessage request = null) {

            HttpConfiguration config = configuration ?? new HttpConfiguration();
            IHttpRouteData route = routeData ?? new HttpRouteData(new HttpRoute());
            HttpRequestMessage req = request ?? new HttpRequestMessage();
            req.Properties[HttpPropertyKeys.HttpConfigurationKey] = config;
            req.Properties[HttpPropertyKeys.HttpRouteDataKey] = route;

            HttpControllerContext context = new HttpControllerContext(config, route, req);
            if (controller != null) {
                context.Controller = controller;
            }
            context.ControllerDescriptor = CreateControllerDescriptor(config);

            return context;
        }

        public static HttpActionContext CreateActionContext(
            HttpControllerContext controllerContext = null, HttpActionDescriptor actionDescriptor = null) {

            HttpControllerContext controllerCtx = controllerContext ?? CreateControllerContext();
            HttpActionDescriptor descriptor = actionDescriptor ?? new Mock<HttpActionDescriptor>() { CallBase = true }.Object;

            return new HttpActionContext(controllerCtx, descriptor);
        }

        public static HttpActionContext GetHttpActionContext(HttpRequestMessage request) {

            HttpActionContext actionContext = CreateActionContext();
            actionContext.ControllerContext.Request = request;

            return actionContext;
        }

        public static HttpActionExecutedContext GetActionExecutedContext(HttpRequestMessage request, HttpResponseMessage response) {

            HttpActionContext actionContext = CreateActionContext();
            actionContext.ControllerContext.Request = request;
            HttpActionExecutedContext actionExecutedContext = new HttpActionExecutedContext(actionContext, null) { Response = response };

            return actionExecutedContext;
        }
    }
}