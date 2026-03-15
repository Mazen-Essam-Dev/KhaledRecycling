using Application.Helpers;
using Application.Interfaces.Admin;
using DocumentFormat.OpenXml.Spreadsheet;
using Domain.DTOs.Admin.AdministrativeReportQuarterlyAnnual;
using Domain.DTOs.Admin.QuartersReport;
using Domain.Entities.MonthlyAdministrativeReport;
using Domain.Entities.QuartersReport;
using Domain.Enums;
using Domain.Resources;
using Infrastructure.Repositories.InterfacesDB;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Application.Services.Admin
{
    public class SMSForSendingOTPService : ISMSForSendingOTPService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _env;
        private readonly string FileName = "AdministrativeReportQuarterlyAnnual";
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISMSService _SMSService;


        public SMSForSendingOTPService(IUnitOfWork unitOfWork, IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor, ISMSService sMSService)
        {
            _unitOfWork = unitOfWork;
            _env = env;
            _httpContextAccessor = httpContextAccessor;
            _SMSService = sMSService;
        }

       
        // Send SMS OTP  --> Signature/OTP for Trainer and Manager
        public async Task<(bool,string)> SendOtpSMSAsync(string code)
        {
            // Get the UserEMail of User Logged in
            var User = _httpContextAccessor.HttpContext?.User;
            var UserId = User?.GetUserId();

            var thisUser = await _unitOfWork.Users.GetByIdAsync(x => x.Id == UserId);
            if (thisUser == null || thisUser.PhoneNumber == null) return (false," ");

            //string to = "971559153004"; // Replace with dynamic value later
            string to = thisUser.PhoneNumber;
            string body = /*"Hello, this is a Fougera Club OTP"*/ Resource1.HellothisFougeraClubOTPMessage + " (otp) : " + code;
            var result = await _SMSService.SendSMSAsync(to, body);

            if (result.success)
            {
                return (true,"done");
            }
            else
            {
                return (false,result.message);
            }
        }

  
    }
}
