using Domain.Resources;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FougeraClub.Areas.Admin.ViewModels.CashExchangeBond
{
    public class CashExchangeBondDetailVM
    {
        [Key]
        public int Id { get; set; }

        // ✅ Navigation property + FK
        public int CashExchangeBondId { get; set; }

        [ForeignKey(nameof(CashExchangeBondId))]
        public virtual CashExchangeBondVM? CashExchangeBond { get; set; }

        [LocalizedRequired("Required"), LocalizedMaxLength(200, "MaxLength_200")]
        public string? DocumentNo { get; set; }


        [Column(TypeName = "date")]
        public DateOnly? DocumentDate { get; set; }

        [LocalizedRequired("Required"), LocalizedMaxLength(100, "MaxLength_100")]
        public string? AccountNo { get; set; }

        [LocalizedRequired("Required"), LocalizedMaxLength(200, "MaxLength_200")]
        public string? Particular { get; set; }
        [Range(0.00, 9999999999999999.99, ErrorMessageResourceType = typeof(Resource1), ErrorMessageResourceName = "MustbeNumber")]    
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Money { get; set; } = 0.00m;

    }
}
