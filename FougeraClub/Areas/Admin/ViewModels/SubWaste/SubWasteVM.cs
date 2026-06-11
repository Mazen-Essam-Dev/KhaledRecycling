using Domain.Entities.Waste;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace KhaledTeamRecycling.Areas.Admin.ViewModels.SubWaste
{
    public class SubWasteVM
    {
        // index data
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 50;
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }

        public int Id { get; set; }

        [LocalizedRequired("Required"), LocalizedMaxLength(100, "MaxLength_100")]
        [RegularExpression(@"^[\u0621-\u064A0-9 ]+$", ErrorMessage = "يجب أن يحتوي على حروف عربية وأرقام فقط")]
        public string? NameAr { get; set; }

        [LocalizedMaxLength(100, "MaxLength_100")]
        [RegularExpression(@"^[a-zA-Z0-9 ]+$", ErrorMessage = "must contain only English letters and numbers")]
        public string? NameEn { get; set; }

        [LocalizedRequired("Required")]
        public int? FKMainWasteId { get; set; }

        public string? MainWasteName { get; set; }
        public List<SelectListItem>? MainWastesList { get; set; } = new();
        [LocalizedRequired("Required")]

        public double? SellPriceUnit { get; set; }
        [LocalizedRequired("Required")]

        public double? SellPriceKilo { get; set; }
        [LocalizedRequired("Required")]

        public double? SellPriceTon { get; set; }
        [LocalizedRequired("Required")]

        public double? BuyPriceUnit { get; set; }
        [LocalizedRequired("Required")]

        public double? BuyPriceKilo { get; set; }
        [LocalizedRequired("Required")]

        public double? BuyPriceTon { get; set; }
        [LocalizedRequired("Required")]

        public double? RatioCountFor1Kilo { get; set; }

        public Char? StatusChar { get; set; }
        public int? StatusId { get; set; }
        public List<SelectListItem>? StatusesList { get; set; } = new();

        public IEnumerable<Domain.Entities.Waste.SubWaste>? Items { get; set; }
        public string? SearchString { get; set; }
        public int? MainWasteFilterId { get; set; }
    }
}
