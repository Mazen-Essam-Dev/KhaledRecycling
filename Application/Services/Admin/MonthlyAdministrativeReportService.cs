using Application.Helpers;
using Application.Interfaces.Admin;
using Domain.Entities.MonthlyAdministrativeReport;
using Domain.Enums;
using Infrastructure.Repositories.InterfacesDB;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Application.Services.Admin
{
    public class MonthlyAdministrativeReportService : IMonthlyAdministrativeReportService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _env;
        private readonly ISMSService _SMSService;
        private readonly ISMSForSendingOTPService _SMSForSendingOTPService;
        private readonly string FileName = "MonthlyAdministrativeReports";

        public MonthlyAdministrativeReportService(
            IUnitOfWork unitOfWork,
            IWebHostEnvironment env,
            IHttpContextAccessor httpContextAccessor, ISMSService sMSService, ISMSForSendingOTPService sMSForSendingOTPService)
        {
            _unitOfWork = unitOfWork;
            _env = env;
            _httpContextAccessor = httpContextAccessor;
            _SMSService = sMSService;
            _SMSForSendingOTPService = sMSForSendingOTPService;
        }

        public async Task<IEnumerable<MonthlyAdministrativeReport>> GetAllAsync()
        {
            return await _unitOfWork.MonthlyAdministrativeReports.GetAllAsync(c=>c.ManagerSignature , c => c.TrainerSignature);
        }

        public async Task<MonthlyAdministrativeReport?> GetByIdAsync(int id)
        {
            return await _unitOfWork.MonthlyAdministrativeReports
                .GetByIdAsync(
                    e => e.Id == id,
                    e => e.Details,
                    e => e.TrainerSignature,
                    e => e.ManagerSignature
                );
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

            var report = await _unitOfWork.MonthlyAdministrativeReports.GetByIdAsync(e => e.Id == id);
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

            if (role == "trainer")
                report.TrainerSignitureId = latestSignature.Id;
            else if (role == "manager")
                report.ManagerSignitureId = latestSignature.Id;
            else
                return (false, "Invalid role");

            _unitOfWork.MonthlyAdministrativeReports.Update(report);
            await _unitOfWork.CompleteAsync();
            return (true, null);
        }


        public async Task<IEnumerable<int>> GetAllYearsInDb()
        {
            var allRecords = await _unitOfWork.MonthlyAdministrativeReports.GetAllAsync();
            var allYears = allRecords
                .Where(e => e.Date.HasValue)
                .Select(e => e.Date.Value.Year)
                .Distinct()
                .OrderBy(y => y)
                .ToList();

            return allYears;
        }

        public async Task<int> AddAsync(MonthlyAdministrativeReport entity)
        {
            #region add images
            //entity.Image1Path = entity.Image1 == null ? "" : await FileHelper.SaveImageAsync(entity.Image1, FileName);
            //entity.Image2Path = entity.Image2 == null ? "" : await FileHelper.SaveImageAsync(entity.Image2, FileName);
            //entity.Image3Path = entity.Image3 == null ? "" : await FileHelper.SaveImageAsync(entity.Image3, FileName);
            //entity.Image4Path = entity.Image4 == null ? "" : await FileHelper.SaveImageAsync(entity.Image4, FileName);
            #endregion

            var entit = await _unitOfWork.MonthlyAdministrativeReports.AddAsync(entity);
            await _unitOfWork.CompleteAsync();
            return entit.Id;
        }

        public async Task UpdateAsync(MonthlyAdministrativeReport entity)
        {
            var existing = await _unitOfWork.MonthlyAdministrativeReports.GetByIdAsync(e => e.Id == entity.Id, e => e.Details);
            if (existing == null) return;

            #region update images

            //entity.Image1Path = existing.Image1Path;
            //entity.Image2Path = existing.Image2Path;
            //entity.Image3Path = existing.Image3Path;
            //entity.Image4Path = existing.Image4Path;

            //if (entity.Image1 != null)
            //{
            //    FileHelper.DeleteImageFile(existing.Image1Path); // Delete old
            //    entity.Image1Path = await FileHelper.SaveImageAsync(entity.Image1, FileName); // Save new
            //}
            //if (entity.Image2 != null)
            //{
            //    FileHelper.DeleteImageFile(existing.Image2Path); // Delete old
            //    entity.Image2Path = await FileHelper.SaveImageAsync(entity.Image2, FileName); // Save new
            //}
            //if (entity.Image3 != null)
            //{
            //    FileHelper.DeleteImageFile(existing.Image3Path); // Delete old
            //    entity.Image3Path = await FileHelper.SaveImageAsync(entity.Image3, FileName); // Save new
            //}
            //if (entity.Image4 != null)
            //{
            //    FileHelper.DeleteImageFile(existing.Image4Path); // Delete old
            //    entity.Image4Path = await FileHelper.SaveImageAsync(entity.Image4, FileName); // Save new
            //}
            #endregion

            existing.Details = entity.Details;
            _unitOfWork.MonthlyAdministrativeReports.UpdateValues(existing, entity);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _unitOfWork.MonthlyAdministrativeReports.GetByIdAsync(id);
            if (entity != null)
            {
                // Delete image from disk
                FileHelper.DeleteImageFile(entity.Image1Path); // Delete old
                FileHelper.DeleteImageFile(entity.Image2Path); // Delete old
                FileHelper.DeleteImageFile(entity.Image3Path); // Delete old
                FileHelper.DeleteImageFile(entity.Image4Path); // Delete old
                FileHelper.DeleteImageFile(entity.Image5Path); // Delete old
                FileHelper.DeleteImageFile(entity.Image6Path); // Delete old

                _unitOfWork.MonthlyAdministrativeReportDetails.RemoveRange(entity.Details);
                _unitOfWork.MonthlyAdministrativeReports.Delete(entity);
                await _unitOfWork.CompleteAsync();
            }
        }

        public async Task<bool> CheckIsMonthRegistedBefore(int id, DateOnly? dateOnly, MonthlyAdministrativeReportType type)
        {
            if (dateOnly == null) { return true; }
            var allPreviousWithMonth = await _unitOfWork.MonthlyAdministrativeReports.GetAllAsync(x => x.Date != null && x.Date.Value.Year == dateOnly.Value.Year && x.Date.Value.Month == dateOnly.Value.Month && x.Type == type && x.Id != id);
            if (allPreviousWithMonth != null && allPreviousWithMonth.Count() > 0) { return true; }
            return false;
        }

    }
}
