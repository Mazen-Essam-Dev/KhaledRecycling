using Domain.Entities.Waste;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace KhaledTeamRecycling.Areas.Admin.ViewModels.OrderSellToFactory
{
    public class OrderSellToFactoryVM
    {
        // index data
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 50;
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }

        public int Id { get; set; }

        [LocalizedRequired("Required")]
        public string? FKUserId { get; set; }
        public string? UserName { get; set; }
        public bool IsFactoryUser { get; set; }
        public List<SelectListItem>? UsersList { get; set; } = new();
        [LocalizedRequired("Required")]
        [LocalizedMaxLength(500, "MaxLength_500")]
        public string? Address { get; set; }

        [LocalizedRequired("Required")]
        public int? FKSubWasteId { get; set; }
        public string? SubWasteName { get; set; }
        public List<SelectListItem>? SubWastesList { get; set; } = new();

        [LocalizedRequired("Required")]
        public int? FKMainWasteId { get; set; }
        public string? MainWasteName { get; set; }
        public List<SelectListItem>? MainWastesList { get; set; } = new();

        // Checkbox selections
        public bool IsUnitsSelected { get; set; }
        public bool IsKilosSelected { get; set; }
        public bool IsTonSelected { get; set; }

        // Input values
        public double? UnitsValue { get; set; }
        public double? KilosValue { get; set; }
        public double? TonValue { get; set; }

        // Hidden price values from subWaste
        public double? BuyPriceUnit { get; set; }
        public double? BuyPriceKilo { get; set; }
        public double? BuyPriceTon { get; set; }

        // Original OrderSellToFactory fields
        public int? CountUnits { get; set; }
        public double? Kilo { get; set; }
        public double? Ton { get; set; }

        public DateTime? OrderDate { get; set; }
        public DateTime? ApprovalDate { get; set; }

        public double? DiscountRatio { get; set; }
        public double? DiscountValue { get; set; }
        public double? Total { get; set; }

        public int? StatusId { get; set; }
        public List<SelectListItem>? StatusesList { get; set; } = new();

        public IEnumerable<Domain.Entities.Waste.OrderSellToFactory>? Items { get; set; }
        public string? SearchString { get; set; }
        public int? MainWasteFilterId { get; set; }
        public int? SubWasteFilterId { get; set; }
        public bool isDisabled { get; set; } = false;
        public bool UsePointsDiscount { get; set; }
        public int MaxDiscountRatioAllowed { get; set; }
    }
}
