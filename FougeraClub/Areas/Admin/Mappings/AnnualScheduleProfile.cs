using AutoMapper;
using Domain.Entities;
using FougeraClub.Areas.Admin.ViewModels.AnnualSchedule;
using FougeraClub.Helpers;

namespace FougeraClub.Areas.Admin.Mappings
{
    public class AnnualScheduleProfile : Profile
    {
        public AnnualScheduleProfile()
        {
            CreateMap<AnnualScheduleVM, AnnualSchedule>().ReverseMap()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom((src, dest, destMember, context) => 
                {
                    if (src.AnnualScheduleCategory == null)
                        return null;
                    
                    var currentLang = SessionHelper.GetCurrentLanguage();
                    return currentLang == "ar" ? src.AnnualScheduleCategory.NameAr : src.AnnualScheduleCategory.NameEn;
                }));
        }
    }
}
