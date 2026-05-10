using Domain.Entities.Product;

namespace Application.Interfaces.Admin
{
    public interface IMainProductService
    {
        Task<IEnumerable<MainProduct>> GetAllAsync(string? search = null);
        Task<MainProduct?> GetByIdAsync(int id);
        Task<int> AddAsync(MainProduct entity);
        Task UpdateAsync(MainProduct entity);
        Task DeleteAsync(int id);
        Task<bool> HasRelatedObjectsInDb(int id);
    }
}
