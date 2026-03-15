using Domain.Entities.MonthlyAdministrativeReport;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.MaterialOrder
{
    public class MaterialOrderItem
    {
        [Key]
        public int MaterialOrderItemId { get; set; }

        [Required]
        public int MaterialOrderId { get; set; }
        // Navigation
        [ForeignKey(nameof(MaterialOrderId))]
        public virtual MaterialOrder? MaterialOrder { get; set; } = null!;

        //[Required]
        public int? Quantity { get; set; } = 0;

        //[Required]
        //[Column(TypeName = "decimal(18,2)")]
        [Range(0.00, 9999999999, ErrorMessage = "القيمة يجب أن تكون رقم موجب")]

        //[Range(double.MinValue, double.MaxValue, ErrorMessage = "القيمة يجب أن تكون رقم")]
        public decimal? SinglePrice { get; set; } = 0.00m;

        //[Required]
        [MaxLength(200)]
        public string? ItemName { get; set; } = null!;


    }
}
