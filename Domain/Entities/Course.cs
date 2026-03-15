using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class Course
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(Department))]
        public int? DepartmentId { get; set; }

        [ForeignKey(nameof(Trainer))]
        public int? TrainerId { get; set; }

        [MaxLength(200)]
        public string? TitleAr { get; set; }

        [MaxLength(200)]
        public string? TitleEn { get; set; }

        public DateOnly? StartDate { get; set; }

        public DateOnly? EndDate { get; set; }

        [MaxLength(200)]
        public string? Location { get; set; }

        [MaxLength(50)]
        public string? Time { get; set; }

        public string? Description { get; set; }

        [MaxLength(300)]
        public string? AttachmentPath { get; set; }

        public virtual Department? Department { get; set; }

        public virtual Trainer? Trainer { get; set; }

    }
}
