using Domain.Entities;
using FougeraClub.Areas.Admin.ViewModels.MonthlyAdministrativeReport;
using System.ComponentModel.DataAnnotations;

namespace FougeraClub.Areas.Admin.ViewModels.AdministrativeReportQuarterlyAnnual;
public class MonthsOfYearsAnnualyWithDetailsVM
{
    public int Id { get; set; }
    public int? year { get; set; }
    public int? month { get; set; }
    public int? Type { get; set; }

    [DataType(DataType.Date)]
    public DateOnly? Date { get; set; }


    // Child details
    public IEnumerable<MonthlyAdministrativeReportDetailVM> Details { get; set; }
        = new List<MonthlyAdministrativeReportDetailVM>();

    // Paging Info
    public int? CurrentPage { get; set; }
    public int? PageSize { get; set; }
    public int? TotalDetails { get; set; }


}