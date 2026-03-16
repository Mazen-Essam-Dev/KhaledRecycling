using Microsoft.AspNetCore.Http;

namespace KhaledTeamRecycling.Areas.Admin.ViewModels.SalaryManagement
{
    public class SalaryManagementAttachmentVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public IFormFile? File { get; set; }
    }
}
