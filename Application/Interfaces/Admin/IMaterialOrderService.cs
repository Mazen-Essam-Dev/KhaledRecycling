using Azure.Core;
using Domain.Entities;
using Domain.Entities.MaterialOrder;

namespace Application.Interfaces.Admin
{
    public interface IMaterialOrderService
    {
        Task<IEnumerable<MaterialOrder>> GetAllAsync();
        Task<string> GetNewCodeAsync();
        Task<MaterialOrder?> GetByIdAsync(int id);
        Task<IEnumerable<int>> GetAllYearsInDb();
        Task<int> AddAsync(MaterialOrder entity);
        Task UpdateAsync(MaterialOrder entity);
        Task DeleteAsync(int id);
        Task<bool> CheckIsMonthRegistedBefore(int id,DateOnly? dateOnly);
        //Task<bool> SendOtpAsync();
        //Task<(bool success, string? message)> ValidateOtpAsync(int id, string code, string role);

    }

}
