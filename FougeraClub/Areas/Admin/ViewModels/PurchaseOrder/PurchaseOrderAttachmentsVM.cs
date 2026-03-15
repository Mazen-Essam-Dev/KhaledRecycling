namespace FougeraClub.Areas.Admin.ViewModels.PurchaseOrder
{
    public class PurchaseOrderAttachmentsVM
    {
        public long PurchaseOrderId { get; set; }
        public List<PurchaseOrderAttachmentVM> Attachments { get; set; } = new List<PurchaseOrderAttachmentVM>();
    }
}
