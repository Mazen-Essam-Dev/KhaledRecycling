using Domain.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FougeraClub.Areas.Admin.ViewModels.CarServices
{
    public class CarServiceVM
    {
        public int Id { get; set; }
        [LocalizedRequired("Required")]
        public int CarId { get; set; }
        public Car? Car { get; set; }
        public List<SelectListItem>? CarList { get; set; } = new();
        [LocalizedRequired("Required")]
        [Column(TypeName = "date")]
        public DateOnly? Date { get; set; }
        [LocalizedRequired("Required"), LocalizedMaxLength(500, "MaxLength_500")]
        public string? Details { get; set; }
        [MaxLength(255)]
        public string? AttachmentPath { get; set; }
        public IFormFile? Attachment { get; set; }

        // Temporary uploaded file
        public string? Attachment_TempFilePath { get; set; }

        // For Edit: store old file from DB
        public string? Attachment_OldPath { get; set; }

    }
}

