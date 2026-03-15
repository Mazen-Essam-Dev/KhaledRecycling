using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.CashExchangeBond
{
    public class CashExchangeBondDetail
    {
        [Key]
        public int Id { get; set; }

        // ✅ Navigation property + FK
        public int CashExchangeBondId { get; set; }

        [ForeignKey(nameof(CashExchangeBondId))]
        public virtual CashExchangeBond? CashExchangeBond { get; set; }

        [MaxLength(200)]
        public string? DocumentNo { get; set; }


        [Column(TypeName = "date")]
        public DateOnly? DocumentDate { get; set; }

        [MaxLength(100)]
        public string? AccountNo { get; set; }

        [MaxLength(200)]
        public string? Particular { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? Money { get; set; } = 0.00m;

    }
}
