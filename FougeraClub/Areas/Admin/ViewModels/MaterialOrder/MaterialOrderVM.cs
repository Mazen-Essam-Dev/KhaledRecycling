using Domain.Entities;
using Domain.Entities.MaterialOrder;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KhaledTeamRecycling.Areas.Admin.ViewModels.MaterialOrder;

public class MaterialOrderVM
{
    [Key]
    public int Id { get; set; }

    [LocalizedRequired("Required"), LocalizedMaxLength(50, "MaxLength_50")]
    public string? MaterialOrderCode { get; set; } = null!;

    [LocalizedRequired("Required")]
    public int? DepartmentId { get; set; }

    [ForeignKey(nameof(DepartmentId))]
    public virtual Department? Department { get; set; } = null!;
    [LocalizedRequired("Required")]
    public string? UserId { get; set; }

    public int? TrainerId { get; set; }

    [LocalizedRequired("Required")]
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

    // Username of the manager who signed (for display in signature area)
    public string? ManagerUserName { get; set; }
    public string? TrainerSignedName { get; set; }
    public string? TrainerName { get; set; }
    public string? DepartmentName { get; set; }

    public List<SelectListItem>? DepartmentsList { get; set; } = new();
    public List<SelectListItem>? UsersList { get; set; } = new();

    public bool IsTrainerLogged { get; set; } = false;
}

