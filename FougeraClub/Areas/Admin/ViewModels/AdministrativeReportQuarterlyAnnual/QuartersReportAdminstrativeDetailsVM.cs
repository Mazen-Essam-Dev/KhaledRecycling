using Domain.Entities;
using Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FougeraClub.Areas.Admin.ViewModels.AdministrativeReportQuarterlyAnnual
{
    public class QuartersReportAdminstrativeDetailsVM
    {
        [Key]
        public int Id { get; set; }
        public QuarterlyReportType? Type { get; set; }
        public QuartersYear? Quarter { get; set; }
        public int? Year { get; set; }

        public int? ManagerSignitureId { get; set; }
        [ForeignKey(nameof(ManagerSignitureId))]
        public virtual Signature? ManagerSignature { get; set; }

        public string? ManagerFullName { get; set; }


        public IEnumerable<MonthsOfYearsAnnualyWithDetailsVM>? AdminstrativeDetailsVM_List = new List<MonthsOfYearsAnnualyWithDetailsVM>();

    }
}
