using System.ComponentModel.DataAnnotations;

namespace FougeraClub.Areas.Admin.ViewModels.CashDisbursementVoucher
{
    public class CashDisbursementVoucherAttachmentVM
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        [MaxLength(300)]
        public string? Path { get; set; }
        public IFormFile? File { get; set; }
    }
}
