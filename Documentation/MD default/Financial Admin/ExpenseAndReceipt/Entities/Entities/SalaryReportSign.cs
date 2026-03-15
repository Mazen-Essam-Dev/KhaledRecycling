using Domain.Entities.ExpenseAndReceipt;
using Domain.Enums;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.ExpenseAndReceipt
{
    public class SalaryReportSign
    {
        [Key]
        public int Id { get; set; }
        public int? ReportSalaryTypeId { get; set; }
        [ForeignKey(nameof(ReportSalaryTypeId))]
        public virtual ReportSalaryType? ReportSalaryType { get; set; }
        public int? Month { get; set; }
        public int? Year { get; set; }
        public int? AcountantSignatureId { get; set; }
        [ForeignKey(nameof(AcountantSignatureId))]
        public virtual Signature? AcountantSignature { get; set; }

        public int? ManagerSignitureId { get; set; }
        [ForeignKey(nameof(ManagerSignitureId))]
        public virtual Signature? ManagerSignature { get; set; }

    }
}
