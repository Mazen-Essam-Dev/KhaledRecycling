namespace FougeraClub.Areas.Admin.ViewModels.SMS
{
    public class OtpValidationRequest
    {
        public long Id { get; set; }
        public int? Year { get; set; }
        public int? Month { get; set; }
        public string Code { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}
