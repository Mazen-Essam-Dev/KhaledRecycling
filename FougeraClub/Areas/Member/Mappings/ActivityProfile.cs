using AutoMapper;
using Domain.Entities;
using FougeraClub.Areas.Member.ViewModels;

namespace FougeraClub.Areas.Member.Mappings
{
    public class ActivityProfile : Profile
    {
        public ActivityProfile()
        {
            CreateMap<ActivityVM, Activity>().ReverseMap();
        }
    }
}
