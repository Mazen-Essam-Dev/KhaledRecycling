using Application.Helpers;
using Application.Interfaces.Admin;
using Infrastructure.Repositories.InterfacesDB;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Application.Services.Admin
{
    public class ScientificProjectsService : IScientificProjectsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _env;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISMSService _SMSService;
        private readonly ISMSForSendingOTPService _SMSForSendingOTPService;
        private readonly string FileName = "ScientificProjects";


        public ScientificProjectsService(IUnitOfWork unitOfWork, IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor, ISMSService sMSService, ISMSForSendingOTPService sMSForSendingOTPService)
        {
            _unitOfWork = unitOfWork;
            _env = env;
            _httpContextAccessor = httpContextAccessor;
            _SMSService = sMSService;
            _SMSForSendingOTPService = sMSForSendingOTPService;
        }

        // Signature/OTP for Trainer and Manager
        public async Task<bool> SendOtpAsync(int id, string role)
        {
            var (status, code) = await OTPHelper.SaveOtpAsync(_httpContextAccessor, _unitOfWork);

            if (status == false) return false;

            var resultStatus = await _SMSForSendingOTPService.SendOtpSMSAsync(code);

            return resultStatus.Item1;
        }

        public async Task<(bool success, string? message)> ValidateOtpAsync(int id, string code, string role, System.Security.Claims.ClaimsPrincipal user)
        {
            var success = await OTPHelper.ValidateOtpAsync(_httpContextAccessor, _unitOfWork, code);
            if (!success)
                return (false, "Invalid OTP");

            var report = await _unitOfWork.ScientificProjects.GetByIdAsync(e => e.Id == id);
            if (report == null)
                return (false, "Report not found");

            var userId = user.GetUserId();
            var allSignatures = await _unitOfWork.Signatures.GetAllAsync();
            var latestSignature = allSignatures
                .Where(s => s.UserId == userId)
                .OrderByDescending(s => s.CreatedAt)
                .FirstOrDefault();
            if (latestSignature == null)
                return (false, "Signature not found");

            if (role == "manager")
                report.ManagerSignitureId = latestSignature.Id;
            else if (role == "activityMonitor")
                report.ActivityMonitorSignitureId = latestSignature.Id;
            else if (role == "Trainer")
                report.Trainer1SignitureId = latestSignature.Id;
            else
                return (false, "Invalid role");

            _unitOfWork.ScientificProjects.Update(report);
            await _unitOfWork.CompleteAsync();
            return (true, null);
        }


    }
}
