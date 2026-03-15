using Domain.Enums;

namespace Domain.DTOs.Admin.SMSDTO
{
    public class MessageLogDTO
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
