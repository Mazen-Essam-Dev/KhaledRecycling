namespace FougeraClub.Helpers
{
    public class PaginatedList<T>
    {
        public List<T> Items { get; private set; }
        public int PageIndex { get; private set; }
        public int PageSize { get; private set; }
        public int TotalPages { get; private set; }
        public string? SearchTerm { get; private set; }
        public string? dateTo { get; set; }
        public string? dateFrom { get; set; }
        public int Count { get; private set; }
        public int TotalCount { get; private set; }
        public bool IsUserLogedInISManager { get; set; } = false;
        public bool IsAbleToOpen { get; set; } = false;
        public bool flag1 { get; set; } = false;

        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize, string? searchTerm = null, int totalCount = 0)
        {
            Items = items;
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            SearchTerm = searchTerm;
            PageSize = pageSize;
            Count = count;
            TotalCount = totalCount;
        }

        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;

        public static PaginatedList<T> Create(IQueryable<T> source, int pageIndex, int pageSize, string? searchTerm = null, int totalCount = 0)
        {
            var count = source.Count(); // Replace with CountAsync if EF
            var items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            return new PaginatedList<T>(items, count, pageIndex, pageSize, searchTerm, totalCount);
        }
        public static PaginatedList<T> Create(List<T> source, int pageIndex, int pageSize, string? searchTerm = null,int totalCount=0)
        {
            var count = source.Count(); // Replace with CountAsync if EF
            var items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            return new PaginatedList<T>(items, count, pageIndex, pageSize, searchTerm, totalCount);
        }
    }

}
