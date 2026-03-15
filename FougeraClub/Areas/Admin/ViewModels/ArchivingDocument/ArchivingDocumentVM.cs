using Domain.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FougeraClub.Areas.Admin.ViewModels.ArchivingDocument
{
    public class ArchivingDocumentVM
    {
        public int Id { get; set; }

        [StringLength(200)]
        [LocalizedRequired("Required"), LocalizedMaxLength(200, "MaxLength_200")]
        public string? Title { get; set; }
        [LocalizedRequired("Required")]
        [DataType(DataType.Date)]
        public DateTime? Date { get; set; }

        [LocalizedRequired("Required"), LocalizedMaxLength(200, "MaxLength_200")]
        public string? Authority { get; set; }

        [LocalizedRequired("Required"), LocalizedMaxLength(200, "MaxLength_200")]
        public string? DocumentReferenceNumber { get; set; }

        [LocalizedRequired("Required")]
        public DocumentType? Type { get; set; }
        public string? TypeText { get; set; }
        public IEnumerable<SelectListItem>? TypeEnumList { get; set; }


        [Display(Name = "ملف PDF")]
        [DataType(DataType.Upload)]
        //[AllowedExtensions(new string[] { ".pdf" }, ErrorMessage = "يُسمح فقط بملفات PDF")]
        public string? PdfFilePath { get; set; }
        [NotMapped]
        public IFormFile? PdfFile { get; set; }

        // Temporary uploaded file
        public string? TempFilePath { get; set; }

        // For Edit: store old file from DB
        public string? OldPath { get; set; }
        // Category
        public int? DocumentCategoryId { get; set; }
        public string? DocumentCategoryName { get; set; }
        public IEnumerable<SelectListItem>? DocumentCategoryList { get; set; }

    }
}
