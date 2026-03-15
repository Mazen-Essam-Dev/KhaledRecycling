using Domain.Entities.CashExchangeBond;
using Domain.DTOs.Admin.CashExchangeBond;
using Domain.Enums;

namespace Application.Interfaces.Admin
{
    public interface ICashExchangeBondService
    {
        Task<IEnumerable<CashExchangeBond>> GetAllAsync();
        Task<CashExchangeBond?> GetByIdAsync(int id);
        Task<IEnumerable<int>> GetAllYearsInDb();
        Task<int> AddAsync(CashExchangeBond entity);
        Task UpdateAsync(CashExchangeBond entity);
        Task DeleteAsync(int id);
        Task<bool> CheckIsMonthRegistedBefore(int id,DateOnly? dateOnly);
        Task<bool> SendOtpAsync(int id, string role);
        Task<(bool success, string? message)> ValidateOtpAsync(int id, string code, string role, System.Security.Claims.ClaimsPrincipal user);
        Task UploadAttachmentsAsync(CashExchangeBondAttachmentsDTO model);
        Task<CashExchangeBondAttachmentsDTO> GetAttachmentsAsync(int id);
    }

}
