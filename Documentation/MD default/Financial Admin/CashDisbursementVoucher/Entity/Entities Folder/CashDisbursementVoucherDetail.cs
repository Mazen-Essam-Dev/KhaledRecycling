using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.CashDisbursementVoucher
{
    public class CashDisbursementVoucherDetail
    {
        [Key]
        public int Id { get; set; }
        // ✅ Navigation property + FK
        public int CashDisbursementVoucherId { get; set; }

        [ForeignKey(nameof(CashDisbursementVoucherId))]
        public virtual CashDisbursementVoucher? CashDisbursementVoucher { get; set; }
        public int? DocumentNo { get; set; }
        [Column(TypeName = "date")]
        public DateOnly? DocumentDate { get; set; }
        [MaxLength(500)]
        public string? Notes { get; set; }
        public decimal? Amount { get; set; }
    }
}
