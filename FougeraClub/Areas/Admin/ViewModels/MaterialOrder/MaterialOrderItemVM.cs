using Domain.Entities.MaterialOrder;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FougeraClub.Areas.Admin.ViewModels.MaterialOrder;
public class MaterialOrderItemVM
{
    [Key]
    public int MaterialOrderItemId { get; set; }

    [Required]
    public int MaterialOrderId { get; set; }

    //[Required]
    public int Quantity { get; set; } = 0;

    //[Required]
    //[Column(TypeName = "decimal(18,2)")]
    [Range(0.00, double.MaxValue, ErrorMessage = "القيمة يجب أن تكون رقم")]
    public decimal? SinglePrice { get; set; } = null;

    [LocalizedMaxLength(200, "MaxLength_200")]

    public string ItemName { get; set; } = null!;

    // Navigation
    [ForeignKey(nameof(MaterialOrderId))]
    public virtual MaterialOrderVM? MaterialOrder { get; set; } = null!;
}