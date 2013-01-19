using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace WebApiDoodle.Net.Http {

    public class EmptyContent : StringContent {

        public EmptyContent() : base(string.Empty) { }
    }
}