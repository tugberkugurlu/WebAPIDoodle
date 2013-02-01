using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebApiDoodle.Web.Versioning {

    internal class VersionedControllerIdentity {

        public VersionedControllerIdentity(string controllerName, int[] supportedVersions) {

            ControllerName = controllerName;
            SupportedVersions = supportedVersions;
        }

        public string Namespace { get; set; }
        public string ControllerName { get; private set; }
        public int[] SupportedVersions { get; private set; }
    }
}