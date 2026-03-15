using Application.Helpers;
using Application.Interfaces.Admin;
using Domain.DTOs.Admin.CashDisbursementVoucher;
using Domain.Entities.CashDisbursementVoucher;
using Domain.Enums;
using Infrastructure.Repositories.InterfacesDB;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Application.Services.Admin
{
    public class CashDisbursementVoucherService : ICashDisbursementVoucherService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISMSService _SMSService;
        private readonly ISMSForSendingOTPService _SMSForSendingOTPService;
        private readonly string FileName = "CashDisbursementVoucherAttachments";


        public CashDisbursementVoucherService(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, ISMSService sMSService, ISMSForSendingOTPService sMSForSendingOTPService)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _SMSService = sMSService;
            _SMSForSendingOTPService = sMSForSendingOTPService;
        }

        public async Task<int> GetLastExchangeProofDocumentNo()
        {
            var lastDocumentNo = await _unitOfWork.ExchangeProofs.Table
               .Select(x => x.DocumentNo)
               .Where(x => x.HasValue)
               .OrderByDescending(x => x.Value)
               .FirstOrDefaultAsync();

            return (lastDocumentNo ?? 0) + 1;
        }
        public async Task<IEnumerable<CashDisbursementVoucher>> GetAllAsync()
        {
            return await _unitOfWork.CashDisbursementVouchers.GetAllAsync();
        }
        public async Task<CashDisbursementVoucher?> GetByIdAsync(int id)
        {
            return await _unitOfWork.CashDisbursementVouchers
                .GetByIdAsync(e => e.Id == id, e => e.Details, e => e.DisbursementRequestSignature, e => e.AcknowledgmentReceipt_A_Sig, e => e.DisbursementRequestAccountantSignature);
        }
        public async Task<ExchangeProof?> GetExchangeProofByIdAsyncSingle(int id)
        {
                return await _unitOfWork.ExchangeProofs.GetByIdAsync(e => e.Id == id);
        }
        public async Task<int> AddAsync(CashDisbursementVoucher entity)
        {
            var id = await _unitOfWork.CashDisbursementVouchers.AddAsyncThenGetLastId(entity);
            await _unitOfWork.CompleteAsync();
            return id;
        }
        public async Task UpdateAsync(CashDisbursementVoucher entity)
        {
            var existing = await _unitOfWork.CashDisbursementVouchers.GetByIdAsync(e => e.Id == entity.Id, e => e.Details);
            if (existing == null) return;

            existing.Details = entity.Details;
            _unitOfWork.CashDisbursementVouchers.UpdateValues(existing, entity);
            await _unitOfWork.CompleteAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var entity = await _unitOfWork.CashDisbursementVouchers.GetByIdAsync(id);
            if (entity != null)
            {
                _unitOfWork.CashDisbursementVoucherDetails.RemoveRange(entity.Details ?? []);
                _unitOfWork.CashDisbursementVouchers.Delete(entity);
                await _unitOfWork.CompleteAsync();
            }
        }

        #region exchange proof
        public async Task<(int?, int?)> GetExchangeProofSignaturesAsync(int cashDisbursementVoucherId)
        {
            var result = await _unitOfWork.ExchangeProofs.GetByColumnAsync(e => e.CashDisbursementVoucherId == cashDisbursementVoucherId);
            return result != null ? (result.AccountantSignitureId??0, result.ManagerSignitureId??0) : (0, 0);

        }
        public async Task<ExchangeProof?> GetExchangeProofByIdAsync(int cashDisbursementVoucherId)
        {
            return await _unitOfWork.ExchangeProofs.GetByColumnAsync(e => e.CashDisbursementVoucherId == cashDisbursementVoucherId, e => e.AccountantSigniture, e => e.ManagerSigniture, e => e.Details);
        }
        public async Task AddAsync(ExchangeProof entity)
        {
            // Avoid double-inserting details: extract them, save parent, then insert details once.
            var details = entity.Details?.ToList();
            entity.Details = null;

            var id = await _unitOfWork.ExchangeProofs.AddAsyncThenGetLastId(entity);

            if (details != null && details.Any())
            {
                var repo = _unitOfWork.GetRepository<ExchangeProofDetail>();
                foreach (var d in details)
                {
                    // ensure we don't attempt to insert explicit identity values
                    d.Id = 0;
                    d.ExchangeProofId = id;
                }
                await repo.AddAsync(details);
            }
            await _unitOfWork.CompleteAsync();
        }
        public async Task UpdateAsync(ExchangeProof entity)
        {
            var existing = await _unitOfWork.ExchangeProofs.GetByIdAsync(e => e.Id == entity.Id, e => e.Details);
            if (existing == null) return;

            // Extract new details and null them to prevent EF auto-insert
            var newDetails = entity.Details?.ToList();
            entity.Details = null;

            // remove old details
            if (existing.Details != null && existing.Details.Any())
            {
                var repo = _unitOfWork.GetRepository<ExchangeProofDetail>();
                repo.RemoveRange(existing.Details);
            }

            // Update parent fields
            _unitOfWork.ExchangeProofs.UpdateValues(existing, entity);

            // add new details if present
            if (newDetails != null && newDetails.Any())
            {
                var repo = _unitOfWork.GetRepository<ExchangeProofDetail>();
                foreach (var d in newDetails)
                {
                    // ensure new details don't carry an identity value
                    d.Id = 0;
                    d.ExchangeProofId = existing.Id;
                }
                await repo.AddAsync(newDetails);
            }

            await _unitOfWork.CompleteAsync();

        }
        #endregion

        #region Acknowledgment Receipt
        public async Task<bool> GetAcknowledgmentReceiptSignaturesAsync(int cashDisbursementVoucherId)
        {
            var result = await _unitOfWork.AcknowledgmentReceipts.GetByColumnAsync(e => e.CashDisbursementVoucherId == cashDisbursementVoucherId);
            return (result != null && result.AccountantSignitureId != null);
        }
        public async Task<AcknowledgmentReceipt?> GetAcknowledgmentReceiptByIdAsync(int cashDisbursementVoucherId)
        {
            return await _unitOfWork.AcknowledgmentReceipts.GetByColumnAsync(e => e.CashDisbursementVoucherId == cashDisbursementVoucherId, e => e.AccountantSigniture);
        }
        public async Task AddAsync(AcknowledgmentReceipt entity)
        {
            var id = await _unitOfWork.AcknowledgmentReceipts.AddAsyncThenGetLastId(entity);
            await _unitOfWork.CompleteAsync();
        }
        public async Task UpdateAsync(AcknowledgmentReceipt entity)
        {
            var existing = await _unitOfWork.AcknowledgmentReceipts.GetByIdAsync(e => e.Id == entity.Id);
            if (existing == null) return;

            _unitOfWork.AcknowledgmentReceipts.UpdateValues(existing, entity);
            await _unitOfWork.CompleteAsync();

        }
        #endregion
        public async Task<bool> SendOtpAsync(int id, string role)
        {
            var (status, code) = await OTPHelper.SaveOtpAsync(_httpContextAccessor, _unitOfWork);

            if (status == false) return false;

            var resultStatus = await _SMSForSendingOTPService.SendOtpSMSAsync(code);

            return resultStatus.Item1;
        }

        public async Task<(bool success, string? message)> DisbursementRequestValidateOtpAsync(int id, string code, string role, ClaimsPrincipal user)
        {
            var success = await OTPHelper.ValidateOtpAsync(_httpContextAccessor, _unitOfWork, code);
            if (!success)
                return (false, "Invalid OTP");

            var report = await _unitOfWork.CashDisbursementVouchers.GetByIdAsync(e => e.Id == id);
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
                report.DisbursementRequestSignatureId = latestSignature.Id;
            else if (role == "accountant")
                report.DisbursementRequestSignatureAccountantId = latestSignature.Id;
            else
                return (false, "Invalid role");

            _unitOfWork.CashDisbursementVouchers.Update(report);
            await _unitOfWork.CompleteAsync();
            return (true, null);
        }

        public async Task<(bool success, string? message)> ExchangeProofValidateOtpAsync(int id, string code, string role, ClaimsPrincipal user)
        {
            var success = await OTPHelper.ValidateOtpAsync(_httpContextAccessor, _unitOfWork, code);
            if (!success)
                return (false, "Invalid OTP");

            var report = await _unitOfWork.ExchangeProofs.GetByIdAsync(e => e.Id == id);
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

            if (role == "accountant")
                report.AccountantSignitureId = latestSignature.Id;
            else if (role == "manager")
                report.ManagerSignitureId = latestSignature.Id;
            else
                return (false, "Invalid role");

            _unitOfWork.ExchangeProofs.Update(report);
            await _unitOfWork.CompleteAsync();
            return (true, null);
        }

        public async Task<(bool success, string? message)> AcknowledgmentReceiptValidateOtpAsync(int id, string code, string role, ClaimsPrincipal user)
        {
            var success = await OTPHelper.ValidateOtpAsync(_httpContextAccessor, _unitOfWork, code);
            if (!success)
                return (false, "Invalid OTP");

            var report = await _unitOfWork.AcknowledgmentReceipts.GetByIdAsync(e => e.Id == id);
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

            if (role == "accountant")
                report.AccountantSignitureId = latestSignature.Id;
            else
                return (false, "Invalid role");

            _unitOfWork.AcknowledgmentReceipts.Update(report);
            await _unitOfWork.CompleteAsync();
            return (true, null);
        }
        public async Task UploadAttachmentsAsync(CashDisbursementVoucherAttachmentsDTO model)
        {
            // remove old files
            var oldFiles = await _unitOfWork.CashDisbursementVoucherAttachments.GetAllAsync(e => e.CashDisbursementVoucherId == model.CashDisbursementVoucherId);
            foreach (var oldFile in oldFiles)
            {
                if (FileHelper.IsFileExist(oldFile.Path) && model.Attachments != null && !model.Attachments.Any(a => a.Path == oldFile.Path))
                {
                    FileHelper.DeleteImageFile(oldFile.Path);
                    _unitOfWork.CashDisbursementVoucherAttachments.Delete(oldFile);
                }
            }

            //save new files
            var existing = new CashDisbursementVoucherAttachment();
            foreach (var attachment in model.Attachments ?? [])
            {
                if (!oldFiles.Any() || !oldFiles.Any(a => a.Path == attachment.Path))
                {
                    if (attachment.File != null)
                    {
                        existing.Path = await FileHelper.SaveImageAsync(attachment.File, FileName);
                    }
                    existing.Name = attachment.Name;
                    existing.CashDisbursementVoucherId = model.CashDisbursementVoucherId;
                    await _unitOfWork.CashDisbursementVoucherAttachments.AddAsync(existing);
                    existing = new CashDisbursementVoucherAttachment();
                }
            }

            await _unitOfWork.CompleteAsync();

        }

        public async Task<IEnumerable<CashDisbursementVoucherAttachment>> GetAttachmentsAsync(int cashDisbursementVoucherId)
        {
            var attachments = await _unitOfWork.CashDisbursementVoucherAttachments.GetAllAsync(e => e.CashDisbursementVoucherId == cashDisbursementVoucherId);
            return attachments;
        }



    }
}
