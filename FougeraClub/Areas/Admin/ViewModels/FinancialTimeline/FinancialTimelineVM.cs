namespace KhaledTeamRecycling.Areas.Admin.ViewModels.FinancialTimeline
{
    public class FinancialTimelineVM
    {
        public IEnumerable<FinancialTimelineItemDTO> Items { get; set; } = new List<FinancialTimelineItemDTO>();
        public string? SearchString { get; set; }
        public DateOnly? DateFrom { get; set; }
        public DateOnly? DateTo { get; set; }
        public string? TransactionFilter { get; set; }
        public string? StatusFilter { get; set; }
        public string? SourceFilter { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 50;
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }
    }

    public class FinancialTimelineItemDTO
    {
        public int Id { get; set; }
        public string? SourceType { get; set; }
        public string? SourceName { get; set; }
        public string? TableType { get; set; }
        public string? OperationType { get; set; }
        public char? TypeTransaction { get; set; }
        public double? Amount { get; set; }
        public string? UserName { get; set; }
        public string? PartyType { get; set; }
        public string? ItemName { get; set; }
        public string? Notes { get; set; }
        public string? StatusName { get; set; }
        public string? StatusShortChar { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? SortDate { get; set; }
    }
}
