using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class Supplier
    {
        public int Id { get; set; }

        [StringLength(200)]
        [Display(Name = "اسم المورد")]
        public string? SupplierNameAr { get; set; }
        [StringLength(200)]
        [Display(Name = "اسم الموردE")]
        public string? SupplierNameEn { get; set; }

        [StringLength(50)]
        [Display(Name = "رقم VAT")]
        public string? VATNumber { get; set; }

        [Phone(ErrorMessage = "رقم الهاتف غير صحيح")]
        [StringLength(20)]
        [Display(Name = "رقم الهاتف")]
        public string? Phone { get; set; }

        [Phone(ErrorMessage = "رقم الموبايل غير صحيح")]
        [StringLength(20)]
        [Display(Name = "رقم الموبايل")]
        public string? Mobile { get; set; }

        [EmailAddress(ErrorMessage = "البريد الإلكتروني غير صحيح")]
        [StringLength(200)]
        [Display(Name = "البريد الإلكتروني")]
        public string? Email { get; set; }

        [StringLength(500)]
        [Display(Name = "العنوان")]
        public string? Address { get; set; }

        [StringLength(1000)]
        [Display(Name = "الوصف")]
        public string? Description { get; set; }
        [Display(Name = "تصنيف المورد")]
        public int SupplierCategoryId { get; set; } = 1;

        public SupplierCategory? SupplierCategory { get; set; }
        [NotMapped]
        public bool HasRelatedData { get; set; }
    }
}
