using AutoMapper;
using Domain.DTOs.Admin.Course;
using Domain.Entities;
using FougeraClub.Areas.Admin.ViewModels.Course;

namespace FougeraClub.Areas.Admin.Mappings
{
    public class CourseProfile : Profile
    {
        public CourseProfile()
        {
            CreateMap<CourseVM, Course>()
            .ForMember(dest => dest.AttachmentPath, opt => opt.Ignore()); // set manually if image uploaded

            CreateMap<Course, CourseVM>()
                .ForMember(dest => dest.IsSubscribed, opt => opt.Ignore());

            CreateMap<CourseSubVM, Course>()
            .ForMember(dest => dest.AttachmentPath, opt => opt.Ignore()); // set manually if image uploaded

            CreateMap<Course, CourseSubVM>()
                .ForMember(dest => dest.Subscribed, opt => opt.Ignore());

            CreateMap<SubscribedMemberCourseVM, SubscribedMemberCourseDTO>().ReverseMap();
            CreateMap<CertificateDTO, CertificateVM>().ReverseMap();
            CreateMap<AcceptanceDTO, AcceptanceVM>().ReverseMap();
        }
    }
}