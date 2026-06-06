using Domain.Entities.Inventory;

namespace Application.Interfaces.Admin
{
    public interface IInventoryService
    {
        Task<IEnumerable<Inventory>> GetAllAsync(string? search = null);
        Task<Inventory?> GetByIdAsync(int id);
        Task<int> AddAsync(Inventory entity);
        Task UpdateAsync(Inventory entity);
        Task DeleteAsync(int id);
        Task<bool> HasRelatedObjectsInDb(int id);
    }
}
