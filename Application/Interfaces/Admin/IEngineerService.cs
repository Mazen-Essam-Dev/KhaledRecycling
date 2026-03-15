using Domain.Entities;
using Domain.Entities.MonthlyAdministrativeReport;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces.Admin
{
    public interface IEngineerService
    {
        Task<IEnumerable<Engineer>> GetAllAsync();
        Task<List<int>> GetAllGraduationYears();
        Task<List<int?>?> GetAllGraduationYears_index();
        Task<Engineer?> GetByIdAsync(int id);
        Task<int> AddAsync(Engineer entity, IFormFile? file);
        Task UpdateAsync(Engineer entity, IFormFile? file);
        Task DeleteAsync(int id);
        Task<string> SaveImageAsync(IFormFile file);
        void DeleteImageFile(string? relativePath);
        Task<int> GenerateNewCode();

    }

}
