using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.CashDisbursementVoucher
{
    public class AcknowledgmentReceipt
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey(nameof(CashDisbursementVoucher))]
        public int CashDisbursementVoucherId { get; set; }
        public string? Title { get; set; }
        public string? ThisTo { get; set; }
        public string? Recived { get; set; }
        public int? PaymentVoucherNo { get; set; }
        public string? Amount { get; set; }
        public string? Cheque { get; set; }
        [Column(TypeName = "date")]
        public DateOnly? Date { get; set; }
        public string? Bank { get; set; }
        public string? Being { get; set; }
        [Column(TypeName = "date")]
        public DateOnly? HeaderDate { get; set; }
        public string? HeaderMonth { get; set; }
        public string? AcknowledgmentText { get; set; }
        public int? AccountantSignitureId { get; set; }
        [ForeignKey(nameof(AccountantSignitureId))]
        public virtual Signature? AccountantSigniture { get; set; }
    }
}
