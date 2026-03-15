using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class MaintenanceClubAttachment
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Path { get; set; }

        [NotMapped]
        public IFormFile? File { get; set; }

        // Foreign Key
        public int MaintenanceClubId { get; set; }

        // Navigation Property
        [ForeignKey(nameof(MaintenanceClubId))]
        public MaintenanceClub? MaintenanceClub { get; set; }
    }
}
