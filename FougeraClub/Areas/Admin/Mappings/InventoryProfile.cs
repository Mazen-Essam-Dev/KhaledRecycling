using AutoMapper;
using KhaledTeamRecycling.Areas.Admin.ViewModels.Inventory;

namespace KhaledTeamRecycling.Areas.Admin.Mappings
{
    public class InventoryProfile : Profile
    {
        public InventoryProfile()
        {
            CreateMap<InventoryVM, Domain.Entities.Inventory.Inventory>()
                .ForMember(d => d.Rooms, o => o.Ignore());

            CreateMap<Domain.Entities.Inventory.Inventory, InventoryVM>()
                .ForMember(d => d.Items, o => o.Ignore())
                .ForMember(d => d.SearchString, o => o.Ignore())
                .ForMember(d => d.CurrentPage, o => o.Ignore())
                .ForMember(d => d.PageSize, o => o.Ignore())
                .ForMember(d => d.TotalPages, o => o.Ignore())
                .ForMember(d => d.TotalCount, o => o.Ignore())
                .ForMember(d => d.HasNextPage, o => o.Ignore())
                .ForMember(d => d.HasPreviousPage, o => o.Ignore());
        }
    }
}
