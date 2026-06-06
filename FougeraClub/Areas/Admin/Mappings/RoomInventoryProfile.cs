using AutoMapper;
using Domain.Entities.Inventory;
using KhaledTeamRecycling.Areas.Admin.ViewModels.RoomInventory;

namespace KhaledTeamRecycling.Areas.Admin.Mappings
{
    public class RoomInventoryProfile : Profile
    {
        public RoomInventoryProfile()
        {
            CreateMap<RoomInventoryVM, Domain.Entities.Inventory.RoomInventory>()
                .ForMember(d => d.Inventory, o => o.Ignore())
                .ForMember(d => d.SubWaste, o => o.Ignore());

            CreateMap<Domain.Entities.Inventory.RoomInventory, RoomInventoryVM>()
                .ForMember(d => d.InventoriesList, o => o.Ignore())
                .ForMember(d => d.SubWastesList, o => o.Ignore())
                .ForMember(d => d.Items, o => o.Ignore())
                .ForMember(d => d.SearchString, o => o.Ignore())
                .ForMember(d => d.InventoryFilterId, o => o.Ignore())
                .ForMember(d => d.SubWasteFilterId, o => o.Ignore())
                .ForMember(d => d.CurrentPage, o => o.Ignore())
                .ForMember(d => d.PageSize, o => o.Ignore())
                .ForMember(d => d.TotalPages, o => o.Ignore())
                .ForMember(d => d.TotalCount, o => o.Ignore())
                .ForMember(d => d.HasNextPage, o => o.Ignore())
                .ForMember(d => d.HasPreviousPage, o => o.Ignore());
        }
    }
}
