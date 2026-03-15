using Domain.Entities.Employees;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.Employees
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }
        public int Code { get; set; }

        [MaxLength(100)]
        public string? FullNameAr { get; set; }
        [MaxLength(100)]
        public string? FullNameEn { get; set; }

        [MaxLength(20)]
        public string? PhoneNumber { get; set; }

        public int? NationalityId { get; set; }

        [MaxLength(100)]
        public string? Email { get; set; }


        [MaxLength(200)]
        public string? Address { get; set; }

        [MaxLength(50)]
        public string? PassportNumber { get; set; }

        public DateOnly? PassportExpiryDate { get; set; }

        [MaxLength(50)]
        public string? NationalIdNumber { get; set; }

        public DateOnly? NationalIdExpiryDate { get; set; }

        [MaxLength(300)]
        public string? NationalIdLocation { get; set; }
        [MaxLength(50)]
        public string? BankAccountNumber { get; set; }

        [MaxLength(100)]
        public string? BankName { get; set; }

        [MaxLength(200)]
        public string? PhotoPath { get; set; }
        [NotMapped]
        public IFormFile? Photo { get; set; }

        public double? Salary { get; set; }
        public string? Notes { get; set; }

        public int Type { get; set; }

        // Navigation Properties
        [ForeignKey("NationalityId")]
        public virtual Nationality? Nationality { get; set; }   


        [MaxLength(200)]
        public string? JobTitle { get; set; }

        public virtual ICollection<EmployeeAttachment>? EmployeeAttachments { get; set; }
        [NotMapped]
        public bool HasRelatedObjects { get; set; }
    }
}
