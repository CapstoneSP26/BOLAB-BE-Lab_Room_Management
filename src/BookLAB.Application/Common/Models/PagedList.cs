using Microsoft.EntityFrameworkCore;

namespace BookLAB.Application.Common.Models
{
    public class PagedList<T>
    {
        public List<T> Items { get; }
        public int PageNumber { get; }
        public int TotalPages { get; }
        public int TotalCount { get; }
        public int PageSize { get; }
        public bool HasPreviousPage => PageNumber > 1;
        public bool HasNextPage => PageNumber < TotalPages;

        public PagedList(List<T> items, int count, int pageNumber, int pageSize)
        {
            PageNumber = Math.Max(1, pageNumber);
            PageSize = Math.Max(1, pageSize);
            TotalPages = Math.Max(1, (int)Math.Ceiling(count / (double)PageSize));
            TotalCount = count;
            Items = items;
        }

        // Helper method để tạo PagedList nhanh từ IQueryable
        public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize, CancellationToken ct, bool countItems = false)
        {
            int count = 0;
            if (countItems)
            {
                count = await source.CountAsync(ct);
            }
            
            var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(ct);
            return new PagedList<T>(items, count, pageNumber, pageSize);
        }
    }
}
