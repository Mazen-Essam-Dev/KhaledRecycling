using Microsoft.AspNetCore.Http;

namespace FougeraClub.Areas.Admin.ViewModels.PurchaseOrder
{
    public class PurchaseOrderAttachmentVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
        public IFormFile? File { get; set; }
    }
}
