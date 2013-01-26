using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;

namespace WebApiDoodle.Net.Http.Client {

    public class QueryStringCollection : ICollection<KeyValuePair<string, string>> {

        private readonly ICollection<KeyValuePair<string, string>> _pairs;

        public QueryStringCollection() {

            _pairs = new Collection<KeyValuePair<string, string>>();
        }

        public QueryStringCollection(ICollection<KeyValuePair<string, string>> pairs) {

            if (pairs == null) {
                throw new NullReferenceException("pairs");
            }

            _pairs = pairs;
        }

        public QueryStringCollection(object queryParameters) {

            if (queryParameters == null) {
                throw new NullReferenceException("queryParameters");
            }

            _pairs = new Collection<KeyValuePair<string, string>>();

            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(queryParameters);
            foreach (PropertyDescriptor propertyDescriptor in properties) {

                object value = propertyDescriptor.GetValue(queryParameters);
                Add(new KeyValuePair<string, string>(propertyDescriptor.Name, (value != null) ? value.ToString() : null));
            }
        }

        public override string ToString() {

            StringBuilder queryBuilder = new StringBuilder();
            foreach (var pair in _pairs) {
                queryBuilder.AppendFormat("{0}={1}&", pair.Key, pair.Value);
            }

            var query = queryBuilder.ToString();
            query = query.Substring(0, query.Length - 1);

            return query;
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator() {

            return _pairs.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {

            IEnumerable ie = _pairs;
            return ie.GetEnumerator();
        }

        public void Add(string key, string value) {

            Add(new KeyValuePair<string, string>(key, value));
        }

        public void Add(KeyValuePair<string, string> item) {

            _pairs.Add(new KeyValuePair<string, string>(item.Key, Uri.EscapeUriString(item.Value)));
        }

        public void Clear() {

            _pairs.Clear();
        }

        public bool Contains(KeyValuePair<string, string> item) {

            return _pairs.Contains(item);
        }

        public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex) {

            _pairs.CopyTo(array, arrayIndex);
        }

        public int Count {

            get { return _pairs.Count; }
        }

        public bool IsReadOnly {

            get { return _pairs.IsReadOnly; }
        }

        public bool Remove(KeyValuePair<string, string> item) {

            return _pairs.Remove(item);
        }
    }
}