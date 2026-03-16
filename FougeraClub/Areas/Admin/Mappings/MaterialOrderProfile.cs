using AutoMapper;
using Domain.Entities;
using Domain.Entities.MaterialOrder;
using KhaledTeamRecycling.Areas.Admin.ViewModels.MaterialOrder;


namespace KhaledTeamRecycling.Areas.Admin.Mappings
{
    public class MaterialOrderProfile : Profile
    {
        public MaterialOrderProfile()
        {
            // Parent
            CreateMap<MaterialOrderVM, MaterialOrder>()
                .ReverseMap();

            // Child
            CreateMap<MaterialOrderItemVM, MaterialOrderItem>()
                .ReverseMap();


        }
    }
}