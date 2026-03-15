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
    public class quotesItem
    {
        [Key]
        public int quoteItemId { get; set; }

        [Required]
        public int quoteId { get; set; }
        [ForeignKey(nameof(quoteId))]
        public virtual quote? quote { get; set; } = null!;

        // Navigation
        public virtual ICollection<ItemSupplier> ItemSuppliers { get; set; } = new List<ItemSupplier>();

        //[Required]
        public int? Quantity { get; set; } = 0;


        //[Required]
        [MaxLength(200)]
        public string? ItemName { get; set; } = null!;


    }
}
