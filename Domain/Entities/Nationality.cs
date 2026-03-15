using Domain.Entities.Employees;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class Nationality
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(200, ErrorMessage = "Arabic name cannot exceed 200 characters.")]
        [Display(Name = "Nationality Name (Arabic)")]
        public string? NameAr { get; set; }

        [MaxLength(200, ErrorMessage = "English name cannot exceed 200 characters.")]
        [Display(Name = "Nationality Name (English)")]
        public string? NameEn { get; set; }

        public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

        public virtual ICollection<MemberEntity> Members { get; set; } = new List<MemberEntity>();
    }
}
