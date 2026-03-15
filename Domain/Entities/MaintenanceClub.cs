using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations.Schema;


namespace Domain.Entities
{
    public class MaintenanceClub
    {
        public int Id { get; set; }

        //[Required(ErrorMessage = "العنوان مطلوب")]
        [StringLength(100)]
        [Display(Name = "عنوان الصيانة")]
        public string? Title { get; set; }

        //[Required(ErrorMessage = "التاريخ مطلوب")]
        [DataType(DataType.Date)]
        [Display(Name = "تاريخ الصيانة")]
        public DateTime? Date { get; set; }

        //[Required(ErrorMessage = "الوقت مطلوب")]
        [DataType(DataType.Time)]
        [Display(Name = "وقت الصيانة")]
        public TimeSpan? Time { get; set; }

        [StringLength(200)]
        [Display(Name = "مكان الصيانة")]
        public string? Location { get; set; }

        [StringLength(2000)]
        [Display(Name = "تفاصيل الصيانة")]
        public string? Details { get; set; }

        [Display(Name = "ملف PDF")]
        [DataType(DataType.Upload)]
        //[AllowedExtensions(new string[] { ".pdf" }, ErrorMessage = "يُسمح فقط بملفات PDF")]
        public string? PdfFilePath { get; set; }
        [NotMapped]
        public IFormFile? PdfFile { get; set; }

        // Navigation Property for Attachments
        public virtual ICollection<MaintenanceClubAttachment>? Attachments { get; set; }
    }
}
