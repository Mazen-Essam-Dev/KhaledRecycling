using AutoMapper;
using Domain.Entities.Product;
using KhaledTeamRecycling.Areas.Admin.ViewModels.OrderSellToClient;

namespace KhaledTeamRecycling.Areas.Admin.Mappings
{
    public class OrderSellToClientProfile : Profile
    {
        public OrderSellToClientProfile()
        {
            CreateMap<OrderSellToClientVM, Domain.Entities.Product.OrderSellToClient>()
                .ForMember(d => d.SubProduct, o => o.Ignore())
                .ForMember(d => d.Status, o => o.Ignore());
            //.ForMember(d => d.IsUnitsSelected, o => o.Ignore())
            //.ForMember(d => d.IsKilosSelected, o => o.Ignore())
            //.ForMember(d => d.IsTonSelected, o => o.Ignore())
            //.ForMember(d => d.UnitsValue, o => o.Ignore())
            //.ForMember(d => d.KilosValue, o => o.Ignore())
            //.ForMember(d => d.TonValue, o => o.Ignore())
            //.ForMember(d => d.BuyPriceUnit, o => o.Ignore())
            //.ForMember(d => d.BuyPriceKilo, o => o.Ignore())
            //.ForMember(d => d.BuyPriceTon, o => o.Ignore());

            CreateMap<Domain.Entities.Product.OrderSellToClient, OrderSellToClientVM>()
                .ForMember(d => d.MainProductsList, o => o.Ignore())
                .ForMember(d => d.SubProductsList, o => o.Ignore())
                .ForMember(d => d.StatusesList, o => o.Ignore())
                .ForMember(d => d.UsersList, o => o.Ignore())
                .ForMember(d => d.Items, o => o.Ignore())
                .ForMember(d => d.SearchString, o => o.Ignore())
                .ForMember(d => d.MainProductFilterId, o => o.Ignore())
                .ForMember(d => d.SubProductFilterId, o => o.Ignore())
                .ForMember(d => d.MainProductName, o => o.Ignore())
                .ForMember(d => d.SubProductName, o => o.Ignore())
                .ForMember(d => d.UserName, o => o.Ignore())
                .ForMember(d => d.CurrentPage, o => o.Ignore())
                .ForMember(d => d.PageSize, o => o.Ignore())
                .ForMember(d => d.TotalPages, o => o.Ignore())
                .ForMember(d => d.TotalCount, o => o.Ignore())
                .ForMember(d => d.HasNextPage, o => o.Ignore())
                .ForMember(d => d.HasPreviousPage, o => o.Ignore())
                .ForMember(d => d.IsUnitsSelected, o => o.Ignore())
                .ForMember(d => d.IsKilosSelected, o => o.Ignore())
                .ForMember(d => d.IsTonSelected, o => o.Ignore())
                .ForMember(d => d.UnitsValue, o => o.Ignore())
                .ForMember(d => d.KilosValue, o => o.Ignore())
                .ForMember(d => d.TonValue, o => o.Ignore());

        }
    }
}
