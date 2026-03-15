using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

public class Mission
{
    [Key]
    public int Id { get; set; }
    public int? ExternalWorkMissionId { get; set; }

    public int? MessionKey { get; set; }

    [ForeignKey(nameof(ExternalWorkMissionId))]
    public virtual ExternalWorkMission? ExternalWorkMission { get; set; }
}
