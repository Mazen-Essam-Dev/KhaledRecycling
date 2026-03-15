using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.ParticipationsInEventReport
{
    public class ParticipationsInEventReportDetail
    {
        [Key]
        public int Id { get; set; }

        // ✅ Navigation property + FK
        public int ParticipationsInEventReportId { get; set; }

        [ForeignKey(nameof(ParticipationsInEventReportId))]
        public virtual ParticipationsInEventReport? ParticipationsInEventReport { get; set; }

        [MaxLength(200)]
        public string? ActivityName { get; set; }

        [MaxLength(200)]
        public string? ActivityAction { get; set; }

        [MaxLength(500)]
        public string? ActivityReason { get; set; }
    }
}
