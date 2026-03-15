using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.PurchaseOrder
{
    public class PurchaseOrderAttachment
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(300)]
        public string Path { get; set; }

        [NotMapped]
        public IFormFile? File { get; set; }

        // Foreign Key
        public long PurchaseOrderId { get; set; }

        [ForeignKey(nameof(PurchaseOrderId))]
        public PurchaseOrder PurchaseOrder { get; set; }
    }
}
