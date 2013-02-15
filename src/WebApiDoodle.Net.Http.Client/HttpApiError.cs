using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace WebApiDoodle.Net.Http.Client {

    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "This type is only a dictionary to get the right serialization format")]
    [SuppressMessage("Microsoft.Usage", "CA2237:MarkISerializableTypesWithSerializable", Justification = "DCS does not support IXmlSerializable types that are also marked as [Serializable]")]
    [XmlRoot("Error")]
    public class HttpApiError : Dictionary<string, object>, IXmlSerializable {

        private const string MessageKey = "Message";
        private const string MessageDetailKey = "MessageDetail";
        private const string ModelStateKey = "ModelState";
        private const string ExceptionMessageKey = "ExceptionMessage";
        private const string ExceptionTypeKey = "ExceptionType";
        private const string StackTraceKey = "StackTrace";
        private const string InnerExceptionKey = "InnerException";

        public string Message {
            get { return GetPropertyValue<String>(MessageKey); }
            set { this[MessageKey] = value; }
        }

        // NOTE: Couldn't make it work in a formatter agnostic way.
        //public HttpApiError ModelState {
        //    get { return GetPropertyValue<HttpApiError>(ModelStateKey); }
        //}

        public string MessageDetail {
            get { return GetPropertyValue<String>(MessageDetailKey); }
            set { this[MessageDetailKey] = value; }
        }

        public string ExceptionMessage {
            get { return GetPropertyValue<String>(ExceptionMessageKey); }
            set { this[ExceptionMessageKey] = value; }
        }

        public string ExceptionType {
            get { return GetPropertyValue<String>(ExceptionTypeKey); }
            set { this[ExceptionTypeKey] = value; }
        }

        public string StackTrace {
            get { return GetPropertyValue<String>(StackTraceKey); }
            set { this[StackTraceKey] = value; }
        }

        public HttpApiError InnerException {
            get { return GetPropertyValue<HttpApiError>(InnerExceptionKey); }
        }

        public TValue GetPropertyValue<TValue>(string key) {

            TValue value;
            if (this.TryGetValue(key, out value)) {
                return value;
            }
            return default(TValue);
        }

        XmlSchema IXmlSerializable.GetSchema() {
            return null;
        }

        void IXmlSerializable.ReadXml(XmlReader reader) {

            if (reader.IsEmptyElement) {
                reader.Read();
                return;
            }

            reader.ReadStartElement();

            while (reader.NodeType != System.Xml.XmlNodeType.EndElement) {
                string key = XmlConvert.DecodeName(reader.LocalName);
                string value = reader.ReadInnerXml();

                this.Add(key, value);
                reader.MoveToContent();
            }

            reader.ReadEndElement();
        }

        void IXmlSerializable.WriteXml(XmlWriter writer) {

            foreach (KeyValuePair<string, object> keyValuePair in this) {

                string key = keyValuePair.Key;
                object value = keyValuePair.Value;
                writer.WriteStartElement(XmlConvert.EncodeLocalName(key));
                if (value != null) {

                    HttpApiError innerError = value as HttpApiError;
                    if (innerError == null) {
                        writer.WriteValue(value);
                    }
                    else {
                        ((IXmlSerializable)innerError).WriteXml(writer);
                    }
                }
                writer.WriteEndElement();
            }
        }
    }
}