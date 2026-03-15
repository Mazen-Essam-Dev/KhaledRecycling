using Domain.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.ExpenseAndReceipt
{
    public class ReceivingReceipt
    {
        [Key]
        public int Id { get; set; }

        public string? CodeSerial { get; set; }
        public int? itemType { get; set; }

        [ForeignKey(nameof(itemType))]

        public ItemTypeEntity? ItemTypeEntity { get; set; }
        public string? ItemNumber { get; set; }
        public DateOnly Date { get; set; }
        public decimal Amount { get; set; }


        [MaxLength(500)]
        public string? Received { get; set; }
        [MaxLength(500)]
        public string? Payment { get; set; }
        public string? SumOfAmount { get; set; }
        [MaxLength(500)]
        public string? Being { get; set; }


        public decimal FinalAmount { get; set; }
        [MaxLength(500)]
        public string? Notes { get; set; }

        public int? ExpenseId { get; set; }
        [ForeignKey(nameof(ExpenseId))]
        public virtual ExpenseAndReceiptAndOther? ExpenseAndReceiptAndOther { get; set; }
        public int? ManagerSignitureId { get; set; }
        [ForeignKey(nameof(ManagerSignitureId))]
        public virtual Signature? ManagerSignature { get; set; }
    }
}
