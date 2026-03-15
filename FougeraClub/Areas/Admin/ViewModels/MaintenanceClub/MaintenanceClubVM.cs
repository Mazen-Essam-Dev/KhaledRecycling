using System.ComponentModel.DataAnnotations;

namespace FougeraClub.Areas.Admin.ViewModels.MaintenanceClub;

public class MaintenanceClubVM
{
    public int Id { get; set; }

    [LocalizedRequired("Required")]
    [StringLength(100)]
    [Display(Name = "عنوان الصيانة")]
    public string? Title { get; set; }

    [LocalizedRequired("Required")]
    [DataType(DataType.Date)]
    [Display(Name = "تاريخ الصيانة")]
    public DateTime? Date { get; set; }

    [LocalizedRequired("Required")]
    [DataType(DataType.Time)]
    [Display(Name = "وقت الصيانة")]
    public TimeSpan? Time { get; set; }
    [LocalizedRequired("Required")]
    [StringLength(200)]
    [Display(Name = "مكان الصيانة")]
    public string? Location { get; set; }

    [StringLength(2000)]
    [Display(Name = "تفاصيل الصيانة")]
    public string? Details { get; set; }

    [Display(Name = "ملف PDF")]
    [DataType(DataType.Upload)]
    public IFormFile? PdfFile { get; set; }

    [StringLength(200)]
    public string? PdfFilePath { get; set; }

    // Temporary uploaded file
    public string? Attachment_TempFilePath { get; set; }

    // For Edit: store old file from DB
    public string? Attachment_OldPath { get; set; }
}




