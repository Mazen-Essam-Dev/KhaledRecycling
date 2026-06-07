using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace KhaledTeamRecycling.Areas.Admin.ViewModels.RoomGallery
{
    public class RoomGalleryVM
    {
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 50;
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }

        public int Id { get; set; }

        [LocalizedRequired("Required"), LocalizedMaxLength(100, "MaxLength_100")]
        public string? GenCode { get; set; }

        [LocalizedRequired("Required")]
        public int? FkSubProduct { get; set; }

        [LocalizedRequired("Required")]
        public double? MaxKilo { get; set; }

        [LocalizedRequired("Required")]
        public int? FkGallery { get; set; }

        [LocalizedMaxLength(500, "MaxLength_500")]
        public string? Description { get; set; }

        public List<SelectListItem>? GalleriesList { get; set; } = new();
        public List<SelectListItem>? SubProductsList { get; set; } = new();
        public IEnumerable<Domain.Entities.Gallary.RoomGallery>? Items { get; set; }
        public string? SearchString { get; set; }
        public int? GalleryFilterId { get; set; }
        public int? SubProductFilterId { get; set; }
    }
}
