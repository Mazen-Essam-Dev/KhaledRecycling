using Microsoft.AspNetCore.Http;

namespace Domain.DTOs.Admin.SalaryManagement
{
    public class SalaryManagementAttachmentDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public IFormFile? File { get; set; }
    }
}
