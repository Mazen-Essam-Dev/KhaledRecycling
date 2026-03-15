using System.ComponentModel.DataAnnotations;

namespace FougeraClub.Areas.Admin.ViewModels.Account
{
    public class SignatureVM
    {
        public string UserId { get; set; }
        [MaxLength(300)]
        public string? ImagePath { get; set; }
        public IFormFile? SignatureFile { get; set; }
        public DateOnly CreatedAt { get; set; } 

    }
}
