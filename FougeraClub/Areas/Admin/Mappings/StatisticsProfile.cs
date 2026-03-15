using AutoMapper;
using Domain.DTOs.Admin;
using Domain.Entities;
using FougeraClub.Areas.Admin.ViewModels.Statistics;

namespace FougeraClub.Areas.Admin.Mappings
{
    public class StatisticsProfile : Profile
    {
        public StatisticsProfile()
        {
            CreateMap<StatisticsVM, StatisticsDTO>().ReverseMap();
            CreateMap<AgeGroupStat, AgeGroupStatVM>().ReverseMap();
            CreateMap<CourseCategoryStat, CourseCategoryStatVM>().ReverseMap();
            CreateMap<GenderStat, GenderStatVM>().ReverseMap();
            CreateMap<ResidentsAndCitizensStat, ResidentsAndCitizensStatVM>().ReverseMap();
        }
    }
}
