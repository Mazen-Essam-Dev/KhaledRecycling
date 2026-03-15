using Domain.DTOs.Admin.ExpenseAndReceipt;
using Domain.Entities.ExpenseAndReceipt;

namespace Application.Interfaces.Admin.ExpenseAndReceipt
{
    public interface IExpenseService
    {
        Task<string> GetLastSerialCode();

        Task<bool> SendOtpAsync();
        Task<(bool success, string? message)> ValidateOtp_OpenDetails_ExpensesReportAsync(int year, int month, string code, string role, System.Security.Claims.ClaimsPrincipal user);
        Task<(bool success, string? message)> ValidateOtp_OpenDetails_ExpensesAndReciptReportAsync(int year, int month, string code, string role, System.Security.Claims.ClaimsPrincipal user);

        Task<IEnumerable<ExpenseAndReceiptAndOther>> GetAllAsync();
        Task<ExpensesReportDTO> GetAllExpensesReportAsync(string lang, int year, int month);
        //Task<ExpensesReportDTO> GetThisExpensesReportSignAsync(string lang, int year, int month);
        Task<ExpensesAndReceiptsReportDTO> GetAllExpenseAndReceiptReportAsync(string lang, int year, int month);
        Task<ExpenseAndReceiptAndOther?> GetByIdAsync(int id);
        Task<int> AddAsync(ExpenseAndReceiptAndOther entity);
        Task UpdateAsync(ExpenseAndReceiptAndOther entity);

        Task<ReceivingReceipt?> GetReceivingReceiptByIdAsync(int expenseId);
        Task<int> AddAsync(ReceivingReceipt entity);
        Task UpdateAsync(ReceivingReceipt entity);
        Task<bool> ValidateOtpAsync(int id, string code);

        Task DeleteAsync(int id);
        Task<IEnumerable<int>> GetAllYearsInDb();
        Task<IEnumerable<int>> GetAllYearsOfExpenseAndReceiptReportInDb();
    }
}
