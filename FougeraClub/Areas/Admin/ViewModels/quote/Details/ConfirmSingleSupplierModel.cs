namespace FougeraClub.Areas.Admin.ViewModels.quote.Details
{
    public class ConfirmSingleSupplierModel
    {
        public int QuoteId { get; set; }
        public int SupplierId { get; set; }
        public List<ConfirmSupplierItemModel> Items { get; set; } = new();
    }
}