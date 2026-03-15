using Microsoft.AspNetCore.Http;

namespace FougeraClub.Areas.Admin.ViewModels.MaintenanceClub
{
    public class MaintenanceClubAttachmentVM
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Path { get; set; }
        public IFormFile? File { get; set; }
        public int MaintenanceClubId { get; set; }
    }
}
