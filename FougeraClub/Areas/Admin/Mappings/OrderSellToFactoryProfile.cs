using AutoMapper;
using Domain.Entities.Waste;
using KhaledTeamRecycling.Areas.Admin.ViewModels.OrderSellToFactory;

namespace KhaledTeamRecycling.Areas.Admin.Mappings
{
    public class OrderSellToFactoryProfile : Profile
    {
        public OrderSellToFactoryProfile()
        {
            CreateMap<OrderSellToFactoryVM, Domain.Entities.Waste.OrderSellToFactory>()
                .ForMember(d => d.SubWaste, o => o.Ignore())
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

            CreateMap<Domain.Entities.Waste.OrderSellToFactory, OrderSellToFactoryVM>()
                .ForMember(d => d.MainWastesList, o => o.Ignore())
                .ForMember(d => d.SubWastesList, o => o.Ignore())
                .ForMember(d => d.StatusesList, o => o.Ignore())
                .ForMember(d => d.UsersList, o => o.Ignore())
                .ForMember(d => d.Items, o => o.Ignore())
                .ForMember(d => d.SearchString, o => o.Ignore())
                .ForMember(d => d.MainWasteFilterId, o => o.Ignore())
                .ForMember(d => d.SubWasteFilterId, o => o.Ignore())
                .ForMember(d => d.MainWasteName, o => o.Ignore())
                .ForMember(d => d.SubWasteName, o => o.Ignore())
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
                .ForMember(d => d.TonValue, o => o.Ignore())
                .ForMember(d => d.SellPriceUnit, o => o.Ignore())
                .ForMember(d => d.SellPriceKilo, o => o.Ignore())
                .ForMember(d => d.SellPriceTon, o => o.Ignore());
        }
    }
}
