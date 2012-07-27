using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Hosting;
using System.Web.Http.Routing;

namespace WebAPIDoodle.Test {

    internal static class ContextUtil {

        internal static HttpControllerContext CreateControllerContext(IHttpController instance, string controllerName, Type controllerType, HttpConfiguration configuration = null, IHttpRouteData routeData = null, HttpRequestMessage request = null) {

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

        internal static HttpControllerDescriptor CreateControllerDescriptor(string controllerName, Type controllerType, HttpConfiguration config = null) {

            if (config == null) {

                config = new HttpConfiguration();
            }
            return new HttpControllerDescriptor(config, controllerName, controllerType);
        }
    }
}