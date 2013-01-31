using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace WebApiDoodle.Net.Http.Client.Sample45.Clients.Core {

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class ApiClientContextExtensions {

        public static ICarsClient GetCarsClient(this ApiClientContext apiClientContext) {

            return apiClientContext.GetClient<ICarsClient>(() => new CarsClient(apiClientContext.HttpClient));
        }

        internal static TClient GetClient<TClient>(this ApiClientContext apiClientContext, Func<TClient> valueFactory) {

            return (TClient)apiClientContext.Clients.GetOrAdd(typeof(TClient), k => valueFactory());
        }
    }
}