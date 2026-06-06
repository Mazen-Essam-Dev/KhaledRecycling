using Domain.Entities.Inventory;

namespace Application.Interfaces.Admin
{
    public interface IRoomInventoryService
    {
        Task<IEnumerable<RoomInventory>> GetAllAsync(string? search = null, int? inventoryId = null, int? subWasteId = null);
        Task<RoomInventory?> GetByIdAsync(int id);
        Task<int> AddAsync(RoomInventory entity);
        Task UpdateAsync(RoomInventory entity);
        Task DeleteAsync(int id);
        Task<bool> HasRelatedObjectsInDb(int id);
    }
}
