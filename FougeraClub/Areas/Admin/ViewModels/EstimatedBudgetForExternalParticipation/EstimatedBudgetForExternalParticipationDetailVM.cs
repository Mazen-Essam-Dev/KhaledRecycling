using Domain.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FougeraClub.Areas.Admin.ViewModels.EstimatedBudgetForExternalParticipation
{
    public class EstimatedBudgetForExternalParticipationDetailVM
    {
        [Key]
        public int Id { get; set; }
        public int? EstimatedBudgetForExternalParticipationId { get; set; }
        public string? Name { get; set; }
        public string? Adj { get; set; }
        public string? Profession { get; set; }

    }
}
