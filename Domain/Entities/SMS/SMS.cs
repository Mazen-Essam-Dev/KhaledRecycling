using Domain.Enums;
using Domain.HelperForDomain;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.SMS
{
    public class SMS
    {
        [Key]
        public int Id { get; set; }
        public string? Text { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime SendingDate { get; set; } = AppDubaiTime1.Now;
        public SMSStatus? IsDeliveredSMS { get; set; }
        public virtual ICollection<SMSReceiver> Receivers { get; set; } = new List<SMSReceiver>();
    }
}
