using Domain.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Domain.Entities
{
    public class ArchivingDocument
    {
        public int Id { get; set; }

        [StringLength(200)]
        public string? Title { get; set; }

        [DataType(DataType.Date)]
        public DateTime? Date { get; set; }

        [StringLength(200)]
        public string? Authority { get; set; }

        [StringLength(200)]
        public string? DocumentReferenceNumber { get; set; }

        public DocumentType? Type { get; set; }

        [Display(Name = "ملف PDF")]
        [DataType(DataType.Upload)]
        //[AllowedExtensions(new string[] { ".pdf" }, ErrorMessage = "يُسمح فقط بملفات PDF")]
        public string? PdfFilePath { get; set; }
        [NotMapped]
        public IFormFile? PdfFile { get; set; }

        // Nullable FK to DocumentCategory to preserve existing records when categories are removed
        public int? DocumentCategoryId { get; set; }
        public DocumentCategory? DocumentCategory { get; set; }

    }
}
