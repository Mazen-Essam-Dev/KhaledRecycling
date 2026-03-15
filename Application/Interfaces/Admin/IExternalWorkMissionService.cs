using Domain.DTOs.Admin;
using Domain.DTOs.Admin.SalaryManagement;
using Domain.Entities;
using Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces.Admin
{
    public interface IExternalWorkMissionService
    {
        Task<IEnumerable<ExternalWorkMission>> GetAllAsync();
        Task<IEnumerable<EmployeesNameDTO>> GetAllEmplyeeNames();
        Task UpdateNewMissionsType(List<int>? missionsType, int? modelId);
        Task<ExternalWorkMission?> GetByIdAsync(int id);
        Task<int> AddAsync(ExternalWorkMission entity);
        Task UpdateAsync(ExternalWorkMission entity);
        Task DeleteAsync(int id);
        Task<bool> SendOtpAsync();
        Task<bool> ValidateOtpAsync(int id, string code);
    }

}
