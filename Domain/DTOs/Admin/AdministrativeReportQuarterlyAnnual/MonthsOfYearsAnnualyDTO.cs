using Domain.Entities;

using System.ComponentModel.DataAnnotations;

namespace Domain.DTOs.Admin.AdministrativeReportQuarterlyAnnual;
public class MonthsOfYearsAnnualyDTO
{
    public int Id { get; set; }

    public int? year { get; set; }
    public int? month { get; set; }


    [DataType(DataType.Date)]
    public DateOnly? Date { get; set; }

    public int ActivitiesDetailsCount { get; set; }
    public int AdministrativeDetailsCount { get; set; }

}
