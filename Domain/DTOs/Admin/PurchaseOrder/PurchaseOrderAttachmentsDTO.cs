namespace Domain.DTOs.Admin.PurchaseOrder
{
    public class PurchaseOrderAttachmentsDTO
    {
        public long PurchaseOrderId { get; set; }
        public List<PurchaseOrderAttachmentDTO> Attachments { get; set; } = new List<PurchaseOrderAttachmentDTO>();
    }
}
