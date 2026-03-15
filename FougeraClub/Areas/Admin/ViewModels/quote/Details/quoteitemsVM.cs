using Microsoft.AspNetCore.Mvc.Rendering;

namespace FougeraClub.Areas.Admin.ViewModels.quote.Details
{
    public class quoteitemsVM
    {
        public int Id { get; set; }
        public string? quoteCode { get; set; }
        public string? order { get; set; }
        public DateOnly? Date { get; set; }
        public bool IsConfirmed { get; set; }
        public string? textarea { get; set; }
        public List<SupplierAssignmentItemVM> Items { get; set; } = new();
        public List<SelectListItem> SuppliersList { get; set; } = new();

        public int quoteId { get; set; }
        public virtual quoteVM? quote { get; set; } = null!;


    }

    //public class ConfirmSingleSupplierModel
    //{
    //    public int QuoteId { get; set; }
    //    public int SupplierId { get; set; }
    //}

    //public class DeleteSupplierModel
    //{
    //    public int QuoteId { get; set; }
    //    public int SupplierId { get; set; }
    //}

    //public class DeleteSingleItemModel
    //{
    //    public int QuoteId { get; set; }
    //    public int ItemId { get; set; }
    //    public int SupplierId { get; set; }
    //}

    //public class UpdateItemPriceModel
    //{
    //    public int QuoteId { get; set; }
    //    public int ItemId { get; set; }
    //    public int SupplierId { get; set; }
    //    public decimal? NewPrice { get; set; }
    //}
}
