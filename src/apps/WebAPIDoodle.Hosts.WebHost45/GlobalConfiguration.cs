using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Web.Http.Hosting;
using System.Web.Routing;
using WebAPIDoodle.Hosts.WebHost45.Routing;

namespace WebAPIDoodle.Hosts.WebHost45 {
    
    public static class GlobalConfiguration {

        private static Lazy<HttpConfiguration> _configuration = new Lazy<HttpConfiguration>(
            () => {
                HttpConfiguration config = new HttpConfiguration(new HostedHttpRouteCollection(RouteTable.Routes));
                config.Services.Replace(typeof(IAssembliesResolver), new WebHostAssembliesResolver());
                config.Services.Replace(typeof(IHttpControllerTypeResolver), new WebHostHttpControllerTypeResolver());
                config.Services.Replace(typeof(IHostBufferPolicySelector), new WebHostBufferPolicySelector());
                return config;
            });

        private static Lazy<HttpMessageHandler> _defaultHandler = new Lazy<HttpMessageHandler>(
            () => new HttpRoutingDispatcher(_configuration.Value));

        private static Lazy<IRouteHandler> _routeHandler = new Lazy<IRouteHandler>(
            () => {

                if (_configuration.Value.DependencyResolver != null) {
                    
                    var routeHandlerProvider = _configuration.Value.DependencyResolver.GetService(typeof(IRouteHandlerProvider));
                    if (routeHandlerProvider != null) {

                        return ((IRouteHandlerProvider)routeHandlerProvider).GetRouteHandler();
                    }
                }

                return new HttpControllerRouteHandler();

            }, isThreadSafe: true);

        /// <summary>
        /// Gets the global <see cref="T:System.Web.Http.HttpConfiguration"/>.
        /// </summary>
        public static HttpConfiguration Configuration {

            get { return _configuration.Value; }
        }

        /// <summary>
        /// Gets the default message handler that will be called for all requests.
        /// </summary>
        public static HttpMessageHandler DefaultMessageHandler {

            get { return _defaultHandler.Value; }
        }

        /// <summary>
        /// Gets the provided route handler that will be called for all requests.
        /// </summary>
        public static IRouteHandler DefaultRouteHandler {

            get { return _routeHandler.Value; }
        }
    }
}