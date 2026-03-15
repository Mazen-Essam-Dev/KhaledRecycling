using Domain.Entities.MaterialOrder;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.MaterialOrder
{
    public class MaterialOrder
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string? MaterialOrderCode { get; set; } = null!;

        [Required]
        public int? DepartmentId { get; set; }

        [ForeignKey(nameof(DepartmentId))]
        public virtual Department? Department { get; set; } = null!;
        [Required]
        public string? UserId { get; set; }

        public int? TrainerId { get; set; }

        [Required]
        [Column(TypeName = "date")]
        public DateOnly? Date { get; set; }

        /// <summary>
        /// Sum of (Quantity * SinglePrice) for items.
        /// Maintained by service or computed at runtime (NotMapped).
        /// </summary>
        //[Column(TypeName = "decimal(18,2)")]
        [Range(0.00, double.MaxValue, ErrorMessage = "القيمة يجب أن تكون رقم")]
        public decimal? OrderTotal { get; set; } = 0.00m;

        // Navigation
        public virtual ICollection<MaterialOrderItem> Items { get; set; } = new List<MaterialOrderItem>();

        [MaxLength(500)]
        public string? Notes { get; set; }

        [ForeignKey(nameof(SignatureUser))]
        public int? SignatureUserId { get; set; }
        public virtual Signature? SignatureUser { get; set; }

        [ForeignKey(nameof(SignatureManager))]
        public int? SignatureManagerId { get; set; }
        public virtual Signature? SignatureManager { get; set; }
    }
}
