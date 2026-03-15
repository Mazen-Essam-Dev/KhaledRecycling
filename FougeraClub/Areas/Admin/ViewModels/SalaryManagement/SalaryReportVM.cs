using Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace FougeraClub.Areas.Admin.ViewModels.SalaryManagement
{
    public class SalaryReportVM
    {
        public int? Id { get; set; }
        public int? month { get; set; }
        public int? year { get; set; }

        public IQueryable<SalaryManagementVM>? salaryManagementVMs { get; set; }= null!;

        public int? AcountantSignatureId { get; set; }
        [ForeignKey(nameof(AcountantSignatureId))]
        public virtual Signature? AcountantSignature { get; set; }

        public int? ManagerSignitureId { get; set; }
        [ForeignKey(nameof(ManagerSignitureId))]
        public virtual Signature? ManagerSignature { get; set; }

        public bool IsAbleToOpen { get; set; } = false;
    }
}
