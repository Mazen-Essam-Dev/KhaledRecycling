namespace FougeraClub.Areas.Admin.ViewModels.Employees
{
    public class EmployeeAttachmentsVM
    {
        public int EmployeeId { get; set; }
        public List<AttachmentVM>? Attachments { get; set; }
    }
}
