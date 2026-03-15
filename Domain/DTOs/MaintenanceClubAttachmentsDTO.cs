using Microsoft.AspNetCore.Http;

namespace Domain.DTOs
{
    public class MaintenanceClubAttachmentsDTO
    {
        public int MaintenanceClubId { get; set; }
        public List<MaintenanceClubAttachmentDTO> Attachments { get; set; } = new();
    }
}
