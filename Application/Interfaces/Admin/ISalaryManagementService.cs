using Application.Helpers;
using Domain.DTOs.Admin;
using Domain.DTOs.Admin.SalaryManagement;
using Domain.DTOs.Admin.SalaryManagement;
using Domain.Entities.SalaryManage;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces.Admin
{
    public interface ISalaryManagementService
    {
        Task<bool> SendOtpAsync();
        Task<(bool success, string? message)> ValidateOtp_OpenDetails_payrollReportAsync(int year, int month, string code, string role, System.Security.Claims.ClaimsPrincipal user);
        Task<(bool success, string? message)> ValidateOtp_OpenDetails_DiscountsAndBonusesReportAsync(int year, int month, string code, string role, System.Security.Claims.ClaimsPrincipal user);

        Task<IEnumerable<SalaryManagement>> GetAllAsync();
        Task<SalaryManagement?> GetByIdAsync(int id);
        Task<IEnumerable<EmployeesNameDTO>> GetAllEmplyeeNames();

        Task<int> AddAsync(SalaryManagement entity);
        Task UpdateAsync(SalaryManagement entity);
        Task DeleteAsync(int id);
        Task<string> SaveImageAsync(IFormFile file);
        void DeleteImageFile(string? relativePath);
        Task<IEnumerable<AbsenceDTO>> GetAllAbsencesAsync();
        Task<IEnumerable<int>> GetAllYearsInDb();

        Task UploadAttachmentsAsync(SalaryManagementAttachmentsDTO model);

        Task<SalaryManagementAttachmentsDTO> GetAttachmentsAsync(int SalaryManagementId);

    }

}
