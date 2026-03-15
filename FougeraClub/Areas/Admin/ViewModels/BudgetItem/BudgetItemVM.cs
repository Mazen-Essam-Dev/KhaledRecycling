using System.ComponentModel.DataAnnotations;

namespace FougeraClub.Areas.Admin.ViewModels.BudgetItem
{
    public class BudgetItemVM
    {
        public int Id { get; set; }
        public int? ItemNumber { get; set; }
        [LocalizedRequired("Required")]
        [StringLength(200)]
        public string? ItemTitle { get; set; }
        public bool HasRelatedExpenseOrReceipt { get; set; }

    }
}