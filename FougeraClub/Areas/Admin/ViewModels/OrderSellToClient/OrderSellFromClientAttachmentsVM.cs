using Domain.Entities.Product;
using System.ComponentModel.DataAnnotations;

namespace KhaledTeamRecycling.Areas.Admin.ViewModels.OrderSellToClient
{
    public class OrderSellToClientAttachmentsVM
    {
        public int OrderSellToClientId { get; set; }
        public string ReturnAction { get; set; } = "AddEdit";
        public List<OrderSellToClientAttachment>? Attachments { get; set; } = new();
    }
}
