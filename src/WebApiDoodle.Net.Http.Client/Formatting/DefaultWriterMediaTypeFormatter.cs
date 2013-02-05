using System;
using System.Net.Http.Formatting;

namespace WebApiDoodle.Net.Http.Client.Formatting {
    
    internal sealed class DefaultWriterMediaTypeFormatter : JsonMediaTypeFormatter {

        private static readonly Lazy<DefaultWriterMediaTypeFormatter> lazy =
               new Lazy<DefaultWriterMediaTypeFormatter>(() => new DefaultWriterMediaTypeFormatter());

        public static DefaultWriterMediaTypeFormatter Instance { get { return lazy.Value; } }

        private DefaultWriterMediaTypeFormatter() {
        }
    }
}