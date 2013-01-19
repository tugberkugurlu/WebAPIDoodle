using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Compilation;
using System.Web.Http.Dispatcher;

namespace WebApiDoodle.Web.Hosting.WebHost45 {

    /// <summary>
    /// Provides an implementation of <see cref="IAssembliesResolver"/> using <see cref="BuildManager"/>.
    /// </summary>
    public class WebHostAssembliesResolver : IAssembliesResolver {

        /// <summary>
        /// Returns a list of controllers available for the application.
        /// </summary>
        /// <returns>An <see cref="ICollection{Assembly}" /> of controllers.</returns>
        public ICollection<Assembly> GetAssemblies() {

            return BuildManager.GetReferencedAssemblies().OfType<Assembly>().ToList();
        }
    }
}