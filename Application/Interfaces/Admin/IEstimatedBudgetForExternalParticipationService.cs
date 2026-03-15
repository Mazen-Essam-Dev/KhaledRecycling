using Domain.DTOs.Admin.SalaryManagement;
using Domain.Entities.EstimatedBudgetForExternalParticipation;
using Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces.Admin
{
    public interface IEstimatedBudgetForExternalParticipationService
    {
        Task<IEnumerable<EstimatedBudgetForExternalParticipation>> GetAllAsync();
        Task<IEnumerable<EmployeesNameDTO>> GetAllEmplyeeNames();
        Task UpdateNewDetailsParticipationsType(IEnumerable<EstimatedBudgetForExternalParticipationDetail> allDetails_Records, int? modelId);
        Task<EstimatedBudgetForExternalParticipation?> GetByIdAsync(int id);
        Task<int> AddAsync(EstimatedBudgetForExternalParticipation entity);
        Task UpdateAsync(EstimatedBudgetForExternalParticipation entity);
        Task DeleteAsync(int id);
        Task<bool> SendOtpAsync();
        Task<bool> ValidateOtpAsync(int id, string code);
    }

}
