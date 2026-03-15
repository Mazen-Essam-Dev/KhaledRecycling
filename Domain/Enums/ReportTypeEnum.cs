using System.ComponentModel.DataAnnotations;

namespace Domain.Enums
{
    public enum ReportTypeEnum
    {
        [Display(Name = "ExpensesReport2", ResourceType = typeof(Resources.Resource1))] // Malitious
        ExpensesReport = 1,

        [Display(Name = "ExpensesAndReceiptsReport", ResourceType = typeof(Resources.Resource1))] // Bank
        ExpensesAndReceiptsReport,
    }
}
