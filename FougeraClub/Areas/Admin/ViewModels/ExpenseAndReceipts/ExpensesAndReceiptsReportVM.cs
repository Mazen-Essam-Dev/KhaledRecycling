using Domain.Entities;
using FougeraClub.Helpers;
using System.ComponentModel.DataAnnotations.Schema;

namespace FougeraClub.Areas.Admin.ViewModels.ExpenseAndReceipts;

public class ExpensesAndReceiptsReportVM
{
    public int? Id { get; set; }
    public int? year { get; set; }
    public int? month { get; set; }
    public decimal BeginningBalance { get; set; }
    public List<ExpensesAndReceiptsReportElementVM>? ExpensesAndReceipts { get; set; }
    public decimal EndingBalance { get; set; }
    public PaginatedList<ExpensesAndReceiptsReportElementVM>? PaginatedExpensesAndReceipts { get; set; }
    public int? AcountantSignatureId { get; set; }
    [ForeignKey(nameof(AcountantSignatureId))]
    public virtual Signature? AcountantSignature { get; set; }

    public int? ManagerSignitureId { get; set; }
    [ForeignKey(nameof(ManagerSignitureId))]
    public virtual Signature? ManagerSignature { get; set; }

    public bool IsAbleToOpen { get; set; } = false;
}




