using Domain.Entities.Product;

namespace Application.Interfaces.Admin
{
    public interface ISubProductService
    {
        Task<IEnumerable<SubProduct>> GetAllAsync(string? search = null, int? mainProductId = null, bool? isAdd = null);
        Task<SubProduct?> GetByIdAsync(int id);
        Task<int> AddAsync(SubProduct entity);
        Task UpdateAsync(SubProduct entity);
        Task DeleteAsync(int id);
        Task<bool> HasRelatedObjectsInDb(int id);
    }
}
