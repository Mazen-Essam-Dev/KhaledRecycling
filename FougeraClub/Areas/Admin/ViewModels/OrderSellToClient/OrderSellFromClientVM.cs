using Domain.Entities.Product;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace KhaledTeamRecycling.Areas.Admin.ViewModels.OrderSellToClient
{
    public class OrderSellToClientVM
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
        public bool IsClientUser { get; set; }
        public List<SelectListItem>? UsersList { get; set; } = new();
        [LocalizedRequired("Required")]
        [LocalizedMaxLength(500, "MaxLength_500")]
        public string? Address { get; set; }

        [LocalizedRequired("Required")]
        public int? FKSubProductId { get; set; }
        public string? SubProductName { get; set; }
        public List<SelectListItem>? SubProductsList { get; set; } = new();

        [LocalizedRequired("Required")]
        public int? FKMainProductId { get; set; }
        public string? MainProductName { get; set; }
        public List<SelectListItem>? MainProductsList { get; set; } = new();

        // Checkbox selections
        public bool IsUnitsSelected { get; set; }
        public bool IsKilosSelected { get; set; }
        public bool IsTonSelected { get; set; }

        // Input values
        public double? UnitsValue { get; set; }
        public double? KilosValue { get; set; }
        public double? TonValue { get; set; }

        // Hidden price values from subProduct
        public double? SellPriceUnit { get; set; }
        public double? SellPriceKilo { get; set; }
        public double? SellPriceTon { get; set; }

        // Original OrderSellToClient fields
        public int? CountUnits { get; set; }
        public double? Kilo { get; set; }
        public double? Ton { get; set; }

        public DateTime? OrderDate { get; set; }
        public DateTime? ApprovalDate { get; set; }

        public double? DiscountRatio { get; set; }
        public double? DiscountValue { get; set; }
        public double? Total { get; set; }
        public string? StoreNotes { get; set; }

        public int? StatusId { get; set; }
        public List<SelectListItem>? StatusesList { get; set; } = new();

        public IEnumerable<Domain.Entities.Product.OrderSellToClient>? Items { get; set; }
        public string? SearchString { get; set; }
        public int? MainProductFilterId { get; set; }
        public int? SubProductFilterId { get; set; }
        public bool isDisabled { get; set; } = false;
        public bool UsePointsDiscount { get; set; }
        public int MaxDiscountRatioAllowed { get; set; }
    }
}
