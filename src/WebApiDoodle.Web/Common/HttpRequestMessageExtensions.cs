using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace System.Net.Http {

    public static class HttpRequestMessageExtensions {

        internal static HttpResponseMessage CreateErrorResponse(this HttpRequestMessage request, HttpStatusCode statusCode, string message, string messageDetail) {

            HttpError httpError = new HttpError(message);
            return request.CreateErrorResponse(statusCode, includeErrorDetail => includeErrorDetail ? httpError.AddAndReturn(HttpErrorConstants.MessageDetailKey, messageDetail) : httpError);
        }

        private static HttpResponseMessage CreateErrorResponse(this HttpRequestMessage request, HttpStatusCode statusCode, Func<bool, HttpError> errorCreator) {

            HttpConfiguration configuration = request.GetConfiguration();

            // CreateErrorResponse should never fail, even if there is no configuration associated with the request
            // In that case, use the default HttpConfiguration to con-neg the response media type
            if (configuration == null) {

                using (HttpConfiguration defaultConfig = new HttpConfiguration()) {

                    HttpError error = errorCreator(defaultConfig.ShouldIncludeErrorDetail(request));
                    return request.CreateResponse<HttpError>(statusCode, error, defaultConfig);
                }
            }
            else {

                HttpError error = errorCreator(configuration.ShouldIncludeErrorDetail(request));
                return request.CreateResponse<HttpError>(statusCode, error, configuration);
            }
        }
    }
}
