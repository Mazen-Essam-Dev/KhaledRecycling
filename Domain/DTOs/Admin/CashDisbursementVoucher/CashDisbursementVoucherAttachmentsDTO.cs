namespace Domain.DTOs.Admin.CashDisbursementVoucher
{
    public class CashDisbursementVoucherAttachmentsDTO
    {
        public int CashDisbursementVoucherId { get; set; }
        public List<CashDisbursementVoucherAttachmentDTO>? Attachments { get; set; }
    }
}
