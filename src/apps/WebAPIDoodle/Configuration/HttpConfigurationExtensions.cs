using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Configuration;
using System.Web.Http;
using System.Configuration;

namespace WebAPIDoodle.Configuration {

    public static class HttpConfigurationExtensions {

        private static readonly IDictionary<CustomErrorsMode, IncludeErrorDetailPolicy> policyLookup = 
            new Dictionary<CustomErrorsMode, IncludeErrorDetailPolicy> {
                { CustomErrorsMode.RemoteOnly, IncludeErrorDetailPolicy.LocalOnly },
                { CustomErrorsMode.On, IncludeErrorDetailPolicy.Never },
                { CustomErrorsMode.Off, IncludeErrorDetailPolicy.Always },
            };

        //from WebApiContrib: https://github.com/WebApiContrib/WebAPIContrib/blob/master/src/WebApiContrib/Configuration/ConfigurationExtensions.cs
        public static void UseWebConfigCustomErrors(this HttpConfiguration configuration) {

            var config = (CustomErrorsSection)ConfigurationManager.GetSection("system.web/customErrors");
            configuration.IncludeErrorDetailPolicy = policyLookup[config.Mode];
        }

    }
}
