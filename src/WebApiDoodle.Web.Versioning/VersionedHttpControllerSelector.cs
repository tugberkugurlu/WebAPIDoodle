using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using System.Web.Http.Routing;

namespace WebApiDoodle.Web.Versioning {

    /// <summary>
    /// An implementation of <see cref="System.Web.Http.Dispatcher.IHttpControllerSelector"/> 
    /// for choosing a <see cref="System.Web.Http.Controllers.HttpControllerDescriptor"/> given a <see cref="System.Net.Http.HttpRequestMessage"/>
    /// based on the supplied versioning criteria.
    /// </summary>
    public class VersionedHttpControllerSelector : IHttpControllerSelector {

        // NOTE: 1-) This will carry all the features of DefaultHttpControllerSelector.
        //           So, a lot of cloning for System.Web.Http 
        //       2-) We will look for {specifier}{\d+} and get the {\d+} part.
        //           We will use this to identify the version

        public static readonly string ControllerSuffix = "Controller";
        private const string ControllerKey = "controller";
        private readonly HttpConfiguration _configuration;
        private readonly HttpControllerTypeCache _controllerTypeCache;
        private readonly Lazy<ConcurrentDictionary<VersionedControllerIdentity, HttpControllerDescriptor>> _controllerInfoCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="VersionedHttpControllerSelector"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public VersionedHttpControllerSelector(HttpConfiguration configuration) {

            if (configuration == null) {
                throw new ArgumentNullException("configuration");
            }

            _controllerInfoCache = new Lazy<ConcurrentDictionary<VersionedControllerIdentity, HttpControllerDescriptor>>(InitializeControllerInfoCache);
            _configuration = configuration;
            _controllerTypeCache = new HttpControllerTypeCache(_configuration);
        }

        public HttpControllerDescriptor SelectController(HttpRequestMessage request) {

            if (request == null) {

                throw Error.ArgumentNull("request");
            }

            throw new NotImplementedException();
        }

        public IDictionary<string, HttpControllerDescriptor> GetControllerMapping() {

            throw new NotImplementedException();
        }

        // private helpers

        private ConcurrentDictionary<VersionedControllerIdentity, HttpControllerDescriptor> InitializeControllerInfoCache() {

            var result = new ConcurrentDictionary<VersionedControllerIdentity, HttpControllerDescriptor>();
            var duplicateControllers = new HashSet<string>();
            Dictionary<string, ILookup<string, Type>> controllerTypeGroups = _controllerTypeCache.Cache;

            throw new NotImplementedException();
        }

        private string GetControllerName(HttpRequestMessage request) {

            IHttpRouteData routeData = request.GetRouteData();
            if (routeData == null) {

                return null;
            }

            // Look up controller in route data
            string controllerName = null;
            routeData.Values.TryGetValue(ControllerKey, out controllerName);
            return controllerName;
        }

        private string GetVersionNumber(HttpRequestMessage request) {

            return null;
        }
    }
}