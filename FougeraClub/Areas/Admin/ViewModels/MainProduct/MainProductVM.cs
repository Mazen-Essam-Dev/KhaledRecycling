using Domain.Entities.Product;
using System.ComponentModel.DataAnnotations;

namespace KhaledTeamRecycling.Areas.Admin.ViewModels.MainProduct
{
    public class MainProductVM
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

        // Keep list data in same VM to match project style and make future rename simple.
        public IEnumerable<Domain.Entities.Product.MainProduct>? Items { get; set; }
        public string? SearchString { get; set; }
    }
}
