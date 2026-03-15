namespace FougeraClub.Areas.Admin.ViewModels.ExpenseAndReceipts;

public class ExpensesReportElementVM
{
    public string? ItemNumber { get; set; }
    public DateOnly Date { get; set; }
    public string? Notes { get; set; }
    public string? SupplierName { get; set; }
    public string? BudgetItem { get; set; }
    public decimal Amount { get; set; }
    public decimal Vat { get; set; }
    public decimal AmountWithVat { get; set; }
    public decimal Balance { get; set; }
}




