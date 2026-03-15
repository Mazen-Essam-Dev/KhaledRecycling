namespace Domain.DTOs.Admin.ExpenseAndReceipt;

public class ReceiptDTO
{
    public string? ItemNumber { get; set; }
    public decimal Amount { get; set; }
    public DateOnly Date { get; set; }

    public decimal FinalAmount { get; set; }
    public string? Notes { get; set; }
}



