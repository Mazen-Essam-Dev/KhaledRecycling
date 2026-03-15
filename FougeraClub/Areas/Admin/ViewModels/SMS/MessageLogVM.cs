using Domain.Enums;

namespace FougeraClub.Areas.Admin.ViewModels.SMS
{
    public class MessageLogVM
    {
        public string? MemberNameAr { get; set; }
        public string? MemberNameEn { get; set; }
        public string? Message { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime? DateAndTime { get; set; }
        public SMSStatus? IsDelivered { get; set; }
        public string? IdNumber { get; set; }
    }
}
