using AutoMapper;
using Domain.Entities;
using FougeraClub.Areas.Admin.ViewModels.ArchivingDocument;
using FougeraClub.Areas.Admin.ViewModels.MaintenanceClub;

namespace FougeraClub.Areas.Admin.Mappings
{
    public class ArchivingDocumentProfile : Profile
    {
        public ArchivingDocumentProfile()
        {
            CreateMap<ArchivingDocument, ArchivingDocumentVM>()
                .ForMember(dest => dest.DocumentCategoryName, opt => opt.MapFrom(src => src.DocumentCategory != null ? (src.DocumentCategory.NameAr ?? src.DocumentCategory.NameEn) : null))
                .ReverseMap();
        }
    }
}
