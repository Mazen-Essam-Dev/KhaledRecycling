namespace FougeraClub.Areas.Admin.ViewModels.SalaryManagement
{
    public class SalaryManagementAttachmentsVM
    {
        public int SalaryManagementId { get; set; }
        public List<SalaryManagementAttachmentVM> Attachments { get; set; } = new List<SalaryManagementAttachmentVM>();
    }
}
