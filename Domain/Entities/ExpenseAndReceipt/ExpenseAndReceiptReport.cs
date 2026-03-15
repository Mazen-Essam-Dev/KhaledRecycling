using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.ExpenseAndReceipt;

public class ExpenseAndReceiptReport
{
    [Key]
    public int Id { get; set; }
    public decimal BeginningBalance { get; set; }
    public int Month { get; set; }
    public int Year { get; set; }
    public IEnumerable<ExpenseAndReceiptAndOther>? ExpensesOrReceipts { get; set; }
    public decimal EndingBalance { get; set; }

    public int? AcountantSignatureId { get; set; }
    [ForeignKey(nameof(AcountantSignatureId))]
    public virtual Signature? AcountantSignature { get; set; }

    public int? ManagerSignitureId { get; set; }
    [ForeignKey(nameof(ManagerSignitureId))]
    public virtual Signature? ManagerSignature { get; set; }
}

