using Domain.Enums;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.ExpenseAndReceipt
{
    public class ExpenseAndReceiptAndOther
    {
        [Key]
        public int Id { get; set; }
        public int? ItemType { get; set; }
        public virtual ItemTypeEntity? ItemTypeEntity { get; set; }
        public string? ItemNumber { get; set; }

        public int? BudgetItemId { get; set; }
        public virtual BudgetItem.BudgetItem? BudgetItem { get; set; }

        public int? ExpensesGateId { get; set; }
        [ForeignKey(nameof(ExpensesGateId))]
        public virtual ExpensesGate? ExpensesGate { get; set; }
        public int? ExpensesSourceId { get; set; }
        [ForeignKey(nameof(ExpensesSourceId))]
        public virtual ExpensesSource? ExpensesSource { get; set; }

        public int? SupplierId { get; set; }
        public virtual Supplier? Supplier { get; set; }

        public decimal? Amount { get; set; } = null;
        public decimal? Vat { get; set; } = null;
        public DateOnly Date { get; set; }
        //public DateTime CreatedAt { get; set; }
        public string? Notes { get; set; }

        [MaxLength(300)]
        public string? AttachmentPath { get; set; }
        [NotMapped]
        public IFormFile? Attachment { get; set; }


        public int? ExpensesReportId { get; set; }
        public int? ExpenseAndReceiptReportId { get; set; }
    }
}
