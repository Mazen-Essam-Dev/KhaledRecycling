using Domain.Enums;
using KhaledTeamRecycling.Areas.Admin.ViewModels.SMS;

namespace KhaledTeamRecycling.Areas.Admin.ViewModels.AdministrativeReportQuarterlyAnnual
{
    public class OTPRequest : OtpValidationRequest
    {
        public int? Type { get; set; }
        public int? Quarter { get; set; }
        public int? Year { get; set; }
    }
}
