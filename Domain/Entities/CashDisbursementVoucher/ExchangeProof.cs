using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.CashDisbursementVoucher
{
    public class ExchangeProof
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey(nameof(CashDisbursementVoucher))]
        public int CashDisbursementVoucherId { get; set; }
        public virtual CashDisbursementVoucher? CashDisbursementVoucher { get; set; }
        public int? DocumentNo { get; set; }
        public string? BasedOn { get; set; }
        public string? ItWas { get; set; }
        public string? Amount { get; set; }
        public string? By { get; set; }
        public string? Bank { get; set; }
        [Column(TypeName = "date")]
        public DateOnly? Date { get; set; }
        public string? Being { get; set; }
        public int? AccountantSignitureId { get; set; }
        [ForeignKey(nameof(AccountantSignitureId))]
        public virtual Signature? AccountantSigniture { get; set; }
        public int? ManagerSignitureId { get; set; }
        [ForeignKey(nameof(ManagerSignitureId))]
        public virtual Signature? ManagerSigniture { get; set; }

        public virtual ICollection<ExchangeProofDetail>? Details { get; set; }

    }
}
