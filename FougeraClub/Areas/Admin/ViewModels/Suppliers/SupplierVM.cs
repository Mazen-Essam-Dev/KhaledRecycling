using Domain.Entities;
using Domain.Resources;
using FougeraClub.Attributes;
using System.ComponentModel.DataAnnotations;

namespace FougeraClub.Areas.Admin.ViewModels.Suppliers;

public class SupplierVM
{
    public int Id { get; set; }

    [LocalizedRequired("Required")]
    [StringLength(200)]
    [RegularExpression(@"^[\u0621-\u064A0-9 ]+$", ErrorMessage = "يجب أن يحتوي على حروف عربية وأرقام فقط")]
    [Display(Name = "اسم المورد")]
    public string? SupplierNameAr { get; set; }
    [LocalizedRequired("Required")]
    [StringLength(200)]
    [RegularExpression(@"^[a-zA-Z0-9 ]+$", ErrorMessage = "must contain only English letters and numbers")]
    public string? SupplierNameEn { get; set; }

    public string? textCategory { get; set; }
    public string? VATNumber { get; set; }
    public string? Phone { get; set; }
    public string? Mobile { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }

    [StringLength(1000)]
    public string? Description { get; set; }
    public bool HasRelatedData { get; set; }

    [Display(Name = "تصنيف المورد")]
    public int SupplierCategoryId { get; set; } = 1;

}

