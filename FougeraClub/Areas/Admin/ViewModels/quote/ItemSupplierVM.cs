using Domain.Entities;
using Domain.Entities.MaterialOrder;
using Domain.Entities.quote;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FougeraClub.Areas.Admin.ViewModels.quote;
public class ItemSupplierVM
{
    [Key]
    public int ItemSupplierId { get; set; }

    [LocalizedRequired("Required")]
    [ForeignKey(nameof(quotesItem))]
    public int quotesItemId { get; set; }
    public virtual quotesItem? quotesItem { get; set; } = null!;

    [LocalizedRequired("Required")]
    public int SupplierId { get; set; }
    // Navigation
    [ForeignKey(nameof(SupplierId))]
    public virtual Supplier? Supplier { get; set; } = null!;


    //[Required]
    //[Column(TypeName = "decimal(18,2)")]
    [Range(0.00, 9999999999, ErrorMessage = "القيمة يجب أن تكون رقم موجب")]

    //[Range(double.MinValue, double.MaxValue, ErrorMessage = "القيمة يجب أن تكون رقم")]
    public decimal? SinglePrice { get; set; } = 0.00m;


}