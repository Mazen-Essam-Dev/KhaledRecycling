using AutoMapper;
using Domain.Entities;
using FougeraClub.Areas.Member.ViewModels;

namespace FougeraClub.Areas.Member.Mappings
{
    public class CourseProfile : Profile
    {
        public CourseProfile()
        {
            CreateMap<CourseVM, Course>()
            .ForMember(dest => dest.AttachmentPath, opt => opt.Ignore()); // set manually if image uploaded

            CreateMap<Course, CourseVM>()
                .ForMember(dest => dest.Subscribed, opt => opt.Ignore());
        }
    }
}