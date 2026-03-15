using Domain.Entities;
using Domain.Entities.CashDisbursementVoucher;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations.Schema;

namespace FougeraClub.Areas.Admin.ViewModels.CashDisbursementVoucher;

public class CashDisbursementVoucherVM
{
    public int Id { get; set; }
    public string? TypeText { get; set; }
    [LocalizedRequired("Required")]
    public int? Type { get; set; }
    public IEnumerable<SelectListItem>? TypeEnumList { get; set; }
    [LocalizedRequired("Required")]

    [Column(TypeName = "date")]
    public DateOnly? Date { get; set; }
    public string? Month { get; set; }
    [LocalizedRequired("Required")]
    public int? DocumentNo { get; set; }
    public string? Description { get; set; }
    public decimal? TotalAmount { get; set; }
    public virtual ICollection<CashDisbursementVoucherDetail>? Details { get; set; }

    public int? DisbursementRequestSignatureAccountantId { get; set; }
    public Signature? DisbursementRequestAccountantSignature { get; set; }

    public int? DisbursementRequestSignatureId { get; set; }
    public Signature? DisbursementRequestSignature { get; set; }
    public bool ExchangeProofApprovalDone { get; set; }
    public bool AcknowledgmentReceiptApprovalDone { get; set; }

    // Manager full name for display
    public string? ManagerFullName { get; set; }
    public string? AccountantFullName { get; set; }
}
