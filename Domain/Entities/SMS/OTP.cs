using System;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.SMS
{
    public class OTP
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(128)] // SHA256 hash length = 64 bytes hex = 128 chars
        public string Code { get; set; } = null!;

        [Required]
        public DateTime CreateAt { get; set; }

        [Required]
        public DateTime ExpireAt { get; set; }

        public bool IsUsed { get; set; } = false;

        [MaxLength(50)]
        public string? UserId { get; set; } // optional (if OTP tied to phone)
    }
}
