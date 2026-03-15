namespace FougeraClub.Areas.Admin.ViewModels.SMS
{
    public class SendingVM
    {
        public string? Message { get; set; }
        public List<int>? ReceiverIds { get; set; }
    }
}
