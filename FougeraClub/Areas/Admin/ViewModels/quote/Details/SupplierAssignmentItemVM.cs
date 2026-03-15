namespace FougeraClub.Areas.Admin.ViewModels.quote.Details
{
    public class SupplierAssignmentItemVM
    {
        public int ItemId { get; set; } // quoteItemId
        public string? ItemName { get; set; }
        public int? Quantity { get; set; }
        public int? SupplierId { get; set; }
        public string? SupplierName { get; set; }
        public decimal? SinglePrice { get; set; }
        public bool ItemSupplierIsConfirmed { get; set; }
    }
}
