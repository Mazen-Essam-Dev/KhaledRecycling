using Domain.Entities.Product;

namespace Application.Interfaces.Admin
{
    public interface IOrderBuyFromFactoryService
    {
        Task<IEnumerable<OrderBuyFromFactory>> GetAllAsync(string? search = null, int? mainProductId = null, int? subProductId = null);
        Task<OrderBuyFromFactory?> GetByIdAsync(int id);
        Task<int> AddAsync(OrderBuyFromFactory entity);
        Task UpdateAsync(OrderBuyFromFactory entity);
        Task DeleteAsync(int id);
        Task<bool> HasRelatedObjectsInDb(int id);
    }
}
