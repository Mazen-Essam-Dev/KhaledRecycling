using AutoMapper;
using Domain.Entities;
using Domain.Entities.MonthlyAdministrativeReport;
using FougeraClub.Areas.Admin.ViewModels.Engineer;
using FougeraClub.Areas.Admin.ViewModels.MonthlyAdministrativeReport;

namespace FougeraClub.Areas.Admin.Mappings
{
    public class EngineerProfile : Profile
    {
        public EngineerProfile()
        {
            CreateMap<EngineerVM, Engineer>()
            .ForMember(dest => dest.ProfileImagePath, opt => opt.Ignore()); // set manually if image uploaded

            CreateMap<Engineer, EngineerVM>();
        }
    }
}