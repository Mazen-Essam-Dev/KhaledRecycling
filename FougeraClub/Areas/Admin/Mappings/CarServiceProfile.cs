using AutoMapper;
using Domain.Entities;
using Domain.DTOs.Admin.CarService;
using FougeraClub.Areas.Admin.ViewModels.CarServices;

namespace FougeraClub.Areas.Admin.Mappings
{
    public class CarServiceProfile : Profile
    {
        public CarServiceProfile()
        {
            CreateMap<CarServiceVM, CarServiceEntity>()
            .ForMember(dest => dest.AttachmentPath, opt => opt.Ignore()); // set manually if image uploaded

            CreateMap<CarServiceEntity, CarServiceVM>();

            // Attachments
            CreateMap<CarServiceAttachmentsVM, CarServiceAttachmentsDTO>().ReverseMap();
            CreateMap<CarServiceAttachmentVM, CarServiceAttachmentDTO>().ReverseMap();
            CreateMap<CarServiceAttachment, CarServiceAttachmentVM>().ReverseMap();
            CreateMap<CarServiceAttachment, CarServiceAttachmentDTO>().ReverseMap();
        }
    }
}