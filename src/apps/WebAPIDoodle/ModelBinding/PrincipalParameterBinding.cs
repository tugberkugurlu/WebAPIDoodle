using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Metadata;

namespace WebAPIDoodle.ModelBinding {

    public class PrincipalParameterBinding : HttpParameterBinding {

        public PrincipalParameterBinding(HttpParameterDescriptor descriptor) 
            : base(descriptor) { 
        }

        public override Task ExecuteBindingAsync(ModelMetadataProvider metadataProvider, HttpActionContext actionContext, CancellationToken cancellationToken) {

            string name = Descriptor.ParameterName;
            IPrincipal principal = Thread.CurrentPrincipal;
            actionContext.ActionArguments.Add(name, principal);

            return TaskHelpers.Completed();
        }
    }
}
