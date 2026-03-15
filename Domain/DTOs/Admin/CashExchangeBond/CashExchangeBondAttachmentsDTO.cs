namespace Domain.DTOs.Admin.CashExchangeBond
{
    public class CashExchangeBondAttachmentsDTO
    {
        public int CashExchangeBondId { get; set; }
        public List<CashExchangeBondAttachmentDTO>? Attachments { get; set; }
    }
}
