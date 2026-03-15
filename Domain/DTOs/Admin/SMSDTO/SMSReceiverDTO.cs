using Domain.Entities;
using Domain.Entities.SMS;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.DTOs.Admin.SMSDTO
{
    public class SMSReceiverDTO
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

    }
}
