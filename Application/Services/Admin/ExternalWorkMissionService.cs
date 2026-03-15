using Application.Helpers;
using Application.Interfaces.Admin;
using DocumentFormat.OpenXml.Wordprocessing;
using Domain.DTOs.Admin;
using Domain.DTOs.Admin.SalaryManagement;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Repositories.InterfacesDB;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Admin
{
    public class ExternalWorkMissionService : IExternalWorkMissionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISMSService _SMSService;
        private readonly ISMSForSendingOTPService _SMSForSendingOTPService;
        private readonly string FileName = "ExternalWorkMissions";

        public ExternalWorkMissionService(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, ISMSService sMSService, ISMSForSendingOTPService sMSForSendingOTPService)
        {
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _SMSService = sMSService;
            _SMSForSendingOTPService = sMSForSendingOTPService;
        }

        public async Task<IEnumerable<ExternalWorkMission>> GetAllAsync()
        {
            return await _unitOfWork.ExternalWorkMissions.GetAllAsync(c => c.Missions);
        }

        public async Task<IEnumerable<EmployeesNameDTO>> GetAllEmplyeeNames()
        {
            return await _unitOfWork.Employees.Table.Select(x => new EmployeesNameDTO
            {
                Id = x.Id,
                FullNameAr = x.FullNameAr,
                FullNameEn = x.FullNameEn,
                JobTitle = x.JobTitle,
                //JobId = x.JobId,
                //Job = x.Job,
            }
            ).ToListAsync();
        }

        public async Task UpdateNewMissionsType(List<int>? MissionTypes, int? modelId)
        {
            var allpreviousMissions = await _unitOfWork.Missions.GetAllAsync(x => (x.ExternalWorkMissionId != null) && x.ExternalWorkMissionId == modelId);
            _unitOfWork.Missions.RemoveRange(allpreviousMissions);
            foreach (var missType in MissionTypes)
            {
                var missionType = new Mission
                {
                    ExternalWorkMissionId = modelId,
                    MessionKey = missType
                };
                await _unitOfWork.Missions.AddAsync(missionType);
                await _unitOfWork.CompleteAsync();
            }
        }

        public async Task<ExternalWorkMission?> GetByIdAsync(int id)
        {
            return await _unitOfWork.ExternalWorkMissions
                .GetByIdAsync(e => e.Id == id, c => c.Missions, x => x.Employee, x => x.Signature);
        }

        public async Task<int> AddAsync(ExternalWorkMission entity)
        {

            await _unitOfWork.ExternalWorkMissions.AddAsync(entity);
            await _unitOfWork.CompleteAsync();
            return entity.Id;
        }

        public async Task UpdateAsync(ExternalWorkMission entity)
        {
            var existing = await _unitOfWork.ExternalWorkMissions.GetByIdAsync(entity.Id);
            if (existing == null) return;

            _unitOfWork.ExternalWorkMissions.UpdateValues(existing, entity);
            await _unitOfWork.CompleteAsync();
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
                var mission = await GetByIdAsync(id);
                var updatedMission = mission;
                var userId = _httpContextAccessor.HttpContext?.User.GetUserId();
                var allSignatures = await _unitOfWork.Signatures.GetAllAsync();
                var latestSignature = allSignatures
                    .Where(s => s.UserId == userId)
                    .OrderByDescending(s => s.CreatedAt)
                    .FirstOrDefault();
                if (latestSignature != null && updatedMission != null)
                    updatedMission.SignatureIdApproved = latestSignature.Id;

                _unitOfWork.ExternalWorkMissions?.UpdateValues(mission, updatedMission);
                await _unitOfWork.CompleteAsync();
                return true;
            }

            return false;
        }

        public async Task DeleteAsync(int id)

        {
            var entity = await _unitOfWork.ExternalWorkMissions.GetByIdAsync(x => x.Id == id);
            var Missions = await _unitOfWork.Missions.GetAllAsync(x => x.ExternalWorkMissionId == id);
            if (entity != null)
            {
                _unitOfWork.Missions.RemoveRange(Missions);
                _unitOfWork.ExternalWorkMissions.Delete(entity);
                await _unitOfWork.CompleteAsync();
            }
        }
    }

}
