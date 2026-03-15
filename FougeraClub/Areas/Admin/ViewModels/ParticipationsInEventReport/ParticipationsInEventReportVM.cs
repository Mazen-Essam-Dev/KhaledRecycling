using Domain.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace FougeraClub.Areas.Admin.ViewModels.ParticipationsInEventReport;

public class ParticipationsInEventReportVM
{
    // Display supervisor and manager full names under signatures
    public string? SupervisorFullName { get; set; }
    public string? ManagerFullName { get; set; }

    public int Id { get; set; }

    [LocalizedRequired("Required")]
    [DataType(DataType.Date)]
    public DateOnly? Date { get; set; }


    [MaxLength(200)]
    [LocalizedRequired("Required")]
    public string? AdministrativeDepartment { get; set; }

    [MaxLength(200)]
    [LocalizedRequired("Required")]
    public string? ReportTitle { get; set; }

    [MaxLength(200)]
    public string? ParticipatingTitle { get; set; }

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

    // Temporary uploaded file 
    public string? TempFile1_Path { get; set; }
    public string? TempFile2_Path { get; set; }
    public string? TempFile3_Path { get; set; }
    public string? TempFile4_Path { get; set; }

    // For Edit: store old file from DB
    public string? Old1_Path { get; set; }
    public string? Old2_Path { get; set; }
    public string? Old3_Path { get; set; }
    public string? Old4_Path { get; set; }

    // Child details
    public List<ParticipationsInEventReportDetailVM> Details { get; set; }
        = new List<ParticipationsInEventReportDetailVM>();

    public int? TrainerSignitureId { get; set; }
    public Signature? TrainerSignature { get; set; }
    public int? ManagerSignitureId { get; set; }
    public Signature? ManagerSignature { get; set; }
}
