using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Activity
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(200)]
        public string? TitleAr { get; set; }

        [MaxLength(200)]
        public string? TitleEn { get; set; }

        public DateOnly? StartDate { get; set; }

        public DateOnly? EndDate { get; set; }

        public string? MinimumAge { get; set; }

        [MaxLength(200)]
        public string? Location { get; set; }

        public string? Description { get; set; }
        public string? Achievement { get; set; }

        [MaxLength(300)]
        public string? AttachmentPath { get; set; }

        public virtual ICollection<Question> Questions { get; set; } = new List<Question>();
    }
}
