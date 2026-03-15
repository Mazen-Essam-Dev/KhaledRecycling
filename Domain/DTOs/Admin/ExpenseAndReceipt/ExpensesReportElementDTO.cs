using Domain.Entities.BudgetItem;

namespace Domain.DTOs.Admin.ExpenseAndReceipt;

public class ExpensesReportElementDTO
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




