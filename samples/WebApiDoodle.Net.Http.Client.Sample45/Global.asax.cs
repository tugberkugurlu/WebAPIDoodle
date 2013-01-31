using Autofac;
using Autofac.Integration.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using System.Web.Security;
using System.Web.SessionState;
using WebApiDoodle.Net.Http.Client.Sample45.Clients;
using WebApiDoodle.Net.Http.Client.Sample45.Clients.Core;
using WebApiDoodle.Net.Http.Client.Sample45.MessageHandlers;
using WebApiDoodle.Net.Http.Client.Sample45.RequestCommands;

namespace WebApiDoodle.Net.Http.Client.Sample45 {

    public class Global : HttpApplication {

        protected void Application_Start(object sender, EventArgs e) {

            HttpConfiguration config = GlobalConfiguration.Configuration;
            HttpMessageHandler serverPipeline = HttpClientFactory.CreatePipeline(new HttpControllerDispatcher(config), new[] { new AuthMessageHandler() });

            config.Routes.MapHttpRoute(
                "ServerHttpRoute",
                "api/cars/{id}",
                defaults: new { id = RouteParameter.Optional, controller = "cars" },
                constraints: null,
                handler: serverPipeline
            );

            config.Routes.MapHttpRoute(
                "ClientHttpRoute",
                "client/cars/{id}",
                new { id = RouteParameter.Optional, controller = "carsclient" }
            );

            RegisterDependencies(config);

            // Any complex type parameter which is Assignable From 
            // IRequestCommand will be bound from the URI
            config.ParameterBindingRules.Insert(0, descriptor =>
                typeof(IRequestCommand).IsAssignableFrom(descriptor.ParameterType)
                    ? new FromUriAttribute().GetBinding(descriptor) : null);
        }

        private static void RegisterDependencies(HttpConfiguration config) {

            ContainerBuilder builder = new ContainerBuilder();
            ApiClientContext apiClientContex = ApiClientContext.Create(cfg => 
                cfg.ConnectTo("http://localhost:18132")
                   .SetCredentialsFromAppSetting("Api:UserName", "Api:Password"));

            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            builder.Register(c => apiClientContex.GetCarsClient()).As<ICarsClient>().InstancePerApiRequest();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(builder.Build());
        }
    }
}