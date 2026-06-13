using AutoMapper;
using KhaledTeamRecycling.Areas.Admin.ViewModels.RoomGallery;

namespace KhaledTeamRecycling.Areas.Admin.Mappings
{
    public class RoomGalleryProfile : Profile
    {
        public RoomGalleryProfile()
        {
            CreateMap<RoomGalleryVM, Domain.Entities.Gallery.RoomGallery>()
                .ForMember(d => d.Gallery, o => o.Ignore())
                .ForMember(d => d.SubProduct, o => o.Ignore())
                .ForMember(d => d.MaxUnit, o => o.MapFrom(s => s.MaxUnits));

            CreateMap<Domain.Entities.Gallery.RoomGallery, RoomGalleryVM>()
                .ForMember(d => d.GalleriesList, o => o.Ignore())
                .ForMember(d => d.SubProductsList, o => o.Ignore())
                .ForMember(d => d.Items, o => o.Ignore())
                .ForMember(d => d.SearchString, o => o.Ignore())
                .ForMember(d => d.GalleryFilterId, o => o.Ignore())
                .ForMember(d => d.SubProductFilterId, o => o.Ignore())
                .ForMember(d => d.CurrentPage, o => o.Ignore())
                .ForMember(d => d.PageSize, o => o.Ignore())
                .ForMember(d => d.TotalPages, o => o.Ignore())
                .ForMember(d => d.TotalCount, o => o.Ignore())
                .ForMember(d => d.HasNextPage, o => o.Ignore())
                .ForMember(d => d.HasPreviousPage, o => o.Ignore())
                .ForMember(d => d.MaxUnits, o => o.MapFrom(s => s.MaxUnit));
        }
    }
}
