namespace FougeraClub.Areas.Admin.ViewModels.quote.Details
{
    public class ConfirmSupplierItemModel
    {
        public int ItemId { get; set; }
        public int SupplierId { get; set; }
        public string? SupplierName { get; set; }
        public int? Quantity { get; set; }
        public decimal? SinglePrice { get; set; }
    }
}
