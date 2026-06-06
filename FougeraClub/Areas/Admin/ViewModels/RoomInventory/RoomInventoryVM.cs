using Domain.Entities.Inventory;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace KhaledTeamRecycling.Areas.Admin.ViewModels.RoomInventory
{
    public class RoomInventoryVM
    {
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 50;
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }

        public int Id { get; set; }

        [LocalizedRequired("Required"), LocalizedMaxLength(100, "MaxLength_100")]
        public string? GenCode { get; set; }

        [LocalizedRequired("Required")]
        public int? FKSubWaste { get; set; }

        [LocalizedRequired("Required")]
        public double? MaxKilo { get; set; }

        [LocalizedRequired("Required")]
        public int? FkInventory { get; set; }

        [LocalizedMaxLength(500, "MaxLength_500")]
        public string? Description { get; set; }

        public List<SelectListItem>? InventoriesList { get; set; } = new();
        public List<SelectListItem>? SubWastesList { get; set; } = new();
        public IEnumerable<Domain.Entities.Inventory.RoomInventory>? Items { get; set; }
        public string? SearchString { get; set; }
        public int? InventoryFilterId { get; set; }
        public int? SubWasteFilterId { get; set; }
    }
}
