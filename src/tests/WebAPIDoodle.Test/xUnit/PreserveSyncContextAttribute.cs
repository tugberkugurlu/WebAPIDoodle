using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using Xunit;

namespace WebAPIDoodle.Test {
    
    public class PreserveSyncContextAttribute : BeforeAfterTestAttribute {

        private SynchronizationContext _syncContext;

        public override void Before(MethodInfo methodUnderTest) {

            _syncContext = SynchronizationContext.Current;
        }

        public override void After(MethodInfo methodUnderTest) {

            SynchronizationContext.SetSynchronizationContext(_syncContext);
        }
    }
}
