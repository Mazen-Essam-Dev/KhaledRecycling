using AutoMapper;
using Domain.Entities;
using Domain.DTOs.Admin.Car;
using FougeraClub.Areas.Admin.ViewModels.Cars;

namespace FougeraClub.Areas.Admin.Mappings
{
    public class CarProfile : Profile
    {
        public CarProfile()
        {
            CreateMap<CarVM, Car>()
            .ForMember(dest => dest.AttachmentPath, opt => opt.Ignore()); // set manually if image uploaded

            CreateMap<Car, CarVM>();

            // Attachments
            CreateMap<CarAttachmentsVM, CarAttachmentsDTO>().ReverseMap();
            CreateMap<CarAttachmentVM, CarAttachmentDTO>().ReverseMap();
            CreateMap<CarAttachment, CarAttachmentVM>().ReverseMap();
            CreateMap<CarAttachment, CarAttachmentDTO>().ReverseMap();
        }
    }
}