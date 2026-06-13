using AutoMapper;
using KhaledTeamRecycling.Areas.Admin.ViewModels.Gallery;

namespace KhaledTeamRecycling.Areas.Admin.Mappings
{
    public class GalleryProfile : Profile
    {
        public GalleryProfile()
        {
            CreateMap<GalleryVM, Domain.Entities.Gallery.Gallery>()
                .ForMember(d => d.Rooms, o => o.Ignore());

            CreateMap<Domain.Entities.Gallery.Gallery, GalleryVM>()
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
