using Domain.Entities.BudgetItem;

namespace Application.Interfaces.Admin
{
    public interface IBudgetItemService
    {
        Task<IEnumerable<BudgetItem>> GetAllAsync();
        Task<BudgetItem?> GetByIdAsync(int id);
        Task<int> AddAsync(BudgetItem entity);
        Task UpdateAsync(BudgetItem entity);
        Task<bool> DeleteAsync(int id);
        Task<int> GenerateNewCode();
    }

}
