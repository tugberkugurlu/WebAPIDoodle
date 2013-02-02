using System.Net.Http;
using System.Web;

namespace WebApiDoodle.Web.WebHostEx {

    internal static class HttpRequestMessageExtensions {

        internal static HttpContextBase GetHttpContext(this HttpRequestMessage request) {

            return request.GetProperty<HttpContextBase>(HttpWebHostPropertyKeys.HttpContextBaseKey);
        }

        internal static string GetUserHostAddress(this HttpRequestMessage request) {

            return request.GetHttpContext().Request.UserHostAddress;
        }
    }
}