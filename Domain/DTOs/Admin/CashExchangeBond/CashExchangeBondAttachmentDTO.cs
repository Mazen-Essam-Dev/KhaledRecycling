using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Domain.DTOs.Admin.CashExchangeBond
{
    public class CashExchangeBondAttachmentDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        [MaxLength(300)]
        public string? Path { get; set; }
        public IFormFile? File { get; set; }
    }
}
