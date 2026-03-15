using Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace FougeraClub.Areas.Admin.ViewModels.CashDisbursementVoucher
{
    public class ExchangeProofVM
    {
        // Full names for display
        public string? AccountantFullName { get; set; }
        public string? ManagerFullName { get; set; }

        public int Id { get; set; }
        public int CashDisbursementVoucherId { get; set; }
        [LocalizedRequired("Required")]
        public int? DocumentNo { get; set; }
        [LocalizedRequired("Required")]
        public string? BasedOn { get; set; }
        [LocalizedRequired("Required")]
        public string? ItWas { get; set; }
        [LocalizedRequired("Required")]
        public string? Amount { get; set; }
        [LocalizedRequired("Required")]
        public string? By { get; set; }
        [LocalizedRequired("Required")]
        public string? Bank { get; set; }
        [LocalizedRequired("Required")]
        [Column(TypeName = "date")]
        public DateOnly? Date { get; set; }
        [LocalizedRequired("Required")]
        public string? Being { get; set; }
        public int? AccountantSignitureId { get; set; }
        [ForeignKey(nameof(AccountantSignitureId))]
        public virtual Signature? AccountantSigniture { get; set; }
        public int? ManagerSignitureId { get; set; }
        [ForeignKey(nameof(ManagerSignitureId))]
        public virtual Signature? ManagerSigniture { get; set; }

        public bool isSavedFull { get; set; } = false;
        public List<ExchangeProofDetailVM>? Details { get; set; }

        public bool IsUserLogedInISManager { get; set; } = false;

    }
}
