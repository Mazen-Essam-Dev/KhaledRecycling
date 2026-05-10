using AutoMapper;
using Domain.Entities.Waste;
using KhaledTeamRecycling.Areas.Admin.ViewModels.SubWaste;

namespace KhaledTeamRecycling.Areas.Admin.Mappings
{
    public class SubWasteProfile : Profile
    {
        public SubWasteProfile()
        {
            CreateMap<SubWasteVM, SubWaste>()
                .ForMember(d => d.MainWaste, o => o.Ignore())
                .ForMember(d => d.Status, o => o.Ignore());

            CreateMap<SubWaste, SubWasteVM>()
                .ForMember(d => d.MainWastesList, o => o.Ignore())
                .ForMember(d => d.StatusesList, o => o.Ignore())
                .ForMember(d => d.Items, o => o.Ignore())
                .ForMember(d => d.SearchString, o => o.Ignore())
                .ForMember(d => d.MainWasteFilterId, o => o.Ignore())
                .ForMember(d => d.MainWasteName, o => o.Ignore())
                .ForMember(d => d.CurrentPage, o => o.Ignore())
                .ForMember(d => d.PageSize, o => o.Ignore())
                .ForMember(d => d.TotalPages, o => o.Ignore())
                .ForMember(d => d.TotalCount, o => o.Ignore())
                .ForMember(d => d.HasNextPage, o => o.Ignore())
                .ForMember(d => d.HasPreviousPage, o => o.Ignore());
        }
    }
}
