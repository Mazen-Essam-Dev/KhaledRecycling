using Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.DTOs.Admin.SalaryManagement
{
    public class EmployeesNameDTO
    {
        public int Id { get; set; }
        public string? FullNameAr { get; set; }
        public string? FullNameEn { get; set; }
        public string? JobTitle { get; set; }
        //public int? JobId { get; set; }
        public double? BasicSalary { get; set; }
        //[ForeignKey("JobId")]
        //public virtual Job? Job { get; set; }
    }
}
