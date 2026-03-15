using Domain.DTOs.Admin.ExpenseAndReceipt;
using Domain.Entities.ExpenseAndReceipt;

namespace Application.Interfaces.Admin.ExpenseAndReceipt
{
    public interface IReceiptService
    {
        Task<string> GetLastSerialCode();
        Task<IEnumerable<ExpenseAndReceiptAndOther>> GetAllAsync();
        Task<ExpenseAndReceiptAndOther?> GetByIdAsync(int id);
        Task<int> AddAsync(ExpenseAndReceiptAndOther entity);
        Task UpdateAsync(ExpenseAndReceiptAndOther entity);
        Task DeleteAsync(int id);

    }
}