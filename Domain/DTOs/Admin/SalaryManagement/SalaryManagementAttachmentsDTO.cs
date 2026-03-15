namespace Domain.DTOs.Admin.SalaryManagement
{
    public class SalaryManagementAttachmentsDTO
    {
        public int SalaryManagementId { get; set; }
        public List<SalaryManagementAttachmentDTO> Attachments { get; set; } = new List<SalaryManagementAttachmentDTO>();
    }
}
