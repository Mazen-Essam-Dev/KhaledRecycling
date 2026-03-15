using Azure.Core;
using Domain.DTOs.Admin.Car;
using Domain.Entities;
using Domain.Entities.quote;

namespace Application.Interfaces.Admin
{
    public interface IquoteService
    {
        Task<IEnumerable<quote>> GetAllAsync();
        Task<IEnumerable<quote>> GetAllAsync(bool hasAddOrEdit);
        Task<IEnumerable<Supplier>> GetAllSuppliersAsync();
        Task<string> GetNewCodeAsync();
        Task<quote?> GetByIdAsync(int id);
        Task<IEnumerable<int>> GetAllYearsInDb();
        Task<int> AddAsync(quote entity);
        Task UpdateAsync(quote entity);
        Task DeleteAsync(int id);
        Task<bool> CheckIsMonthRegistedBefore(int id,DateOnly? dateOnly);
        Task<bool> SendOtpAsync();
        Task<(bool success, string? message)> ValidateOtpAsync(int id, string code, string role);

    }

}
