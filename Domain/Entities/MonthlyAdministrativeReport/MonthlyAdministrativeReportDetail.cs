using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.MonthlyAdministrativeReport
{
    public class MonthlyAdministrativeReportDetail
    {
        [Key]
        public int Id { get; set; }

        // ✅ Navigation property + FK
        public int MonthlyAdministrativeReportId { get; set; }

        [ForeignKey(nameof(MonthlyAdministrativeReportId))]
        public virtual MonthlyAdministrativeReport? MonthlyAdministrativeReport { get; set; }

        [Column(TypeName = "date")]
        public DateOnly? ActivityStartDate { get; set; }

        [Column(TypeName = "date")]
        public DateOnly? ActivityEndDate { get; set; }

        [MaxLength(200)]
        public string? ActivityName { get; set; }

        public int? NumberOfParticipants { get; set; }

        [MaxLength(500)]
        public string? Reason { get; set; }
    }
}
