using System.ComponentModel.DataAnnotations;

namespace Domain.Enums
{
    public enum ReportSalaryTypeEnum
    {
        [Display(Name = "payrollReport", ResourceType = typeof(Resources.Resource1))] // Salary Report
        SalaryReport = 1,

        [Display(Name = "DiscountsAndBonusesReport", ResourceType = typeof(Resources.Resource1))] // DiscountsAndBonuses Report
        DiscountsAndBonusesReport,
        [Display(Name = "AbsenceReport", ResourceType = typeof(Resources.Resource1))] // Absence Report
        AbsencesReport,
    }
}
