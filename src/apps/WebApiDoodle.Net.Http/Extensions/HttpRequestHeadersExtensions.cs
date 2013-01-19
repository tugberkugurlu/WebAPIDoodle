using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace System.Net.Http.Headers {
 
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class HttpRequestHeadersExtensions {

        public static IEnumerable<KeyValuePair<string, string>> ParseToSingleValueKeyValuePairs(this HttpRequestHeaders headers) {

            return headers.Select(x =>
                new KeyValuePair<string, string>(
                    x.Key, x.Value.FirstOrDefault()));
        }
    }
}