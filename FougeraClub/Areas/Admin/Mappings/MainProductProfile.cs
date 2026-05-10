using AutoMapper;
using Domain.Entities.Product;
using KhaledTeamRecycling.Areas.Admin.ViewModels.MainProduct;

namespace KhaledTeamRecycling.Areas.Admin.Mappings
{
    public class MainProductProfile : Profile
    {
        public MainProductProfile()
        {
            CreateMap<MainProductVM, MainProduct>().ReverseMap();
        }
    }
}
