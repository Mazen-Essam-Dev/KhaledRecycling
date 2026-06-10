using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.Product
{
    public class OrderBuyFromFactoryAttachment
    {
        public int Id { get; set; }
        [MaxLength(100)]
        public string? Name { get; set; }
        [MaxLength(300)]
        public string? Path { get; set; }
        [NotMapped]
        public IFormFile? File { get; set; }
        public int OrderBuyFromFactoryId { get; set; }
    }
}
