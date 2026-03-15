namespace FougeraClub.Areas.Admin.ViewModels.quote.Details
{
    public class ConfirmSupplierAssignmentModel
    {
        public int QuoteId { get; set; }
        public List<ConfirmSupplierItemModel> Items { get; set; } = new();
    }
}
