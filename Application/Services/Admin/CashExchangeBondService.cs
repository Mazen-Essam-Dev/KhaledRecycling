using Application.Helpers;
using Application.Interfaces.Admin;
using Domain.DTOs.Admin.CashExchangeBond;
using Domain.Entities.CashExchangeBond;
using Domain.Enums;
using Infrastructure.Repositories.InterfacesDB;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Application.Services.Admin
{
    public class CashExchangeBondService : ICashExchangeBondService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _env;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISMSService _SMSService;
        private readonly ISMSForSendingOTPService _SMSForSendingOTPService;
        private readonly string FileName = "CashExchangeBonds";
        private readonly string AttachmentFileName = "CashExchangeBondAttachments";


        public CashExchangeBondService(IUnitOfWork unitOfWork, IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor, ISMSService sMSService, ISMSForSendingOTPService sMSForSendingOTPService)
        {
            _unitOfWork = unitOfWork;
            _env = env;
            _httpContextAccessor = httpContextAccessor;
            _SMSService = sMSService;
            _SMSForSendingOTPService = sMSForSendingOTPService;
        }

        public async Task<IEnumerable<CashExchangeBond>> GetAllAsync()
        {
            return await _unitOfWork.CashExchangeBonds.GetAllAsync( x => x.AccountantSigniture, x => x.ManagerSignature);
        }

        public async Task<CashExchangeBond?> GetByIdAsync(int id)
        {
            return await _unitOfWork.CashExchangeBonds
                .GetByIdAsync(e => e.Id == id, e => e.Details, x => x.AccountantSigniture, x => x.ManagerSignature);
        }

        public async Task<IEnumerable<int>> GetAllYearsInDb()
        {
            var allRecords = await _unitOfWork.CashExchangeBonds.GetAllAsync();
            var allYears = allRecords
                .Where(e => e.Date.HasValue)
                .Select(e => e.Date.Value.Year)
                .Distinct()
                .OrderBy(y => y)
                .ToList();

            return allYears;
        }

        public async Task<int> AddAsync(CashExchangeBond entity)
        {

            var entit = await _unitOfWork.CashExchangeBonds.AddAsync(entity);
            await _unitOfWork.CompleteAsync();
            return entit.Id;
        }

        public async Task UpdateAsync(CashExchangeBond entity)
        {
            var existing = await _unitOfWork.CashExchangeBonds.GetByIdAsync(e => e.Id == entity.Id, e => e.Details);
            if (existing == null) return;

            existing.Details = entity.Details;
            _unitOfWork.CashExchangeBonds.UpdateValues(existing, entity);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _unitOfWork.CashExchangeBonds.GetByIdAsync(id);
            if (entity != null)
            {
                _unitOfWork.CashExchangeBondDetails.RemoveRange(entity.Details);
                _unitOfWork.CashExchangeBonds.Delete(entity);
                await _unitOfWork.CompleteAsync();
            }
        }

        public async Task<bool> CheckIsMonthRegistedBefore(int id, DateOnly? dateOnly)
        {
            if (dateOnly == null) { return true; }
            var allPreviousWithMonth = await _unitOfWork.CashExchangeBonds.GetAllAsync(x => x.Date != null && x.Date.Value.Year == dateOnly.Value.Year && x.Date.Value.Month == dateOnly.Value.Month && x.Id != id);
            if (allPreviousWithMonth != null && allPreviousWithMonth.Count() > 0) { return true; }
            return false;
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

            var report = await _unitOfWork.CashExchangeBonds.GetByIdAsync(e => e.Id == id);
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
            else if (role == "accountant")
                report.AccountantSignitureId = latestSignature.Id;
            else
                return (false, "Invalid role");

            _unitOfWork.CashExchangeBonds.Update(report);
            await _unitOfWork.CompleteAsync();
            return (true, null);
        }

        public async Task UploadAttachmentsAsync(CashExchangeBondAttachmentsDTO model)
        {
            // remove old files
            var oldFiles = await _unitOfWork.CashExchangeBondAttachments.GetAllAsync(e => e.CashExchangeBondId == model.CashExchangeBondId);
            foreach (var oldFile in oldFiles)
            {
                if (FileHelper.IsFileExist(oldFile.Path) && model.Attachments != null && !model.Attachments.Any(a => a.Path == oldFile.Path))
                {
                    FileHelper.DeleteImageFile(oldFile.Path);
                    _unitOfWork.CashExchangeBondAttachments.Delete(oldFile);
                }
            }

            //save new files
            var existing = new CashExchangeBondAttachment();
            foreach (var attachment in model.Attachments ?? [])
            {
                if (!oldFiles.Any() || !oldFiles.Any(a => a.Path == attachment.Path))
                {
                    if (attachment.File != null)
                    {
                        existing.Path = await FileHelper.SaveImageAsync(attachment.File, AttachmentFileName);
                    }
                    existing.Name = attachment.Name;
                    existing.CashExchangeBondId = model.CashExchangeBondId;
                    await _unitOfWork.CashExchangeBondAttachments.AddAsync(existing);
                    existing = new CashExchangeBondAttachment();
                }
            }

            await _unitOfWork.CompleteAsync();

        }

        public async Task<CashExchangeBondAttachmentsDTO> GetAttachmentsAsync(int cashExchangeBondId)
        {
            var attachments = await _unitOfWork.CashExchangeBondAttachments.GetAllAsync(e => e.CashExchangeBondId == cashExchangeBondId);
            return new CashExchangeBondAttachmentsDTO
            {
                CashExchangeBondId = cashExchangeBondId,
                Attachments = attachments.Select(a => new CashExchangeBondAttachmentDTO
                {
                    Id = a.Id,
                    Name = a.Name,
                    Path = a.Path
                }).ToList()
            };
        }


    }
}
