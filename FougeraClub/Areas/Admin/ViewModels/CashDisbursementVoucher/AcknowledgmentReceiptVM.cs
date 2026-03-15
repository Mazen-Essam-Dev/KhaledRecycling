using Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace FougeraClub.Areas.Admin.ViewModels.CashDisbursementVoucher
{
    public class AcknowledgmentReceiptVM
    {
        // Accountant full name for display
        public string? AccountantFullName { get; set; }

        public int Id { get; set; }
        public int CashDisbursementVoucherId { get; set; }
        [LocalizedRequired("Required")]
        public string? Title { get; set; }
        [LocalizedRequired("Required")]
        public string? ThisTo { get; set; }
        [LocalizedRequired("Required")]
        public string? Recived { get; set; }
        [LocalizedRequired("Required")]
        public int? PaymentVoucherNo { get; set; }
        [LocalizedRequired("Required")]
        public string? Amount { get; set; }
        [LocalizedRequired("Required")]
        public string? Cheque { get; set; }
        [LocalizedRequired("Required")]
        [Column(TypeName = "date")]
        public DateOnly? Date { get; set; }
        [LocalizedRequired("Required")]
        public string? Bank { get; set; }
        [LocalizedRequired("Required")]
        public string? Being { get; set; }
        public string? AcknowledgmentText { get; set; }
        [Column(TypeName = "date")]
        public DateOnly? HeaderDate { get; set; }
        public string? HeaderMonth { get; set; }
        public int? AccountantSignitureId { get; set; }
        public Signature? AccountantSigniture { get; set; }

        public bool isSavedFull { get; set; } = false;
    }
}
