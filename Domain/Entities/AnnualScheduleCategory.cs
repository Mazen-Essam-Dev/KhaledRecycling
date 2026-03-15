using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class AnnualScheduleCategory
    {
        public int Id { get; set; }

        [StringLength(200)]
        public string? NameAr { get; set; }

        [StringLength(200)]
        public string? NameEn { get; set; }

        public ICollection<AnnualSchedule>? AnnualSchedules { get; set; }
    }
}
