namespace FougeraClub.Areas.Admin.ViewModels.Cars
{
    public class CarAttachmentsVM
    {
        public int CarId { get; set; }
        public List<CarAttachmentVM> Attachments { get; set; } = new List<CarAttachmentVM>();
    }
}
