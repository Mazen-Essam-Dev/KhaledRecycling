using Microsoft.AspNetCore.Http;

namespace Domain.DTOs.Admin.PurchaseOrder
{
    public class PurchaseOrderAttachmentDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public IFormFile? File { get; set; }
    }
}
