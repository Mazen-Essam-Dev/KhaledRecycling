using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class City
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Arabic name is required.")]
        [MaxLength(100, ErrorMessage = "Arabic name cannot exceed 100 characters.")]
        [Display(Name = "City Name (Arabic)")]
        public string? NameAr { get; set; }

        [Required(ErrorMessage = "English name is required.")]
        [MaxLength(100, ErrorMessage = "English name cannot exceed 100 characters.")]
        [Display(Name = "City Name (English)")]
        public string? NameEn { get; set; }
    }
}
