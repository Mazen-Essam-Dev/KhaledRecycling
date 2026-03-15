using Domain.Entities;
using Domain.Entities.MonthlyAdministrativeReport;
using System.ComponentModel.DataAnnotations;

namespace Domain.DTOs.Admin.AdministrativeReportQuarterlyAnnual;
public class MonthsOfYearsAnnualyWithDetailsDTO
{
    public int Id { get; set; }
    public int? year { get; set; }
    public int? month { get; set; }
    public int? Type { get; set; }

    [DataType(DataType.Date)]
    public DateOnly? Date { get; set; }


    // Child details
    public IEnumerable<MonthlyAdministrativeReportDetail> Details { get; set; } = new List<MonthlyAdministrativeReportDetail>();


}