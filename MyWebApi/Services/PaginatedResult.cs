using System;
using System.Collections.Generic;

namespace MyWebApi.Services
{
    public class PaginatedResult<T>
    {
        public PaginatedResult(int pageIndex, int pageSize, IEnumerable<T> result, int totalCount)
        {
            Result = result;
            PageIndex = pageIndex;
            PageSize = pageSize;
            TotalCount = totalCount;
        }

        public IEnumerable<T> Result { get; private set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }

        public int TotalPageCount => (int)Math.Ceiling(TotalCount / (double)PageSize);

        public bool HasPreviousPage => PageIndex > 1;

        public bool HasNextPage => PageIndex < TotalPageCount;
    }
}