using Domain.Entities.MonthlyAdministrativeReport;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.quote
{
    public class ItemSupplier
    {
        [Key]
        public int ItemSupplierId { get; set; }

        [Required]
        [ForeignKey(nameof(quotesItem))]
        public int quotesItemId { get; set; }
        public virtual quotesItem? quotesItem { get; set; } = null!;

        [Required]
        public int SupplierId { get; set; }
        // Navigation
        [ForeignKey(nameof(SupplierId))]
        public virtual Supplier? Supplier { get; set; } = null!;


        //[Required]
        //[Column(TypeName = "decimal(18,2)")]
        [Range(0.00, 9999999999, ErrorMessage = "القيمة يجب أن تكون رقم موجب")]

        //[Range(double.MinValue, double.MaxValue, ErrorMessage = "القيمة يجب أن تكون رقم")]
        public decimal? SinglePrice { get; set; } = 0.00m;

        public bool ItemSupplierIsConfirmed { get; set; } = false;

    }
}
