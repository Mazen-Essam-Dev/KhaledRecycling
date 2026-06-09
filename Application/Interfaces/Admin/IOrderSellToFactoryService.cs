using Domain.Entities.Waste;

namespace Application.Interfaces.Admin
{
    public interface IOrderSellToFactoryService
    {
        Task<IEnumerable<OrderSellToFactory>> GetAllAsync(string? search = null, int? mainWasteId = null, int? subWasteId = null);
        Task<OrderSellToFactory?> GetByIdAsync(int id);
        Task<int> AddAsync(OrderSellToFactory entity);
        Task UpdateAsync(OrderSellToFactory entity);
        Task DeleteAsync(int id);
        Task<bool> HasRelatedObjectsInDb(int id);
    }
}
