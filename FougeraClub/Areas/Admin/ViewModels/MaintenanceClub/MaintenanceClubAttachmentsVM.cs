using Microsoft.AspNetCore.Http;

namespace FougeraClub.Areas.Admin.ViewModels.MaintenanceClub
{
    public class MaintenanceClubAttachmentsVM
    {
        public int MaintenanceClubId { get; set; }
        public List<MaintenanceClubAttachmentVM> Attachments { get; set; } = new();
    }
}
