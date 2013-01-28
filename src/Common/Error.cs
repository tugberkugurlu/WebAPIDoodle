using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace WebApiDoodle {

    /// <summary>
    /// Utility class for creating and unwrapping <see cref="Exception"/> instances.
    /// </summary>
    internal static class Error {

        internal static string Format(string format, params object[] args) {

            return String.Format(CultureInfo.CurrentCulture, format, args);
        }

        internal static ArgumentException Argument(string messageFormat, params object[] messageArgs) {

            return new ArgumentException(Error.Format(messageFormat, messageArgs));
        }

        internal static ArgumentException ArgumentNullOrEmpty(string parameterName) {

            return Error.Argument(parameterName, "The argument '{0}' is null or empty.", parameterName);
        }

        internal static ArgumentNullException PropertyNull() {

            return new ArgumentNullException("value");
        }

        internal static ArgumentNullException ArgumentNull(string parameterName) {

            return new ArgumentNullException(parameterName);
        }

        internal static ArgumentNullException ArgumentNull(string parameterName, string messageFormat, params object[] messageArgs) {

            return new ArgumentNullException(parameterName, Error.Format(messageFormat, messageArgs));
        }

        /// <summary>
        /// Creates an <see cref="InvalidOperationException"/>.
        /// </summary>
        /// <param name="messageFormat">A composite format string explaining the reason for the exception.</param>
        /// <param name="messageArgs">An object array that contains zero or more objects to format.</param>
        /// <returns>The logged <see cref="Exception"/>.</returns>
        internal static InvalidOperationException InvalidOperation(string messageFormat, params object[] messageArgs) {

            return new InvalidOperationException(Error.Format(messageFormat, messageArgs));
        }
    }
}