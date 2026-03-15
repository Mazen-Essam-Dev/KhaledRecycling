using Domain.HelperForDomain;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class Signature
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }
        [MaxLength(300)]
        public string? ImagePath { get; set; }
        [NotMapped]
        public IFormFile? SignatureFile { get; set; }
        public DateOnly CreatedAt { get; set; } = DateOnly.FromDateTime(AppDubaiTime1.Now);
    }
}
