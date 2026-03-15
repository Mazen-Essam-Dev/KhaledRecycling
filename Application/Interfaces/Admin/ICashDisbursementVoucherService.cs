using Domain.DTOs.Admin.CashDisbursementVoucher;
using Domain.Entities.CashDisbursementVoucher;
using System.Security.Claims;

namespace Application.Interfaces.Admin
{
    public interface ICashDisbursementVoucherService
    {
        Task<int> GetLastExchangeProofDocumentNo();
        Task<IEnumerable<CashDisbursementVoucher>> GetAllAsync();
        Task<CashDisbursementVoucher?> GetByIdAsync(int id);
        Task<ExchangeProof?> GetExchangeProofByIdAsyncSingle(int id);
        Task<int> AddAsync(CashDisbursementVoucher entity);
        Task UpdateAsync(CashDisbursementVoucher entity);
        Task DeleteAsync(int id);

        Task<(int?,int?)> GetExchangeProofSignaturesAsync(int cashDisbursementVoucherId);
        Task<ExchangeProof?> GetExchangeProofByIdAsync(int cashDisbursementVoucherId);
        Task AddAsync(ExchangeProof entity);
        Task UpdateAsync(ExchangeProof entity);

        Task<bool> GetAcknowledgmentReceiptSignaturesAsync(int cashDisbursementVoucherId);
        Task<AcknowledgmentReceipt?> GetAcknowledgmentReceiptByIdAsync(int cashDisbursementVoucherId);
        Task AddAsync(AcknowledgmentReceipt entity);
        Task UpdateAsync(AcknowledgmentReceipt entity);

        Task<bool> SendOtpAsync(int id, string role);
        Task<(bool success, string? message)> DisbursementRequestValidateOtpAsync(int id, string code, string role, ClaimsPrincipal user);
        Task<(bool success, string? message)> ExchangeProofValidateOtpAsync(int id, string code, string role, ClaimsPrincipal user);
        Task<(bool success, string? message)> AcknowledgmentReceiptValidateOtpAsync(int id, string code, string role, ClaimsPrincipal user);

        // Attachment methods
        Task UploadAttachmentsAsync(CashDisbursementVoucherAttachmentsDTO model);
        Task<IEnumerable<CashDisbursementVoucherAttachment>> GetAttachmentsAsync(int cashDisbursementVoucherId);
    }
}
