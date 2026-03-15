using Domain.Entities.PurchaseOrder;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.PurchaseOrder
{
    public class PurchaseOrder
    {
        [Key]
        public long Id { get; set; }

        [Required, MaxLength(50)]
        public string? PurchaseOrderCode { get; set; } = null!;

        [Required]
        public int? SupplierId { get; set; }

        [Required]
        [Column(TypeName = "date")]
        public DateOnly? Date { get; set; }

        public bool HasVAT { get; set; } = false;

        /// <summary>
        /// VAT percentage (e.g. 14 for 14%). Stored as percent value.
        /// </summary>
        //[Column(TypeName = "decimal(18,2)")]
        [Range(0.05, double.MaxValue, ErrorMessage = "القيمة يجب أن تكون رقم")]
        public decimal? VATValue { get; set; } = 0.05m;

        /// <summary>
        /// Sum of (Quantity * SinglePrice) for items.
        /// Maintained by service or computed at runtime (NotMapped).
        /// </summary>
        //[Column(TypeName = "decimal(18,2)")]
        [Range(0.00, double.MaxValue, ErrorMessage = "القيمة يجب أن تكون رقم")]
        public decimal? OrderTotal { get; set; } = 0.00m;

        /// <summary>
        /// OrderTotal + VAT (if HasVAT)
        /// </summary>
        //[Column(TypeName = "decimal(18,2)")]
        [Range(0.00, double.MaxValue, ErrorMessage = "القيمة يجب أن تكون رقم")]
        public decimal? OrderTotalWithVAT { get; set; }= 0.00m;

        [ForeignKey(nameof(SupplierId))]
        public virtual Supplier? Supplier { get; set; } = null!;

        // Navigation
        public virtual ICollection<PurchaseOrderItem> Items { get; set; } = new List<PurchaseOrderItem>();

        public int? SignatureId { get; set; }
        public virtual Signature? Signature { get; set; }
    }
}
