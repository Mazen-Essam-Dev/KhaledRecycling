using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class Car
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(20)]
        public string? PlateNumber { get; set; }
        [MaxLength(100)]
        public string? Type { get; set; }
        [MaxLength(100)]
        public string? DriverName { get; set; }
        [Column(TypeName = "date")]
        public DateOnly? OwnershipExpiryDate { get; set; }
        [MaxLength(100)]
        public string? Model { get; set; }
        [MaxLength(100)]
        public string? Color { get; set; }
        public string? Notes { get; set; }
        [MaxLength(255)]
        public string? AttachmentPath { get; set; }

        // Navigation property
        public ICollection<CarAttachment> CarAttachments { get; set; }
    }
}


