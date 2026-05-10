using Domain.Entities.Waste;

namespace Application.Interfaces.Admin
{
    public interface IMainWasteService
    {
        Task<IEnumerable<MainWaste>> GetAllAsync(string? search = null);
        Task<MainWaste?> GetByIdAsync(int id);
        Task<int> AddAsync(MainWaste entity);
        Task UpdateAsync(MainWaste entity);
        Task DeleteAsync(int id);
        Task<bool> HasRelatedObjectsInDb(int id);
    }
}
