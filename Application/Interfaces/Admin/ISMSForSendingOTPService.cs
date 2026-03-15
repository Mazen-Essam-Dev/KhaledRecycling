using Domain.DTOs.Admin.AdministrativeReportQuarterlyAnnual;
using Domain.DTOs.Admin.QuartersReport;
using Domain.Entities.MonthlyAdministrativeReport;
using Domain.Enums;
using System.Security.Claims;

namespace Application.Interfaces.Admin
{
    public interface ISMSForSendingOTPService
    {  
        Task<(bool,string)> SendOtpSMSAsync(string code);

    }

}
