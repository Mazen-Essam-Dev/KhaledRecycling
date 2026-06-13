using System.ComponentModel.DataAnnotations;

namespace KhaledTeamRecycling.Areas.Admin.ViewModels.Gallery
{
    public class GalleryVM
    {
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 50;
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }

        public int Id { get; set; }

        [LocalizedRequired("Required"), LocalizedMaxLength(100, "MaxLength_100")]
        public string? Name { get; set; }

        [LocalizedRequired("Required")]
        public int? NoRooms { get; set; }

        [LocalizedMaxLength(500, "MaxLength_500")]
        public string? Location { get; set; }

        [LocalizedMaxLength(500, "MaxLength_500")]
        public string? Description { get; set; }

        public IEnumerable<Domain.Entities.Gallery.Gallery>? Items { get; set; }
        public string? SearchString { get; set; }
    }
}
