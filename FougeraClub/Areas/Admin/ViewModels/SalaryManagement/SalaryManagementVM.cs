using Domain.Entities.Employees;
using Domain.HelperForDomain;
using Domain.Resources;
using KhaledTeamRecycling.Helpers;
using Humanizer;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KhaledTeamRecycling.Areas.Admin.ViewModels.SalaryManagement;

public class SalaryManagementVM
{
    [Key]
    public int Id { get; set; }

    // 🔹 Employee Reference
    [LocalizedRequired("Required")]
    [ForeignKey(nameof(Employee))]
    public int EmployeeId { get; set; }
    public virtual Employee? Employee { get; set; }

    // 🔹 Month And Year
    [LocalizedRequired("Required")]
    public int Year { get; set; }

    [LocalizedRequired("Required")]
    [Range(1, 12, ErrorMessageResourceType = typeof(Resource1), ErrorMessageResourceName = "The_Value_Invalid")]
    public int Month { get; set; } = 0;

    // 🔹 Job Title
    [LocalizedMaxLength(150, "MaxLength_150")]
    public string? JobTitle { get; set; }

    // 🔹 monthly Salary Details
    [Column(TypeName = "decimal(18,2)")]
    public decimal? BasicSalary { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? Allowances { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? TotalSalary { get; set; }

    // 🔹 Days Information
    [Range(0, 31, ErrorMessageResourceType = typeof(Resource1), ErrorMessageResourceName = "RequiredFrom0_31")]
    public int? WorkDays { get; set; } = 30;

    // Holds the reduced workdays submitted from the client. Not mapped to DB directly;
    // controller will write this value into `WorkDays` before persisting.
    [NotMapped]
    public int? ReducedWorkDays { get; set; }

    [Range(0, 31, ErrorMessageResourceType = typeof(Resource1), ErrorMessageResourceName = "RequiredFrom0_31")]
    public int? AbsentDays { get; set; }

    [Range(0, 31, ErrorMessageResourceType = typeof(Resource1), ErrorMessageResourceName = "RequiredFrom0_31")]
    public int? SickLeaveDays { get; set; }

    [Range(0, 31, ErrorMessageResourceType = typeof(Resource1), ErrorMessageResourceName = "RequiredFrom0_31")]
    public int? AnnualLeaveDays { get; set; }

    // 🔹 Bonuses And Deductions
    [Column(TypeName = "decimal(18,2)")]
    public decimal? DeductionsAddition { get; set; } 

    [Column(TypeName = "decimal(18,2)")]
    public decimal? TotalDeductions { get; set; } 

    [Column(TypeName = "decimal(18,2)")]
    public decimal? BonusesAndMissions { get; set; }

    // 🔹 NetSalary
    [Column(TypeName = "decimal(18,2)")]
    public decimal? NetSalary { get; set; }

    public DateTime CreatedAt { get; set; } = AppDubaiTime1.Now;

}



