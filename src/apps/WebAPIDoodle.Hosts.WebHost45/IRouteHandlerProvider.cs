using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Routing;

namespace WebAPIDoodle.Hosts.WebHost45 {
    
    public interface IRouteHandlerProvider {

        IRouteHandler GetRouteHandler();
    }
}