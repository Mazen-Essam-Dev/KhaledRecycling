using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities.ExpenseAndReceipt
{
    public class ItemTypeEntity
    {
        [Key]
        public int Id { get; set; }
        public string? NameAr { get; set; }
        public string? NameEn { get; set; }

        public virtual ICollection<ExpenseAndReceiptAndOther>? ExpenseAndReceiptAndOthers { get; set; } = new List<ExpenseAndReceiptAndOther>();
    }
}
