using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class Trainer
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(Department))]
        public int DepartmentId { get; set; }
        public Department Department { get; set; }


        public string UserId { get; set; }

        public string? Bio { get; set; }

        [MaxLength(255)]
        public string? AttachmentPath { get; set; }
        public virtual ICollection<Course>? Courses { get; set; }
    }
}
