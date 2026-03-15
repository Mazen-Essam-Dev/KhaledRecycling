using Application.Helpers;
using Application.Interfaces.Admin;
using Domain.DTOs.Admin.SalaryManagement;
using Domain.Entities.EstimatedBudgetForExternalParticipation;
using Domain.Enums;
using Infrastructure.Repositories.InterfacesDB;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Admin
{
    public class EstimatedBudgetForExternalParticipationService : IEstimatedBudgetForExternalParticipationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISMSService _SMSService;
        private readonly ISMSForSendingOTPService _SMSForSendingOTPService;
        private readonly string FileName = "EstimatedBudgetForExternalParticipations";

        public EstimatedBudgetForExternalParticipationService(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, ISMSService sMSService, ISMSForSendingOTPService sMSForSendingOTPService)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _SMSService = sMSService;
            _SMSForSendingOTPService = sMSForSendingOTPService;
        }
        public async Task<bool> SendOtpAsync()
        {
            var (status, code) = await OTPHelper.SaveOtpAsync(_httpContextAccessor, _unitOfWork);

            if (status == false) return false;

            var resultStatus = await _SMSForSendingOTPService.SendOtpSMSAsync(code);

            return resultStatus.Item1;
        }

        public async Task<bool> ValidateOtpAsync(int id, string code)
        {
            var success = await OTPHelper.ValidateOtpAsync(_httpContextAccessor, _unitOfWork, code);

            if (success)
            {
                var entity = await GetByIdAsync(id);
                var userId = _httpContextAccessor.HttpContext?.User.GetUserId();
                var allSignatures = await _unitOfWork.Signatures.GetAllAsync();
                var latestSignature = allSignatures
                    .Where(s => s.UserId == userId)
                    .OrderByDescending(s => s.CreatedAt)
                    .FirstOrDefault();
                if (latestSignature != null && entity != null)
                {
                    entity.SignatureIdApproved = latestSignature.Id;
                    _unitOfWork.EstimatedBudgetForExternalParticipations.Update(entity);
                    await _unitOfWork.CompleteAsync();
                    return true;
                }
            }

            return false;
        }

        public async Task<IEnumerable<EstimatedBudgetForExternalParticipation>> GetAllAsync()
        {
            return await _unitOfWork.EstimatedBudgetForExternalParticipations.GetAllAsync();
        }

        public async Task<IEnumerable<EmployeesNameDTO>> GetAllEmplyeeNames()
        {
            return await _unitOfWork.Employees.Table.Select(x => new EmployeesNameDTO
            {
                Id = x.Id,
                FullNameAr = x.FullNameAr,
                FullNameEn = x.FullNameEn,
                //JobId = x.JobId,
            }
            ).ToListAsync();
        }

        public async Task UpdateNewDetailsParticipationsType(IEnumerable<EstimatedBudgetForExternalParticipationDetail> allDetails_Records, int? modelId)
        {
            var allprevious_Records = await _unitOfWork.EstimatedBudgetForExternalParticipationDetails.GetAllAsync(x => (x.EstimatedBudgetForExternalParticipationId != null) && x.EstimatedBudgetForExternalParticipationId == modelId);
            _unitOfWork.EstimatedBudgetForExternalParticipationDetails.RemoveRange(allprevious_Records);
            // Only add new details (Id <= 0)
            foreach (var singleRecord in allDetails_Records)
            {
                if (singleRecord.Id > 0)
                {
                    // Skip existing records, do not try to insert
                    continue;
                }
                singleRecord.EstimatedBudgetForExternalParticipationId = modelId;
                singleRecord.Id = 0;
                await _unitOfWork.EstimatedBudgetForExternalParticipationDetails.AddAsync(singleRecord);
                await _unitOfWork.CompleteAsync();
            }
        }

        public async Task<EstimatedBudgetForExternalParticipation?> GetByIdAsync(int id)
        {
            return await _unitOfWork.EstimatedBudgetForExternalParticipations
                .GetByIdAsync(e => e.Id == id, c => c.EstimatedBudgetForExternalParticipationDetails, x => x.Signature);
        }

        public async Task<int> AddAsync(EstimatedBudgetForExternalParticipation entity)
        {

            await _unitOfWork.EstimatedBudgetForExternalParticipations.AddAsync(entity);
            await _unitOfWork.CompleteAsync();
            return entity.Id;
        }

        public async Task UpdateAsync(EstimatedBudgetForExternalParticipation entity)
        {
            var existing = await _unitOfWork.EstimatedBudgetForExternalParticipations.GetByIdAsync(entity.Id);
            if (existing == null) return;

            _unitOfWork.EstimatedBudgetForExternalParticipations.UpdateValues(existing, entity);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _unitOfWork.EstimatedBudgetForExternalParticipations.GetByIdAsync(id);
            var Details = await _unitOfWork.EstimatedBudgetForExternalParticipationDetails.GetAllAsync(x => x.EstimatedBudgetForExternalParticipationId == id);
            if (entity != null)
            {
                _unitOfWork.EstimatedBudgetForExternalParticipationDetails.RemoveRange(Details);
                _unitOfWork.EstimatedBudgetForExternalParticipations.Delete(entity);
                await _unitOfWork.CompleteAsync();
            }

        }
    }

}
