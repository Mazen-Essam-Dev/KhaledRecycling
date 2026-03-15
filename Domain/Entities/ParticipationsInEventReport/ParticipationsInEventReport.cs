using Domain.Enums;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.ParticipationsInEventReport
{
    public class ParticipationsInEventReport
    {
        [Key]
        public int Id { get; set; }

        [Column(TypeName = "date")]
        public DateOnly? Date { get; set; }

        [MaxLength(200)]
        public string? AdministrativeDepartment { get; set; }

        [MaxLength(200)]
        public string? ReportTitle { get; set; }
        [MaxLength(200)]
        public string? ParticipatingTitle { get; set; }
        [NotMapped]
        public IFormFile? Image1 { get; set; }
        [MaxLength(300)]
        public string? Image1Path { get; set; }
        [NotMapped]
        public IFormFile? Image2 { get; set; }
        [MaxLength(300)]
        public string? Image2Path { get; set; }
        [NotMapped]
        public IFormFile? Image3 { get; set; }
        [MaxLength(300)]
        public string? Image3Path { get; set; }
        [NotMapped]
        public IFormFile? Image4 { get; set; }
        [MaxLength(300)]
        public string? Image4Path { get; set; }

        // ✅ Recommended ICollection with initializer (avoids null issues)
        public virtual ICollection<ParticipationsInEventReportDetail> Details { get; set; }
            = new List<ParticipationsInEventReportDetail>();
        
        public int? TrainerSignitureId { get; set; }
        [ForeignKey(nameof(TrainerSignitureId))]
        public virtual Signature? TrainerSignature { get; set; }
        public int? ManagerSignitureId { get; set; }
        [ForeignKey(nameof(ManagerSignitureId))]
        public virtual Signature? ManagerSignature { get; set; }

    }
}
