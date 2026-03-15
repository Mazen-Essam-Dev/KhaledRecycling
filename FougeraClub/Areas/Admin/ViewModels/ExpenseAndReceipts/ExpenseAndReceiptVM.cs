using Application.Helpers;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace FougeraClub.Areas.Admin.ViewModels.ExpenseAndReceipts;

public class ExpenseAndReceiptVM : IValidatableObject
{
    public int Id { get; set; }
    public int ItemTypeId { get; set; }
    public ItemType ItemType { get; set; }
    public IEnumerable<SelectListItem>? ItemTypesList { get; set; }
    [LocalizedRequired("Required")]
    public string? ItemNumber { get; set; } = null;
    [LocalizedRequired("Required")]
    public int? BudgetItemId { get; set; }
    public Domain.Entities.BudgetItem.BudgetItem? BudgetItem { get; set; }
    public List<SelectListItem>? BudgetItemsList { get; set; }
    public int? ExpensesGateId { get; set; }
    public IEnumerable<SelectListItem>? ExpensesGateList { get; set; }
    [LocalizedRequired("Required")]
    public int? ExpensesSourceId { get; set; }
    public IEnumerable<SelectListItem>? ExpensesSourcesList { get; set; }
    [LocalizedRequired("Required")]
    public int? SupplierId { get; set; }
    public Supplier? Supplier { get; set; }
    public List<SelectListItem>? SuppliersList { get; set; }
    [LocalizedRequired("Required")]
    public decimal? Amount { get; set; } = null;
    public decimal? Vat { get; set; } = null;
    [LocalizedRequired("Required")]
    public DateOnly? Date { get; set; } = DateOnly.FromDateTime(AppDubaiTime.Now);

    [LocalizedRequired("Required")]
    public string? Notes { get; set; }


    [MaxLength(300)]
    public string? AttachmentPath { get; set; }
    public IFormFile? Attachment { get; set; }

    // Temporary uploaded file
    public string? Attachment_TempFilePath { get; set; }

    // For Edit: store old file from DB
    public string? Attachment_OldPath { get; set; }

    public decimal? TotalAmount { get; set; } = null;
    public decimal? TotalVAT { get; set; } = null;
    public decimal? TotalAmountWithVAT { get; set; } = null;

    public bool isSigned { get; set; } = false;
    public int idForReciptSign { get; set; } = -2;


    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        // Allow empty/null Date to be handled by other validators if needed
        if (Date.HasValue)
        {
            // Only enforce the date rule for "MiscellaneousExpenses" source
            if (ExpensesSourceId.HasValue && ExpensesSourceId.Value != (int)Domain.Enums.ExpensesSourceEnum.MiscellaneousExpenses)
            {
                var min = new DateOnly(2025, 12, 1);
                if (Date.Value < min)
                {
                    yield return new ValidationResult("التاريخ يجب أن لا يكون قبل 1 ديسمبر 2025", new[] { nameof(Date) });
                }
            }
        }
    }
}




