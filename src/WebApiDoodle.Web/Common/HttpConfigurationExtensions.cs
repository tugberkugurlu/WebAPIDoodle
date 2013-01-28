using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Http.Hosting;

namespace WebApiDoodle.Web {

    internal static class HttpConfigurationExtensions {

        internal static bool ShouldIncludeErrorDetail(this HttpConfiguration config, HttpRequestMessage request) {

            switch (config.IncludeErrorDetailPolicy) {

                case IncludeErrorDetailPolicy.Default:

                    Lazy<bool> includeErrorDetail;

                    if (request.Properties.TryGetValue<Lazy<bool>>(HttpPropertyKeys.IncludeErrorDetailKey, out includeErrorDetail)) {

                        // If we are on webhost and the user hasn't changed the IncludeErrorDetailPolicy
                        // look up into the ASP.NET CustomErrors property else default to LocalOnly.
                        return includeErrorDetail.Value;
                    }

                    goto case IncludeErrorDetailPolicy.LocalOnly;

                case IncludeErrorDetailPolicy.LocalOnly:

                    if (request == null) {
                        return false;
                    }

                    Lazy<bool> isLocal;
                    if (request.Properties.TryGetValue<Lazy<bool>>(HttpPropertyKeys.IsLocalKey, out isLocal)) {
                        return isLocal.Value;
                    }
                    return false;

                case IncludeErrorDetailPolicy.Always:
                    return true;

                case IncludeErrorDetailPolicy.Never:
                    return false;

                default:
                    return false;
            }
        }
    }
}