using Domain.Entities.Waste;

namespace Application.Interfaces.Admin
{
    public interface ISubWasteService
    {
        Task<IEnumerable<SubWaste>> GetAllAsync(string? search = null, int? mainWasteId = null, bool? isAdd = null);
        Task<SubWaste?> GetByIdAsync(int id);
        Task<int> AddAsync(SubWaste entity);
        Task UpdateAsync(SubWaste entity);
        Task DeleteAsync(int id);
        Task<bool> HasRelatedObjectsInDb(int id);
    }
}
