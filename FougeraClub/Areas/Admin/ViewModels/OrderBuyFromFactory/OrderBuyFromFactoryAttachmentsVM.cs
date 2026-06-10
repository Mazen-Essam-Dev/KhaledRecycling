using Domain.Entities.Product;
using Domain.Entities.Waste;
using System.ComponentModel.DataAnnotations;

namespace KhaledTeamRecycling.Areas.Admin.ViewModels.OrderBuyFromFactory
{
    public class OrderBuyFromFactoryAttachmentsVM
    {
        public int OrderBuyFromFactoryId { get; set; }
        public string ReturnAction { get; set; } = "AddEdit";
        public List<OrderBuyFromFactoryAttachment>? Attachments { get; set; } = new();
    }
}
