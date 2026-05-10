using AutoMapper;
using Domain.Entities.Product;
using KhaledTeamRecycling.Areas.Admin.ViewModels.SubProduct;

namespace KhaledTeamRecycling.Areas.Admin.Mappings
{
    public class SubProductProfile : Profile
    {
        public SubProductProfile()
        {
            CreateMap<SubProductVM, SubProduct>()
                .ForMember(d => d.MainProduct, o => o.Ignore())
                .ForMember(d => d.Status, o => o.Ignore());

            CreateMap<SubProduct, SubProductVM>()
                .ForMember(d => d.MainProductsList, o => o.Ignore())
                .ForMember(d => d.StatusesList, o => o.Ignore())
                .ForMember(d => d.Items, o => o.Ignore())
                .ForMember(d => d.SearchString, o => o.Ignore())
                .ForMember(d => d.MainProductFilterId, o => o.Ignore())
                .ForMember(d => d.MainProductName, o => o.Ignore())
                .ForMember(d => d.CurrentPage, o => o.Ignore())
                .ForMember(d => d.PageSize, o => o.Ignore())
                .ForMember(d => d.TotalPages, o => o.Ignore())
                .ForMember(d => d.TotalCount, o => o.Ignore())
                .ForMember(d => d.HasNextPage, o => o.Ignore())
                .ForMember(d => d.HasPreviousPage, o => o.Ignore());
        }
    }
}
