using Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.CashDisbursementVoucher
{
    public class CashDisbursementVoucher
    {
        [Key]
        public int Id { get; set; }

        public CashDisbursementVoucherType? Type { get; set; }

        [Column(TypeName = "date")]
        public DateOnly? Date { get; set; }

        public string? Month { get; set; }

        public int? DocumentNo { get; set; }

        public string? Description { get; set; }

        public decimal? TotalAmount { get; set; }

        public virtual ICollection<CashDisbursementVoucherDetail>? Details { get; set; }
        
        public virtual ICollection<CashDisbursementVoucherAttachment>? CashDisbursementVoucherAttachments { get; set; }

        // --- Foreign keys stored in CashDisbursementVoucher table ---

        //Accountant
        public int? DisbursementRequestSignatureAccountantId { get; set; }
        [ForeignKey(nameof(DisbursementRequestSignatureAccountantId))]
        public Signature? DisbursementRequestAccountantSignature { get; set; }

        //Manager
        public int? DisbursementRequestSignatureId { get; set; }
        [ForeignKey(nameof(DisbursementRequestSignatureId))]
        public virtual Signature? DisbursementRequestSignature { get; set; }

  
        public int? AcknowledgmentReceipt_A_Sig_Id { get; set; }
        [ForeignKey(nameof(AcknowledgmentReceipt_A_Sig_Id))]
        public virtual Signature? AcknowledgmentReceipt_A_Sig { get; set; }
    }
}
