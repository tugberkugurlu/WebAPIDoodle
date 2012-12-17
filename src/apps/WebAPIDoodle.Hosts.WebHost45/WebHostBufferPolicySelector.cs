using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Hosting;

namespace WebAPIDoodle.Hosts.WebHost45 {

    public class WebHostBufferPolicySelector : IHostBufferPolicySelector {

        public bool UseBufferedInputStream(object hostContext) {

            throw new NotImplementedException();
        }

        public bool UseBufferedOutputStream(HttpResponseMessage response) {

            throw new NotImplementedException();
        }
    }
}