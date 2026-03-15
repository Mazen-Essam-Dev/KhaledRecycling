using Domain.Entities.CashExchangeBond;
using Domain.HelperForDomain;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class ScientificProjectGoals
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey(nameof(ScientificProjects))]
        public int ScientificProjectId { get; set; }
        public virtual ScientificProjects? ScientificProjects { get; set; }
        public int LangType { get; set; }
        [MaxLength(300)]
        public string? GoalDescription { get; set; }

    }
}
