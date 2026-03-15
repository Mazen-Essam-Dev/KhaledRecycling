using Domain.Entities.Employees;
using Domain.HelperForDomain;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.SalaryManage
{
    public class SalaryManagement
    {
        [Key]
        public int Id { get; set; }

        // 🔹 Employee Reference
        [Required]
        [ForeignKey(nameof(Employee))]
        public int EmployeeId { get; set; }
        public virtual Employee? Employee { get; set; }

        // 🔹 Month And Year
        [Required]
        public int Year { get; set; }

        [Required]
        [Range(1, 12)]
        public int Month { get; set; }

        // 🔹 Job Title
        [StringLength(150)]
        public string? JobTitle { get; set; }

        // 🔹 monthly Salary Details
        [Column(TypeName = "decimal(18,2)")]
        public decimal? BasicSalary { get; set; } = null;

        [Column(TypeName = "decimal(18,2)")]
        public decimal? Allowances { get; set; } = null;

        [Column(TypeName = "decimal(18,2)")]
        public decimal? TotalSalary { get; set; } = null;

        // 🔹 Days Information
        [Range(0, 31)]
        public int WorkDays { get; set; } = 30;

        [Range(0, 31)]
        public int? AbsentDays { get; set; } = null;

        [Range(0, 31)]
        public int? SickLeaveDays { get; set; } = null;

        [Range(0, 31)]
        public int? AnnualLeaveDays { get; set; } = null;

        // 🔹 Bonuses And Deductions
        [Column(TypeName = "decimal(18,2)")]
        public decimal? DeductionsAddition { get; set; } = null;

        [Column(TypeName = "decimal(18,2)")]
        public decimal? TotalDeductions { get; set; }  = null;

        [Column(TypeName = "decimal(18,2)")]
        public decimal? BonusesAndMissions { get; set; }  = null;

        // 🔹 NetSalary
        [Column(TypeName = "decimal(18,2)")]
        public decimal? NetSalary { get; set; } = null;

        public DateTime CreatedAt { get; set; } = AppDubaiTime1.Now;

    }
}
