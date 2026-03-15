namespace FougeraClub.Areas.Admin.ViewModels.AdministrativeReportQuarterlyAnnual;
public class ActivitiesReportQuarterlyVM
{
    public IEnumerable<MonthsOfYearsAnnualyWithDetailsVM> MonthsOfYearsAnnualyWithDetails { get; set; }
        = new List<MonthsOfYearsAnnualyWithDetailsVM>();
    public PieChartVM PieChart { get; set; } = new PieChartVM();
}