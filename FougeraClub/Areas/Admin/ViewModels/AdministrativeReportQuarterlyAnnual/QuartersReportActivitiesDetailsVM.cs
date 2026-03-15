using Domain.Entities;
using Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FougeraClub.Areas.Admin.ViewModels.AdministrativeReportQuarterlyAnnual
{
    public class QuartersReportActivitiesDetailsVM
    {
        // Display manager full name under signature
        public string? ManagerFullName { get; set; }
    
        [Key]
    public int Id { get; set; }
    public QuarterlyReportType? Type { get; set; }
    public QuartersYear? Quarter { get; set; }
    public int? Year { get; set; }

    public int? ManagerSignitureId { get; set; }
    [ForeignKey(nameof(ManagerSignitureId))]
    public virtual Signature? ManagerSignature { get; set; }

    public ActivitiesReportQuarterlyVM? ActivitiesDetailsVM { get; set; }


}
}
