using AutoMapper;
using Domain.Entities;
using FougeraClub.Areas.Admin.ViewModels.Suppliers;

namespace FougeraClub.Areas.Admin.Mappings
{
    public class SupplierProfile : Profile
    {
        public SupplierProfile()
        {
            CreateMap<SupplierVM, Supplier>().ReverseMap();
        }
    }
}
