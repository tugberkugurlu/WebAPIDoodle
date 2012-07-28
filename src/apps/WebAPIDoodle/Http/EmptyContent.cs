using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace WebAPIDoodle.Http {

    public class EmptyContent : StringContent {

        public EmptyContent() : base(string.Empty) { }
    }
}