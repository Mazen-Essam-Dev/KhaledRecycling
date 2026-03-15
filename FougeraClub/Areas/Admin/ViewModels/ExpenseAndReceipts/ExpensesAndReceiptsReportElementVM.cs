namespace FougeraClub.Areas.Admin.ViewModels.ExpenseAndReceipts;

public class ExpensesAndReceiptsReportElementVM
{
    public DateOnly Date { get; set; }
    public string? Notes { get; set; }
    public string? SupplierName { get; set; }
    public decimal Withdrawal { get; set; }
    public decimal Deposit { get; set; }
    public decimal Balance { get; set; }
}




