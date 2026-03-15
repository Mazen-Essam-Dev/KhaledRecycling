using AutoMapper;
using Domain.DTOs;
using Domain.Entities;
using FougeraClub.Areas.Admin.ViewModels.MaintenanceClub;

namespace FougeraClub.Areas.Admin.Mappings
{
    public class MaintenanceClubProfile : Profile
    {
        public MaintenanceClubProfile()
        {
            CreateMap<MaintenanceClubVM, MaintenanceClub>().ReverseMap();
            
            // Attachment mappings
            CreateMap<MaintenanceClubAttachment, MaintenanceClubAttachmentDTO>().ReverseMap();
            CreateMap<MaintenanceClubAttachmentDTO, MaintenanceClubAttachmentVM>().ReverseMap();
            CreateMap<MaintenanceClubAttachmentsDTO, MaintenanceClubAttachmentsVM>().ReverseMap();
        }
    }
}
