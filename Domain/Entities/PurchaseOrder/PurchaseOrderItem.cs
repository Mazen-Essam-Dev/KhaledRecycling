using Domain.Entities.MonthlyAdministrativeReport;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.PurchaseOrder
{
    public class PurchaseOrderItem
    {
        [Key]
        public int PurchaseOrderItemId { get; set; }

        [Required]
        public long PurchaseOrderId { get; set; }

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

        // Navigation
        [ForeignKey(nameof(PurchaseOrderId))]
        public virtual PurchaseOrder? PurchaseOrder { get; set; } = null!;
    }
}
