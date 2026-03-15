using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class CarServiceEntity
    {
        [Key]
        public int Id { get; set; }


        [ForeignKey(nameof(Car))]
        public int? CarId { get; set; }
        public virtual Car? Car { get; set; }

        [Column(TypeName = "date")]
        public DateOnly? Date { get; set; }
        public string? Details { get; set; }
        [MaxLength(255)]
        public string? AttachmentPath { get; set; }

        // Navigation property
        public ICollection<CarServiceAttachment> CarServiceAttachments { get; set; }
    }
}


