namespace FougeraClub.Areas.Admin.ViewModels.CashDisbursementVoucher
{
    public class ExchangeProofDetailVM
    {
        public int Id { get; set; }
        public int ExchangeProofId { get; set; }
        public string? AccountNo { get; set; }
        public decimal? CurrentBalance { get; set; }
        public string? Item { get; set; }
        public decimal? Amount { get; set; }
        public string? PaymentType { get; set; }
        public decimal? Balance { get; set; }
    }
}
