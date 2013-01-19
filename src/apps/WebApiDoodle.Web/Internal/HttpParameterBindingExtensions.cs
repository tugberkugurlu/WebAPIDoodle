using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;
using System.Web.Http.ValueProviders;
using System.Web.Http.ValueProviders.Providers;

namespace WebApiDoodle.Web.Internal {

    internal static class HttpParameterBindingExtensions {

        internal static bool WillReadUri(this HttpParameterBinding parameterBinding) {

            if (parameterBinding == null) {

                throw Error.ArgumentNull("parameterBinding");
            }

            IValueProviderParameterBinding valueProviderParameterBinding = parameterBinding as IValueProviderParameterBinding;
            if (valueProviderParameterBinding != null) {

                IEnumerable<ValueProviderFactory> valueProviderFactories = valueProviderParameterBinding.ValueProviderFactories;

                // NOTE: IUriValueProviderFactory is an internal interface. So, instead of checing the factory
                // against it, we can check against the actual types as there are only two of them:
                //      QueryStringValueProviderFactory
                //      RouteDataValueProviderFactory
                if (valueProviderFactories.Any() && valueProviderFactories.All(factory =>
                    factory.GetType() == typeof(QueryStringValueProviderFactory) ||
                    factory.GetType() == typeof(RouteDataValueProviderFactory))) {

                    return true;
                }
            }

            return false;
        }
    }
}
