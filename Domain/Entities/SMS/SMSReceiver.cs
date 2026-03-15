using Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.SMS
{
    public class SMSReceiver
    {
        [Key]
        public int Id { get; set; }

        // FK → MemberEntity
        public int ReceiverId { get; set; }
        [ForeignKey(nameof(ReceiverId))]
        public virtual MemberEntity Receiver { get; set; } = null!;

        // FK → SMS
        public int SMSId { get; set; }
        [ForeignKey(nameof(SMSId))]
        public virtual SMS SMS { get; set; } = null!;

        [MaxLength(20)]
        public string? PhoneNumber { get; set; }

        public string? ServMessage { get; set; }
        public string? ServResponse { get; set; }
        public SMSStatus? IsDelivered { get; set; }

    }
}
