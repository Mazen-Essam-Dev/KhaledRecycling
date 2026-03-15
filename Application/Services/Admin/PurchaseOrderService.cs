using Application.Helpers;
using Application.Interfaces.Admin;
using Domain.DTOs.Admin.Car;
using Domain.DTOs.Admin.PurchaseOrder;
using Domain.Entities;
using Domain.Entities.PurchaseOrder;
using Infrastructure.Repositories.InterfacesDB;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Admin
{
    public class PurchaseOrderService : IPurchaseOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _env;
        private readonly string FileName = "PurchaseOrders";
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISMSService _SMSService;
        private readonly ISMSForSendingOTPService _SMSForSendingOTPService;


        public PurchaseOrderService(IUnitOfWork unitOfWork, IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor, ISMSService sMSService, ISMSForSendingOTPService sMSForSendingOTPService)
        {
            _unitOfWork = unitOfWork;
            _env = env;
            _httpContextAccessor = httpContextAccessor;
            _SMSService = sMSService;
            _SMSForSendingOTPService = sMSForSendingOTPService;
        }

        public async Task<IEnumerable<PurchaseOrder>> GetAllAsync()
        {
            return await _unitOfWork.PurchaseOrders.GetAllAsync(x => x.Signature);
        }

        public async Task<IEnumerable<Supplier>> GetAllSuppliersAsync()
        {
            return await _unitOfWork.Suppliers.Table.Select(x => new Supplier
            {
                SupplierNameAr = x.SupplierNameAr,
                SupplierNameEn = x.SupplierNameEn,
                Id = x.Id
            }
            ).ToListAsync();
        }

        public async Task<string> GetNewCodeAsync()
        {
            var lastId = await _unitOfWork.PurchaseOrders.Table.Select(x => (long?)x.Id).MaxAsync() ?? 0;

            var getLastCode = await _unitOfWork.PurchaseOrders.Table.Where(x => x.Id == lastId).Select(x => x.PurchaseOrderCode).FirstOrDefaultAsync();

            if (!long.TryParse(getLastCode, out var numericCode))
                numericCode = 187; // start 188

            return (numericCode + 1).ToString("D5");
        }

        public async Task<PurchaseOrder?> GetByIdAsync(long id)
        {
            return await _unitOfWork.PurchaseOrders
                .GetByIdAsync(e => e.Id == id, e => e.Items, e => e.Signature!);
        }

        public async Task<IEnumerable<int>> GetAllYearsInDb()
        {
            var allRecords = await _unitOfWork.PurchaseOrders.GetAllAsync();
            var allYears = allRecords
                .Where(e => e.Date.HasValue)
                .Select(e => e.Date.Value.Year)
                .Distinct()
                .OrderBy(y => y)
                .ToList();

            return allYears;
        }

        public async Task<long> AddAsync(PurchaseOrder entity)
        {
            int n = 0;
            decimal totalAll = 0.00m;
            decimal VATTotal = 0.00m;
            var VATValue = entity.VATValue.Value;
            var hasVAT = entity.HasVAT;
            // 🔹 Add new items
            foreach (var item in entity.Items)
            {
                // sum Total
                if (item.SinglePrice == null) item.SinglePrice = 0.00m;
                if (item.Quantity == null) item.Quantity = 0;
                var totalRecord = item.SinglePrice.Value * item.Quantity.Value;
                totalAll = totalAll + totalRecord;
                //------ End Summition

                n++;
                // Set the ID manually
                item.PurchaseOrderItemId = n;
                item.PurchaseOrderId = entity.Id;
            }
            if (hasVAT)
                VATTotal = VATValue * totalAll;

            entity.OrderTotal = totalAll;
            entity.OrderTotalWithVAT = totalAll + VATTotal;

            var codeExists = _unitOfWork.PurchaseOrders.Table.Any(x => x.PurchaseOrderCode == entity.PurchaseOrderCode);
            if (codeExists)
            {
                entity.PurchaseOrderCode = await GetNewCodeAsync();
            }
            var entit = await _unitOfWork.PurchaseOrders.AddAsync(entity);
            await _unitOfWork.CompleteAsync();
            return entit.Id;
        }

        public async Task UpdateAsync(PurchaseOrder entity)
        {
            var existing = await _unitOfWork.PurchaseOrders.GetByIdAsync(
                e => e.Id == entity.Id,
                e => e.Items
            );

            if (existing == null)
                return;


            // 🔹 Delete all old items
            _unitOfWork.PurchaseOrderItems.RemoveRange(existing.Items);
            int n = 0;
            decimal totalAll = 0.00m;
            decimal VATTotal = 0.00m;
            var VATValue = entity.VATValue.Value;
            var hasVAT = entity.HasVAT;
            // 🔹 Add new items
            foreach (var item in entity.Items)
            {
                // sum Total
                if (item.SinglePrice == null) item.SinglePrice = 0.00m;
                if (item.Quantity == null) item.Quantity = 0;
                var totalRecord = item.SinglePrice.Value * item.Quantity.Value;
                totalAll = totalAll + totalRecord;
                //------ End Summition

                n++;
                // Set the ID manually
                item.PurchaseOrderItemId = n;
                item.PurchaseOrderId = existing.Id;
                await _unitOfWork.PurchaseOrderItems.AddAsync(item);
            }
            if (hasVAT)
                VATTotal = VATValue * totalAll;

            entity.OrderTotal = totalAll;
            entity.OrderTotalWithVAT = totalAll + VATTotal;

            // 🔹 Update global values (without items)
            _unitOfWork.PurchaseOrders.UpdateValues(existing, entity);

            //🔹 Save changes
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteAsync(long id)
        {
            var entity = await _unitOfWork.PurchaseOrders.GetByIdAsync(id);
            if (entity != null)
            {
                _unitOfWork.PurchaseOrderItems.RemoveRange(entity.Items);
                _unitOfWork.PurchaseOrders.Delete(entity);
                await _unitOfWork.CompleteAsync();
            }
        }

        public async Task<bool> CheckIsMonthRegistedBefore(long id, DateOnly? dateOnly)
        {
            if (dateOnly == null) { return true; }
            var allPreviousWithMonth = await _unitOfWork.PurchaseOrders.GetAllAsync(x => x.Date != null && x.Date.Value.Year == dateOnly.Value.Year && x.Date.Value.Month == dateOnly.Value.Month && x.Id != id);
            if (allPreviousWithMonth != null && allPreviousWithMonth.Count() > 0) { return true; }
            return false;
        }

        public async Task<bool> SendOtpAsync()
        {
            var (status, code) = await OTPHelper.SaveOtpAsync(_httpContextAccessor, _unitOfWork);

            if (status == false) return false;

            var resultStatus = await _SMSForSendingOTPService.SendOtpSMSAsync(code);

            return resultStatus.Item1;
        }
        public async Task<bool> ValidateOtpAsync(long id, string code)
        {
            var success = await OTPHelper.ValidateOtpAsync(_httpContextAccessor, _unitOfWork, code);

            if (success)
            {
                var purchaseOrder = await GetByIdAsync(id);
                var updatedPurchaseOrder = purchaseOrder;

                // Get all signatures for the user and pick the latest
                var userId = _httpContextAccessor.HttpContext.User.GetUserId();
                var allSignatures = await _unitOfWork.Signatures.GetAllAsync();
                var latestSignature = allSignatures
                    .Where(s => s.UserId == userId)
                    .OrderByDescending(s => s.CreatedAt)
                    .FirstOrDefault();
                if (latestSignature != null && updatedPurchaseOrder != null)
                    updatedPurchaseOrder.SignatureId = latestSignature.Id;

                _unitOfWork.PurchaseOrders?.UpdateValues(purchaseOrder, updatedPurchaseOrder);
                await _unitOfWork.CompleteAsync();
                return true;
            }

            return false;
        }

        public async Task UploadAttachmentsAsync(PurchaseOrderAttachmentsDTO model)
        {
            // Get existing attachments
            var existingAttachments = await _unitOfWork.PurchaseOrderAttachments.GetAllAsync(e => e.PurchaseOrderId == model.PurchaseOrderId);

            // Delete removed attachments
            foreach (var existingAttachment in existingAttachments)
            {
                var stillExists = model.Attachments.Any(a => a.Id == existingAttachment.Id);
                if (!stillExists)
                {
                    FileHelper.DeleteImageFile(existingAttachment.Path);
                    _unitOfWork.PurchaseOrderAttachments.Delete(existingAttachment);
                }
            }

            // Add or update attachments
            foreach (var attachmentDto in model.Attachments)
            {
                if (attachmentDto.File != null)
                {
                    var path = await FileHelper.SaveImageAsync(attachmentDto.File, "PurchaseOrderAttachments");

                    var attachment = new PurchaseOrderAttachment
                    {
                        Name = attachmentDto.Name,
                        Path = path,
                        PurchaseOrderId = model.PurchaseOrderId
                    };

                    await _unitOfWork.PurchaseOrderAttachments.AddAsync(attachment);
                }
            }

            await _unitOfWork.CompleteAsync();

        }

        public async Task<PurchaseOrderAttachmentsDTO> GetAttachmentsAsync(long PurchaseOrderId)
        {
            var attachments = await _unitOfWork.PurchaseOrderAttachments.GetAllAsync(e => e.PurchaseOrderId == PurchaseOrderId);
            return new PurchaseOrderAttachmentsDTO
            {
                PurchaseOrderId = PurchaseOrderId,
                Attachments = attachments.Select(a => new PurchaseOrderAttachmentDTO
                {
                    Id = a.Id,
                    Name = a.Name,
                    Path = a.Path
                }).ToList()
            };
        }

    }
}
