using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.EstimatedBudgetForExternalParticipation;

public class EstimatedBudgetForExternalParticipationDetail
{
    [Key]
    public int Id { get; set; }
    public int? EstimatedBudgetForExternalParticipationId { get; set; }

    public string? Name { get; set; }
    public string? Adj { get; set; }
    public string? Profession { get; set; }

    [ForeignKey(nameof(EstimatedBudgetForExternalParticipationId))]
    public virtual EstimatedBudgetForExternalParticipation? EstimatedBudgetForExternalParticipation { get; set; }
}
