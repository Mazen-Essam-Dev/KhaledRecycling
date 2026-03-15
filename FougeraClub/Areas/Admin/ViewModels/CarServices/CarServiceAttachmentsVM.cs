namespace FougeraClub.Areas.Admin.ViewModels.CarServices
{
    public class CarServiceAttachmentsVM
    {
        public int CarServiceId { get; set; }
        public List<CarServiceAttachmentVM> Attachments { get; set; } = new List<CarServiceAttachmentVM>();
    }
}
