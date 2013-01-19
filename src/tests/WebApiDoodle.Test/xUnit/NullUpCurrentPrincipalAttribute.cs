using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using Xunit;

namespace WebApiDoodle.Web.Test {
    
    public class NullUpCurrentPrincipalAttribute : BeforeAfterTestAttribute {

        public override void Before(MethodInfo methodUnderTest) {

            Thread.CurrentPrincipal = null;
        }
    }
}