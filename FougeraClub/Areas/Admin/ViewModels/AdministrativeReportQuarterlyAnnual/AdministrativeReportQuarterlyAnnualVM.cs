using Domain.Entities;
using FougeraClub.Areas.Admin.ViewModels.MonthlyAdministrativeReport;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace FougeraClub.Areas.Admin.ViewModels.AdministrativeReportQuarterlyAnnual;
public class AdministrativeReportQuarterlyAnnualVM
{
    public int Id { get; set; }

    [DataType(DataType.Date)]
    public DateOnly? Date { get; set; }

    public string? TypeText { get; set; }
    public int? Type { get; set; }
    public IEnumerable<SelectListItem>? TypeEnumList { get; set; }

    [MaxLength(200)]
    [LocalizedRequired("Required")]
    public string? AdministrativeDepartment { get; set; }

    [MaxLength(200)]
    [LocalizedRequired("Required")]
    public string? ReportTitle { get; set; }

    public IFormFile? Image1 { get; set; }
    [MaxLength(300)]
    public string? Image1Path { get; set; }
    public IFormFile? Image2 { get; set; }
    [MaxLength(300)]
    public string? Image2Path { get; set; }
    public IFormFile? Image3 { get; set; }
    [MaxLength(300)]
    public string? Image3Path { get; set; }
    public IFormFile? Image4 { get; set; }
    [MaxLength(300)]
    public string? Image4Path { get; set; }

    // Child details
    public List<MonthlyAdministrativeReportDetailVM> Details { get; set; }
        = new List<MonthlyAdministrativeReportDetailVM>();

    public int? TrainerSignitureId { get; set; }
    public Signature? TrainerSignature { get; set; }
    public int? ManagerSignitureId { get; set; }
    public Signature? ManagerSignature { get; set; }
}
