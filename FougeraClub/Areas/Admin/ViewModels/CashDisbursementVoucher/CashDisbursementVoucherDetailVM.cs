using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FougeraClub.Areas.Admin.ViewModels.CashDisbursementVoucher;
public class CashDisbursementVoucherDetailVM
{
    public int Id { get; set; }
    public int CashDisbursementVoucherId { get; set; }
    public CashDisbursementVoucherVM? CashDisbursementVoucher { get; set; }
    public string? DocumentNo { get; set; }
    [Column(TypeName = "date")]
    public DateOnly? DocumentDate { get; set; }
    [MaxLength(500)]
    public string? Notes { get; set; }
    public decimal? Amount { get; set; }
}