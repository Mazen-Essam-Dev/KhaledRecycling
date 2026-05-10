using AutoMapper;
using Domain.Entities.Waste;
using KhaledTeamRecycling.Areas.Admin.ViewModels.MainWaste;

namespace KhaledTeamRecycling.Areas.Admin.Mappings
{
    public class MainWasteProfile : Profile
    {
        public MainWasteProfile()
        {
            CreateMap<MainWasteVM, MainWaste>().ReverseMap();
        }
    }
}
