using Domain.Entities.Gallary;

namespace Application.Interfaces.Admin
{
    public interface IGalleryService
    {
        Task<IEnumerable<Gallery>> GetAllAsync(string? search = null);
        Task<Gallery?> GetByIdAsync(int id);
        Task<int> AddAsync(Gallery entity);
        Task UpdateAsync(Gallery entity);
        Task DeleteAsync(int id);
        Task<bool> HasRelatedObjectsInDb(int id);
    }
}
