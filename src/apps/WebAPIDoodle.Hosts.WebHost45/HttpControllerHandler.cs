using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Hosting;

namespace WebAPIDoodle.Hosts.WebHost45 {
    
    public class HttpControllerHandler : HttpTaskAsyncHandler {

        internal const string HttpContextBaseKey = "MS_HttpContext";

        private static readonly Lazy<HttpMessageInvoker> _server = 
            new Lazy<HttpMessageInvoker>(() => {
                HttpServer server = new HttpServer(GlobalConfiguration.Configuration, GlobalConfiguration.DefaultMessageHandler);
                return new HttpMessageInvoker(server);
            });

        private static readonly Lazy<IHostBufferPolicySelector> _bufferPolicySelector = 
            new Lazy<IHostBufferPolicySelector>(() =>
                GlobalConfiguration.Configuration.Services.GetHostBufferPolicySelector());

        public override Task ProcessRequestAsync(HttpContext context) {

            CancellationTokenSource cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(
                context.Request.TimedOutToken, 
                context.Response.ClientDisconnectedToken);

            CancellationToken cancellationToken = cancellationTokenSource.Token;

            throw new NotImplementedException();
        }

        public override bool IsReusable {
            get {
                return false;
            }
        }

        // private helpers
    }
}