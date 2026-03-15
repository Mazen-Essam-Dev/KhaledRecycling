using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.SalaryManage
{
    public class SalaryManagementAttachment
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(300)]
        public string Path { get; set; }

        [NotMapped]
        public IFormFile? File { get; set; }

        // Foreign Key
        public int SalaryManagementId { get; set; }

        [ForeignKey(nameof(SalaryManagementId))]
        public SalaryManagement SalaryManagement { get; set; }
    }
}
