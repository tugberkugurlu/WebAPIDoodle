using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WebAPIDoodle.Controllers {

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class UriParametersAttribute : Attribute {

        private static readonly string[] _emptyArray = new string[0];
        private readonly string[] _paramsArray;

        public UriParametersAttribute(params string[] parameters) {

            _paramsArray = parameters;
        }

        public string[] Parameters {
            get {
                return _paramsArray;
            }
        }
    }
}
