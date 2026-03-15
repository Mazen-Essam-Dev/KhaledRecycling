namespace FougeraClub.Areas.Admin.ViewModels.CashDisbursementVoucher
{
    public class CashDisbursementVoucherAttachmentsVM
    {
        public int CashDisbursementVoucherId { get; set; }
        public List<CashDisbursementVoucherAttachmentVM>? Attachments { get; set; }
    }
}
