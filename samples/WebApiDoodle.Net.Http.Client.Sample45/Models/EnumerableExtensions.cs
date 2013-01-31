using System.Collections.Generic;
using System.Linq;

namespace WebApiDoodle.Net.Http.Client.Sample45.Models {

    public static class EnumerableExtensions {

        public static PaginatedList<T> ToPaginatedList<T>(
            this IEnumerable<T> query, int pageIndex, int pageSize) {

            var totalCount = query.Count();
            var collection = query.Skip((pageIndex - 1) * pageSize).Take(pageSize);

            return new PaginatedList<T>(pageIndex, pageSize, totalCount, collection);
        }
    }
}