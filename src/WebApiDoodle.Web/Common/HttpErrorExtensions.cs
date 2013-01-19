using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Http;

namespace System.Web.Http {
    
    internal static class HttpErrorExtensions {

        internal static HttpError AddAndReturn(this HttpError httpError, string key, object value) {
            
            httpError.Add(key, value);
            return httpError;
        }
    }
}
