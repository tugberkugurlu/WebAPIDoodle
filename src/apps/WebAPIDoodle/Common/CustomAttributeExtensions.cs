using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace WebApiDoodle.Web {
    
    internal static class CustomAttributeExtensions {

        internal static T GetAttribute<T>(this MemberInfo element) where T : Attribute {

            return (T)((object)element.GetAttribute(typeof(T)));
        }

        internal static Attribute GetAttribute(this MemberInfo element, Type attributeType) {

            return Attribute.GetCustomAttribute(element, attributeType);
        }

        public static IEnumerable<Attribute> GetCustomAttributes(this MemberInfo element) {

            return Attribute.GetCustomAttributes(element);
        }

        public static IEnumerable<Attribute> GetCustomAttributes(this ParameterInfo element) {

            return Attribute.GetCustomAttributes(element);
        }
    }
}
