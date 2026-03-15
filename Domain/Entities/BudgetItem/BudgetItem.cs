using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.BudgetItem
{
    public class BudgetItem
    {
        [Key]
        public int Id { get; set; }
        public int? ItemNumber { get; set; }
        [StringLength(200)]
        public string? ItemTitle { get; set; }
    }
}
