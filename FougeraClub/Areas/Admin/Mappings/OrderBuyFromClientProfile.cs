using AutoMapper;
using Domain.Entities.Waste;
using KhaledTeamRecycling.Areas.Admin.ViewModels.OrderBuyFromClient;

namespace KhaledTeamRecycling.Areas.Admin.Mappings
{
    public class OrderBuyFromClientProfile : Profile
    {
        public OrderBuyFromClientProfile()
        {
            CreateMap<OrderBuyFromClientVM, Domain.Entities.Waste.OrderBuyFromClient>()
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

            CreateMap<Domain.Entities.Waste.OrderBuyFromClient, OrderBuyFromClientVM>()
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
                .ForMember(d => d.BuyPriceUnit, o => o.Ignore())
                .ForMember(d => d.BuyPriceKilo, o => o.Ignore())
                .ForMember(d => d.BuyPriceTon, o => o.Ignore());
        }
    }
}
