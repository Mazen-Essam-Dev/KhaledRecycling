using Domain.Entities.BudgetItem;

namespace Domain.DTOs.Admin.ExpenseAndReceipt;

public class ExpensesAndReceiptsReportElementDTO
{
    public DateOnly Date { get; set; }
    public string? Notes { get; set; }
    public string? SupplierName { get; set; }
    public decimal Withdrawal { get; set; }
    public decimal Deposit { get; set; }
    public decimal Balance { get; set; }
}




