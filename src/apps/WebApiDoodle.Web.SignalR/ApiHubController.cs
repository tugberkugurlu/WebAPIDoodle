using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Http;
using SignalR;
using SignalR.Hubs;

namespace WebApiDoodle.Web.SignalR {

    public abstract class ApiHubController<THub> : ApiController where THub : Hub {

        Lazy<IHubContext> _hub = new Lazy<IHubContext>(
            () => GlobalHost.ConnectionManager.GetHubContext<THub>()
        );

        protected IHubContext Hub {
            get { 
                return _hub.Value; 
            }
        }
    }
}
