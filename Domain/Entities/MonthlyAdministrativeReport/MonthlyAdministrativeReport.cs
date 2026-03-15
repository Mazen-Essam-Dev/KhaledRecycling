using Domain.Enums;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.MonthlyAdministrativeReport
{
    public class MonthlyAdministrativeReport
    {
        [Key]
        public int Id { get; set; }

        [Column(TypeName = "date")]
        public DateOnly? Date { get; set; }

        public MonthlyAdministrativeReportType? Type { get; set; }

        [MaxLength(200)]
        public string? AdministrativeDepartment { get; set; }

        [MaxLength(200)]
        public string? ReportTitle { get; set; }
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
        [NotMapped]
        public IFormFile? Image5 { get; set; }
        [MaxLength(300)]
        public string? Image5Path { get; set; }
        [NotMapped]
        public IFormFile? Image6 { get; set; }
        [MaxLength(300)]
        public string? Image6Path { get; set; }

        [MaxLength(500)]
        public string? Notes { get; set; }

        // ✅ Recommended ICollection with initializer (avoids null issues)
        public virtual ICollection<MonthlyAdministrativeReportDetail> Details { get; set; }
            = new List<MonthlyAdministrativeReportDetail>();
        
        public int? TrainerSignitureId { get; set; }
        [ForeignKey(nameof(TrainerSignitureId))]
        public virtual Signature? TrainerSignature { get; set; }
        public int? ManagerSignitureId { get; set; }
        [ForeignKey(nameof(ManagerSignitureId))]
        public virtual Signature? ManagerSignature { get; set; }

    }
}
