using Domain.Enums;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.CashExchangeBond
{
    public class CashExchangeBond
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(100)]
        public string? Code { get; set; }

        [Column(TypeName = "date")]
        public DateOnly? Date { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? Money { get; set; } = 0.00m;

        [MaxLength(200)]
        public string? PersonName { get; set; }
        [MaxLength(200)]
        public string? AboutText { get; set; }


        [Column(TypeName = "decimal(18,2)")]
        public decimal? Total { get; set; } = 0.00m;


        // ✅ Recommended ICollection with initializer (avoids null issues)
        public virtual ICollection<CashExchangeBondDetail> Details { get; set; }
            = new List<CashExchangeBondDetail>();

        public virtual ICollection<CashExchangeBondAttachment>? CashExchangeBondAttachments { get; set; }

        public int? AccountantSignitureId { get; set; }
        [ForeignKey(nameof(AccountantSignitureId))]
        public virtual Signature? AccountantSigniture { get; set; }
        public int? ManagerSignitureId { get; set; }
        [ForeignKey(nameof(ManagerSignitureId))]
        public virtual Signature? ManagerSignature { get; set; }
    }
}
