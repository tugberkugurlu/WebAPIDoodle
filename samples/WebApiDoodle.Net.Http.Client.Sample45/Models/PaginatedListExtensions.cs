using System.Collections.Generic;
using WebApiDoodle.Net.Http.Client.Model;
using System.Linq;

namespace WebApiDoodle.Net.Http.Client.Sample45.Models {

    internal static class PaginatedListExtensions {

        internal static PaginatedDto<TDto> ToPaginatedDto<TDto>(
            this PaginatedList<TDto> source) where TDto : IDto {

            return new PaginatedDto<TDto> {
                PageIndex = source.PageIndex,
                PageSize = source.PageSize,
                TotalCount = source.TotalCount,
                TotalPageCount = source.TotalPageCount,
                HasNextPage = source.HasNextPage,
                HasPreviousPage = source.HasPreviousPage,
                Items = source.AsEnumerable()
            };
        }
    }
}