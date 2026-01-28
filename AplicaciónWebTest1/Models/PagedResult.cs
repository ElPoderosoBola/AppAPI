using System.Collections.Generic;

namespace AplicaciónWebTest1.Models
{
    public class PagedResult<T>
    {
        public PagedResult(List<T> items, int total, int pageIndex, int pageSize)
        {
            Items = items;
            Total = total;
            PageIndex = pageIndex;
            PageSize = pageSize;
        }

        public List<T> Items { get; }
        public int Total { get; }
        public int PageIndex { get; }
        public int PageSize { get; }
        public int TotalPages => PageSize > 0 ? (int)System.Math.Ceiling((double)Total / PageSize) : 0;
    }
}
