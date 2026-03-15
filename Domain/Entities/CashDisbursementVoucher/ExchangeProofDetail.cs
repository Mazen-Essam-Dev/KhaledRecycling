using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.CashDisbursementVoucher
{
    public class ExchangeProofDetail
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(ExchangeProof))]
        public int ExchangeProofId { get; set; }
        public virtual ExchangeProof? ExchangeProof { get; set; }

        public string? AccountNo { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? CurrentBalance { get; set; }
        public string? Item { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Amount { get; set; }
        public string? PaymentType { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Balance { get; set; }
    }
}
