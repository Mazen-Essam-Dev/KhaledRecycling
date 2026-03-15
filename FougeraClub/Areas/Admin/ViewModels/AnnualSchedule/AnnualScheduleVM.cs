using Domain.Entities;
using FougeraClub.Helpers;
using Humanizer;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FougeraClub.Areas.Admin.ViewModels.AnnualSchedule;

public class AnnualScheduleVM
{
    public int Id { get; set; }
    [LocalizedRequired("Required")]
    [StringLength(200)]
    [Display(Name = "البند")]
    public string? Item { get; set; }

    [LocalizedRequired("Required")]
    [StringLength(1000)]
    [Display(Name = "البيان")]
    public string? Statement { get; set; }

    [LocalizedRequired("Required")]
    [Range(double.MinValue, double.MaxValue, ErrorMessage = "القيمة يجب أن تكون رقم")]
    [Display(Name = "السابق")]
    public decimal? Previous { get; set; }

    [LocalizedRequired("Required")]
    [Range(double.MinValue, double.MaxValue, ErrorMessage = "القيمة يجب أن تكون رقم")]
    [Display(Name = "الحالي")]
    public decimal? Current { get; set; }

    [LocalizedRequired("Required")]
    [Range(double.MinValue, double.MaxValue, ErrorMessage = "القيمة يجب أن تكون رقم")]
    [Display(Name = "السجل")]
    public decimal? Record { get; set; }

    [Display(Name = "الحالة")]
    [LocalizedRequired("Required")]
    [StringLength(200)]
    public string? Status { get; set; }

    [LocalizedRequired("Required")]
    [DataType(DataType.Date)]
    [Display(Name = "اليوم")]
    public DateTime? Day { get; set; }

    //[LocalizedRequired("Required")]
    [StringLength(200)]
    [Display(Name = "السنة")]
    public string? year { get; set; }

    [Display(Name = "التصنيف")]
    public int? AnnualScheduleCategoryId { get; set; }

    public string? CategoryName { get; set; }

}




