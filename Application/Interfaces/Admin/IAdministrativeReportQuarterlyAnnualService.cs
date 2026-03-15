using Domain.DTOs.Admin.AdministrativeReportQuarterlyAnnual;
using Domain.DTOs.Admin.QuartersReport;
using Domain.Entities.MonthlyAdministrativeReport;
using Domain.Enums;
using System.Security.Claims;

namespace Application.Interfaces.Admin
{
    public interface IAdministrativeReportQuarterlyAnnualService
    {
        Task<IEnumerable<MonthlyAdministrativeReport>> GetAllAsync();
        Task<IEnumerable<MonthsOfYearsAnnualyDTO>> GetAllMonthsofYearsAnnualy(int? year);
        Task<IEnumerable<MonthsOfYearsAnnualyWithDetailsDTO>> GetAllMonthsofYearsAnnualy_Data(int? year);
        Task<MonthlyAdministrativeReport?> GetByIdAsync(int id);
        Task<IEnumerable<int>> GetAllYearsInDb();
        Task<bool> CheckCollaborativeReportIsSiggned(int? year);
        Task<MonthsOfYearsAnnualyActivitiesAndAdministrativeDTO> GetAdministrativeAndActivitiesDetailsAsync(int selectedYear, int quarter, string lang);
        Task<bool> SendOtpAsync(int id, string role);
        Task<(bool success, string? message)> ValidateOtpAsync(int type, int quarter, int year, string code, string role, ClaimsPrincipal user);

        Task<QuartersReportDTO?> GetQuarterSignature(int? selectedYear, QuartersYear? quarter, QuarterlyReportType? ActionType);
    }

}
