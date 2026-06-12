using Domain.Entities.Waste;
using System.ComponentModel.DataAnnotations;

namespace KhaledTeamRecycling.Areas.Admin.ViewModels.OrderSellToFactory
{
    public class OrderSellToFactoryAttachmentsVM
    {
        public int OrderSellToFactoryId { get; set; }
        public string ReturnAction { get; set; } = "AddEdit";
        public List<OrderSellToFactoryAttachment>? Attachments { get; set; } = new();
        public bool? IsClientUser { get; set; }
    }
}
