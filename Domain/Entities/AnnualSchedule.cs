using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class AnnualSchedule
    {
        public int Id { get; set; }

        [StringLength(200)]
        public string? Item { get; set; }

        [StringLength(1000)]
        public string? Statement { get; set; }

        public decimal? Previous { get; set; }

        public decimal? Current { get; set; }

        public decimal? Record { get; set; }

        [StringLength(200)]
        public string? Status { get; set; }

        [DataType(DataType.Date)]
        public DateTime? Day { get; set; }

        [StringLength(200)]
        public string? year { get; set; }

        // Nullable Foreign Key to AnnualScheduleCategory
        public int? AnnualScheduleCategoryId { get; set; }
        public AnnualScheduleCategory? AnnualScheduleCategory { get; set; }
    }
}
