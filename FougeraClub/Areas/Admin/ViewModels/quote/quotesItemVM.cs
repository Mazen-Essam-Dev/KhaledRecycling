using Domain.Entities.quote;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FougeraClub.Areas.Admin.ViewModels.quote;
public class quotesItemVM
{
    [Key]
    public int quotesItemId { get; set; }

    [LocalizedRequired("Required")]
    public int quoteId { get; set; }
    [ForeignKey(nameof(quoteId))]
    public virtual Domain.Entities.quote.quote? quote { get; set; } = null!;

    // Navigation
    public virtual ICollection<ItemSupplier> ItemSuppliers { get; set; } = new List<ItemSupplier>();

    [LocalizedRequired("Required")]
    public int? Quantity { get; set; } = 0;


    //[Required]
    [LocalizedRequired("Required")]
    [LocalizedMaxLength(200, "MaxLength_200")]
    public string? ItemName { get; set; } = null!;


}