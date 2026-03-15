using Domain.Entities.MonthlyAdministrativeReport;
using Domain.Enums;

namespace Application.Interfaces.Admin
{
    public interface IMonthlyAdministrativeReportService
    {
        Task<IEnumerable<MonthlyAdministrativeReport>> GetAllAsync();
        Task<MonthlyAdministrativeReport?> GetByIdAsync(int id);
        Task<IEnumerable<int>> GetAllYearsInDb();
        Task<int> AddAsync(MonthlyAdministrativeReport entity);
        Task UpdateAsync(MonthlyAdministrativeReport entity);
        Task DeleteAsync(int id);
        Task<bool> CheckIsMonthRegistedBefore(int id, DateOnly? dateOnly, MonthlyAdministrativeReportType type);
        Task<bool> SendOtpAsync(int id, string role);
        Task<(bool success, string? message)> ValidateOtpAsync(int id, string code, string role, System.Security.Claims.ClaimsPrincipal user);
    }

}
