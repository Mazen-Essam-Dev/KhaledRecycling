using Domain.Entities;
using Domain.Entities.CashExchangeBond;
using Domain.Resources;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FougeraClub.Areas.Admin.ViewModels.CashExchangeBond
{
    public class CashExchangeBondVM
    {
        [Key]
        public int Id { get; set; }

        [LocalizedRequired("Required"), LocalizedMaxLength(100, "MaxLength_100")]
        public string? Code { get; set; }
        [LocalizedRequired("Required")]

        [Column(TypeName = "date")]
        public DateOnly? Date { get; set; }
        [LocalizedRequired("Required")]
        //[Range(0.00, 9999999999999999.99, ErrorMessageResourceType = typeof(Resource1), ErrorMessageResourceName = "MustbeNumber")]
        [Range(-9999999999999999.99, 9999999999999999.99, ErrorMessageResourceType = typeof(Resource1), ErrorMessageResourceName = "MustbeNumber")]

        [Column(TypeName = "decimal(18,2)")]
        public decimal? Money { get; set; } = null;

        [LocalizedRequired("Required"), LocalizedMaxLength(200, "MaxLength_200")]
        public string? PersonName { get; set; }
        [LocalizedRequired("Required"), LocalizedMaxLength(200, "MaxLength_200")]
        public string? AboutText { get; set; }


        [Range(-9999999999999999.99, 9999999999999999.99, ErrorMessageResourceType = typeof(Resource1), ErrorMessageResourceName = "MustbeNumber")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Total { get; set; } = 0.00m;


        // ✅ Recommended ICollection with initializer (avoids null issues)
        public virtual ICollection<CashExchangeBondDetail> Details { get; set; }
            = new List<CashExchangeBondDetail>();


        public int? AccountantSignitureId { get; set; }
        public Signature? AccountantSigniture { get; set; }
        public int? ManagerSignitureId { get; set; }
        public Signature? ManagerSignature { get; set; }

        // Full names for display
        public string? AccountantFullName { get; set; }
        public string? ManagerFullName { get; set; }
    }
}
