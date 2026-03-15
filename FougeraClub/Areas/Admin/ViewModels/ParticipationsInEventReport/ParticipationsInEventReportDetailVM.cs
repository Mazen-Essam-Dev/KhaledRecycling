using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FougeraClub.Areas.Admin.ViewModels.ParticipationsInEventReport;
public class ParticipationsInEventReportDetailVM
{
    [Key]
    public int Id { get; set; }

    // ✅ Navigation property + FK
    public int ParticipationsInEventReportId { get; set; }

    [MaxLength(200)]
    public string? ActivityName { get; set; }

    [MaxLength(200)]
    public string? ActivityAction { get; set; }

    [MaxLength(500)]
    public string? ActivityReason { get; set; }
}