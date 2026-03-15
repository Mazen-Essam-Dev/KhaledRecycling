using Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces.Admin
{
    public interface IAnnualScheduleService
    {
        Task<IEnumerable<AnnualSchedule>> GetAllAsync();
        Task<AnnualSchedule?> GetByIdAsync(int id);
        Task<int> AddAsync(AnnualSchedule entity);
        Task UpdateAsync(AnnualSchedule entity);
        Task DeleteAsync(int id);
        Task<string> SaveImageAsync(IFormFile file);
        void DeleteImageFile(string? relativePath);

    }

}
