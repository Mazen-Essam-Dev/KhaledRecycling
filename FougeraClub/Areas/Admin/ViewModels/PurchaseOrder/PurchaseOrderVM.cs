using Domain.Entities;
using Domain.Entities.PurchaseOrder;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FougeraClub.Areas.Admin.ViewModels.PurchaseOrder;

public class PurchaseOrderVM
{
    [Key]
    public long Id { get; set; }

    [LocalizedRequired("Required"), LocalizedMaxLength(50, "MaxLength_50")]

    public string? PurchaseOrderCode { get; set; } = null!;

    [LocalizedRequired("Required")]
    public int? SupplierId { get; set; }

    [LocalizedRequired("Required")]
    [Column(TypeName = "date")]
    public DateOnly? Date { get; set; }

    public bool HasVAT { get; set; } = false;

    /// <summary>
    /// VAT percentage (e.g. 14 for 14%). Stored as percent value.
    /// </summary>
    //[Column(TypeName = "decimal(5,2)")]
    [Range(0.05, 1, ErrorMessage = "قيمة الضريبة يجب أن تكون رقم بين 0.05 إلى 1")]
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
    [Range(0.00, double.MaxValue, ErrorMessage = "القيمة يجب أن تكون رقم")]
    public decimal? OrderTotalWithVAT { get; set; } = 0.00m;

    [ForeignKey(nameof(SupplierId))]
    public virtual Supplier? Supplier { get; set; } = null!;


    // Navigation
    public virtual ICollection<PurchaseOrderItem> Items { get; set; } = new List<PurchaseOrderItem>();
    public IEnumerable<Supplier>? suppliers { get; set; } = new List<Supplier>();
    public int? SignatureId { get; set; }
    public Signature? Signature { get; set; }

    // Username of the manager who signed (for display in signature area)
    public string? ManagerUserName { get; set; }
}

