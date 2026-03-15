using System.ComponentModel.DataAnnotations;

namespace FougeraClub.Areas.Admin.ViewModels.MonthlyAdministrativeReport;
public class MonthlyAdministrativeReportDetailVM
{
    public int Id { get; set; }

    [DataType(DataType.Date)]
    public DateOnly? ActivityStartDate { get; set; }

    [DataType(DataType.Date)]
    public DateOnly? ActivityEndDate { get; set; }

    [MaxLength(200)]
    public string? ActivityName { get; set; }

    public int? NumberOfParticipants { get; set; }

    [MaxLength(500)]
    public string? Reason { get; set; }

}