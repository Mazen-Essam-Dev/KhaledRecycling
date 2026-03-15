using Application.Helpers;
using Domain.Entities;
using Domain.Enums;

namespace FougeraClub.Areas.Admin.ViewModels.ExpenseAndReceipts;

public class ReceivingReceiptVM
{
    public int Id { get; set; }

    public ItemType? itemType { get; set; }
    public string? CodeSerial { get; set; }

    [LocalizedRequired("Required")]
    public string? ItemNumber { get; set; }
    [LocalizedRequired("Required")]
    public DateOnly Date { get; set; } = DateOnly.FromDateTime(AppDubaiTime.Now);
    [LocalizedRequired("Required")]
    public decimal? Amount { get; set; }


    [LocalizedRequired("Required")]
    public string? Received { get; set; }
    [LocalizedRequired("Required")]
    public string? Payment { get; set; }
    [LocalizedRequired("Required")]
    public string? SumOfAmount { get; set; }
    [LocalizedRequired("Required")]
    public string? Being { get; set; }


    [LocalizedRequired("Required")]
    public decimal? FinalAmount { get; set; }
    public string? Notes { get; set; }

    public int? ExpenseId { get; set; }
    public int? ManagerSignatureId { get; set; }
    public Signature? ManagerSignature { get; set; }
    public bool isSavedFull { get; set; } = false;

}




