using Domain.Entities;
using FougeraClub.Areas.Admin.ViewModels.MonthlyAdministrativeReport;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace FougeraClub.Areas.Admin.ViewModels.AdministrativeReportQuarterlyAnnual;
public class MonthsOfYearsAnnualyActivitiesAndAdministrative
{
    public List<MonthsOfYearsAnnualyWithDetailsVM>? model_Activities { get; set; }
    public List<MonthsOfYearsAnnualyWithDetailsVM>? model_Administrative { get; set; }
    public List<int>? listOfQuarter { get; set; }
    public List<string>? labelsMonths { get; set; }
    public List<int>? adminstrative { get; set; }
    public List<int>? activities { get; set; }

}
