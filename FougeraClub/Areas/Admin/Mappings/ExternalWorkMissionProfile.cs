using AutoMapper;
using Domain.Entities;
using FougeraClub.Areas.Admin.ViewModels.ExternalWorkMission;

namespace FougeraClub.Areas.Admin.Mappings
{
    public class ExternalWorkMissionProfile : Profile
    {
        public ExternalWorkMissionProfile()
        {
            CreateMap<ExternalWorkMissionVM, ExternalWorkMission>().ReverseMap();
        }
    }
}