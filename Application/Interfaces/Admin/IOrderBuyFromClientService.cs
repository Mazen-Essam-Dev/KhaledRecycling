using Domain.Entities.Waste;

namespace Application.Interfaces.Admin
{
    public interface IOrderBuyFromClientService
    {
        Task<IEnumerable<OrderBuyFromClient>> GetAllAsync(string? search = null, int? mainWasteId = null, int? subWasteId = null);
        Task<OrderBuyFromClient?> GetByIdAsync(int id);
        Task<int> AddAsync(OrderBuyFromClient entity);
        Task UpdateAsync(OrderBuyFromClient entity);
        Task DeleteAsync(int id);
        Task<bool> HasRelatedObjectsInDb(int id);
    }
}
