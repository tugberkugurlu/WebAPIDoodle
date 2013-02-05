using System;
using System.Collections.ObjectModel;
using System.Net.Http.Formatting;

namespace WebApiDoodle.Net.Http.Client.Formatting {

    internal sealed class DefaultMediaTypeFormatterCollection : ReadOnlyCollection<MediaTypeFormatter> {

        private static readonly Lazy<DefaultMediaTypeFormatterCollection> lazy =
               new Lazy<DefaultMediaTypeFormatterCollection>(() => new DefaultMediaTypeFormatterCollection());

        public static DefaultMediaTypeFormatterCollection Instance { get { return lazy.Value; } }

        private DefaultMediaTypeFormatterCollection() 
            : base(new MediaTypeFormatterCollection()) {
        }
    }
}