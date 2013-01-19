using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Dispatcher;

namespace WebApiDoodle.Web.Hosting.WebHost45 {
    
    public class WebHostHttpControllerTypeResolver : IHttpControllerTypeResolver {

        public ICollection<Type> GetControllerTypes(IAssembliesResolver assembliesResolver) {

            throw new NotImplementedException();
        }
    }
}