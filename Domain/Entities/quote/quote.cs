using Domain.Entities.quote;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.quote
{
    public class quote
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string? quoteCode { get; set; } = null!;

        public string? OrderText { get; set; }

        [MaxLength(500)]
        public string? TextArea { get; set; }

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

        public bool quoteIsConfirmed { get; set; } = false;

        // Navigation
        public virtual ICollection<quotesItem> quotesItems { get; set; } = new List<quotesItem>();

        [ForeignKey(nameof(SignatureSuperVisor))]
        public int? SignatureSuperVisorId { get; set; }
        public virtual Signature? SignatureSuperVisor { get; set; }


        [ForeignKey(nameof(SignatureAccountant))]
        public int? SignatureAccountantId { get; set; }
        public virtual Signature? SignatureAccountant { get; set; }


        [ForeignKey(nameof(SignatureUserSecetary))]
        public int? SignatureUserSecetaryId { get; set; }
        public virtual Signature? SignatureUserSecetary { get; set; }


        [ForeignKey(nameof(SignatureManager))]
        public int? SignatureManagerId { get; set; }
        public virtual Signature? SignatureManager { get; set; }
    }
}
