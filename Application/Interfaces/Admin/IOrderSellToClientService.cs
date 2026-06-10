using Domain.Entities.Product;

namespace Application.Interfaces.Admin
{
    public interface IOrderSellToClientService
    {
        Task<IEnumerable<OrderSellToClient>> GetAllAsync(string? search = null, int? mainProductId = null, int? subProductId = null);
        Task<OrderSellToClient?> GetByIdAsync(int id);
        Task<int> AddAsync(OrderSellToClient entity);
        Task UpdateAsync(OrderSellToClient entity);
        Task DeleteAsync(int id);
        Task<bool> HasRelatedObjectsInDb(int id);
    }
}
