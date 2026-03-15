using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class RequestLog
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(200)]
        public string? Path { get; set; }

        [MaxLength(200)]
        public string? Method { get; set; }

        [MaxLength(200)]
        public string? Controller { get; set; }

        [MaxLength(200)]
        public string? Action { get; set; }

        [MaxLength(200)]
        public string? NameAr { get; set; }

        [MaxLength(200)]
        public string? NameEn { get; set; }

        public string? LogTarget { get; set; }

        [MaxLength(450)]
        public string? UserId { get; set; }

        public DateTime RequestTime { get; set; }
    }
}
