using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.ExpenseAndReceipt
{
    public class ExpensesGate
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)] // To prevent auto-increment
        public int Id { get; set; }

        [MaxLength(100)]
        public string? NameAr { get; set; }

        [MaxLength(100)]
        public string? NameEn { get; set; }
        public virtual ICollection<ExpenseAndReceiptAndOther> ExpenseAndReceiptAndOthers { get; set; } = new List<ExpenseAndReceiptAndOther>();
    }
}