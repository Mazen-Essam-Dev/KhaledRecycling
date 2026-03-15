using AutoMapper;
using Domain.Entities;
using FougeraClub.Areas.Admin.ViewModels.Activity;

namespace FougeraClub.Areas.Admin.Mappings
{
    public class ActivityProfile : Profile
    {
        public ActivityProfile()
        {
            CreateMap<ActivityVM, Activity>().ReverseMap();
        }
    }
}
