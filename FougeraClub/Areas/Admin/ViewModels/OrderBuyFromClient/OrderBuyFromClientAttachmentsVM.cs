using Domain.Entities.Waste;
using System.ComponentModel.DataAnnotations;

namespace KhaledTeamRecycling.Areas.Admin.ViewModels.OrderBuyFromClient
{
    public class OrderBuyFromClientAttachmentsVM
    {
        public int OrderBuyFromClientId { get; set; }
        public string ReturnAction { get; set; } = "AddEdit";
        public List<OrderBuyFromClientAttachment>? Attachments { get; set; } = new();
    }
}
