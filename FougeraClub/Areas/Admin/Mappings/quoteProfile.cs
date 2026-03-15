using AutoMapper;
using Domain.DTOs.Admin.CarService;
using Domain.Entities;
using Domain.Entities.quote;
using FougeraClub.Areas.Admin.ViewModels.CarServices;
using FougeraClub.Areas.Admin.ViewModels.MaterialOrder;
using FougeraClub.Areas.Admin.ViewModels.quote;


namespace FougeraClub.Areas.Admin.Mappings
{
    public class quoteProfile : Profile
    {
        public quoteProfile()
        {
            // Parent
            CreateMap<quoteVM, quote>()
                .ReverseMap();

            // Child
            CreateMap<quotesItemVM, quotesItem>()
                .ReverseMap();

            //  Child Child
            CreateMap<ItemSupplierVM, ItemSupplier>()
                .ReverseMap();
        }
    }
}