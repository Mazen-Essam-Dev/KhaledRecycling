using Domain.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace FougeraClub.Areas.Admin.ViewModels.MonthlyAdministrativeReport;

public class MonthlyAdministrativeReportVM
{
    // Display supervisor and manager full names under signatures
    public string? SupervisorFullName { get; set; }
    public string? ManagerFullName { get; set; }

    public int Id { get; set; }

    [LocalizedRequired("Required")]
    [DataType(DataType.Date)]
    public DateOnly? Date { get; set; }

    public string? TypeText { get; set; }

    [LocalizedRequired("Required")]
    public int? Type { get; set; }
    public IEnumerable<SelectListItem>? TypeEnumList { get; set; }

    [LocalizedMaxLength(200, "MaxLength_200")]
    [LocalizedRequired("Required")]
    public string? AdministrativeDepartment { get; set; }

    [LocalizedMaxLength(200, "MaxLength_200")]
    [LocalizedRequired("Required")]
    public string? ReportTitle { get; set; }

    public IFormFile? Image1 { get; set; }
    [LocalizedMaxLength(300, "MaxLength_300")]
    public string? Image1Path { get; set; }
    public IFormFile? Image2 { get; set; }
    [LocalizedMaxLength(300, "MaxLength_300")]
    public string? Image2Path { get; set; }
    public IFormFile? Image3 { get; set; }
    [LocalizedMaxLength(300, "MaxLength_300")]
    public string? Image3Path { get; set; }
    public IFormFile? Image4 { get; set; }
    [LocalizedMaxLength(300, "MaxLength_300")]
    public string? Image4Path { get; set; }

    public IFormFile? Image5 { get; set; }
    [LocalizedMaxLength(300, "MaxLength_300")]
    public string? Image5Path { get; set; }

    public IFormFile? Image6 { get; set; }
    [LocalizedMaxLength(300, "MaxLength_300")]
    public string? Image6Path { get; set; }

    // Temporary uploaded file 
    public string? TempFile1_Path { get; set; }
    public string? TempFile2_Path { get; set; }
    public string? TempFile3_Path { get; set; }
    public string? TempFile4_Path { get; set; }
    public string? TempFile5_Path { get; set; }
    public string? TempFile6_Path { get; set; }

    // For Edit: store old file from DB
    public string? Old1_Path { get; set; }
    public string? Old2_Path { get; set; }
    public string? Old3_Path { get; set; }
    public string? Old4_Path { get; set; }
    public string? Old5_Path { get; set; }
    public string? Old6_Path { get; set; }


    [LocalizedMaxLength(500, "MaxLength_500")]
    public string? Notes { get; set; }

    // Child details
    public List<MonthlyAdministrativeReportDetailVM> Details { get; set; }
        = new List<MonthlyAdministrativeReportDetailVM>();

    public int? TrainerSignitureId { get; set; }
    public Signature? TrainerSignature { get; set; }
    public int? ManagerSignitureId { get; set; }
    public Signature? ManagerSignature { get; set; }
}
