using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.Reflection {
    
    internal static class CustomAttributeExtensions {

        internal static T GetCustomAttribute<T>(this MemberInfo element) where T : Attribute {

            return (T)((object)element.GetCustomAttribute(typeof(T)));
        }

        internal static Attribute GetCustomAttribute(this MemberInfo element, Type attributeType) {

            return Attribute.GetCustomAttribute(element, attributeType);
        }

        public static bool IsDefined(this MemberInfo element, Type attributeType) {

            return Attribute.IsDefined(element, attributeType);
        }
    }
}
