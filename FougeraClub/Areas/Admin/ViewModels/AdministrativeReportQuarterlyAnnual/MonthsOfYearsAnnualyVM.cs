using Domain.Entities;
using FougeraClub.Areas.Admin.ViewModels.MonthlyAdministrativeReport;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace FougeraClub.Areas.Admin.ViewModels.AdministrativeReportQuarterlyAnnual;
public class MonthsOfYearsAnnualyVM
{
    public int Id { get; set; }

    public int? year { get; set; }
    public int? month { get; set; }


    [DataType(DataType.Date)]
    public DateOnly? Date { get; set; }


    // Child details
    public List<MonthlyAdministrativeReportDetailVM> Details { get; set; }
        = new List<MonthlyAdministrativeReportDetailVM>();

  
}
