using Domain.Enums;
using FougeraClub.Areas.Admin.ViewModels.SMS;

namespace FougeraClub.Areas.Admin.ViewModels.AdministrativeReportQuarterlyAnnual
{
    public class OTPRequest : OtpValidationRequest
    {
        public int? Type { get; set; }
        public int? Quarter { get; set; }
        public int? Year { get; set; }
    }
}
