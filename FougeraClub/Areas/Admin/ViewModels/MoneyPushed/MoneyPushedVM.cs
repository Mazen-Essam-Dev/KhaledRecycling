using System.ComponentModel.DataAnnotations;
using Domain.Entities;

namespace KhaledTeamRecycling.Areas.Admin.ViewModels.MoneyPushed
{
    public class MoneyPushedVM
    {
        public int Id { get; set; }

        [Display(Name = "المستخدم")]
        public string? FKUserId { get; set; }

        [Display(Name = "المبلغ")]
        [Required(ErrorMessage = "المبلغ مطلوب")]
        [Range(0.01, double.MaxValue, ErrorMessage = "يجب أن يكون المبلغ أكبر من صفر")]
        public decimal? Money { get; set; }

        [Display(Name = "نوع المعاملة")]
        public char? TypeTransaction { get; set; }

        [Display(Name = "تاريخ الإنشاء")]
        public DateTime? CreatedDate { get; set; }

        // Pagination & Search properties
        public IEnumerable<MoneyPushedItemDTO> Items { get; set; } = new List<MoneyPushedItemDTO>();
        public string? SearchString { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 50;
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }
    }

    public class MoneyPushedItemDTO
    {
        public int Id { get; set; }
        public string? FKUserId { get; set; }
        public string? UserName { get; set; }
        public decimal? Money { get; set; }
        public char? TypeTransaction { get; set; }
        public DateTime? CreatedDate { get; set; }
    }
}
