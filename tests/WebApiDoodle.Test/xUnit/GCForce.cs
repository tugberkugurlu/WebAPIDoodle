using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Xunit;

namespace WebApiDoodle.Web.Test {

    public class GCForce : BeforeAfterTestAttribute {

        public override void After(MethodInfo methodUnderTest) {

            GC.Collect(99);
            GC.Collect(99);
            GC.Collect(99);
        }
    }
}
