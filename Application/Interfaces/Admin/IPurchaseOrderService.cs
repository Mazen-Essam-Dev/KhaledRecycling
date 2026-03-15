using Azure.Core;
using Domain.DTOs.Admin.Car;
using Domain.DTOs.Admin.PurchaseOrder;
using Domain.Entities;
using Domain.Entities.PurchaseOrder;

namespace Application.Interfaces.Admin
{
    public interface IPurchaseOrderService
    {
        Task<IEnumerable<PurchaseOrder>> GetAllAsync();
        Task<IEnumerable<Supplier>> GetAllSuppliersAsync();
        Task<string> GetNewCodeAsync();
        Task<PurchaseOrder?> GetByIdAsync(long id);
        Task<IEnumerable<int>> GetAllYearsInDb();
        Task<long> AddAsync(PurchaseOrder entity);
        Task UpdateAsync(PurchaseOrder entity);
        Task DeleteAsync(long id);
        Task<bool> CheckIsMonthRegistedBefore(long id,DateOnly? dateOnly);
        Task<bool> SendOtpAsync();
        Task<bool> ValidateOtpAsync(long id , string code);

        Task UploadAttachmentsAsync(PurchaseOrderAttachmentsDTO model);
        Task<PurchaseOrderAttachmentsDTO> GetAttachmentsAsync(long PurchaseOrderId);
    }

}
