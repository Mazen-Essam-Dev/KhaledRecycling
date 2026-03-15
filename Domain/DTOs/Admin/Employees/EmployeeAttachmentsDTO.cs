namespace Domain.DTOs.Admin.Employees
{
    public class EmployeeAttachmentsDTO
    {
        public int EmployeeId { get; set; }
        public List<AttachmentDTO>? Attachments { get; set; }
    }
}
