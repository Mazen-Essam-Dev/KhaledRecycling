using AutoMapper;
using Domain.DTOs.Admin.CarService;
using Domain.Entities;
using Domain.Entities.MaterialOrder;
using FougeraClub.Areas.Admin.ViewModels.CarServices;
using FougeraClub.Areas.Admin.ViewModels.MaterialOrder;


namespace FougeraClub.Areas.Admin.Mappings
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