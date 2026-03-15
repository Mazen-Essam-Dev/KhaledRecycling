using Application.Helpers;
using Application.Interfaces.Admin;
using DocumentFormat.OpenXml.InkML;
using Domain.DTOs.Admin.Car;
using Domain.Entities;
using Domain.Entities.quote;
using Infrastructure.Repositories.InterfacesDB;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Application.Services.Admin
{
    public class quoteService : IquoteService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _env;
        private readonly string FileName = "quote";
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISMSService _SMSService;
        private readonly ISMSForSendingOTPService _SMSForSendingOTPService;


        public quoteService(IUnitOfWork unitOfWork, IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor, ISMSService sMSService, ISMSForSendingOTPService sMSForSendingOTPService)
        {
            _unitOfWork = unitOfWork;
            _env = env;
            _httpContextAccessor = httpContextAccessor;
            _SMSService = sMSService;
            _SMSForSendingOTPService = sMSForSendingOTPService;
        }

        public async Task<IEnumerable<quote>> GetAllAsync()
        {
            return await _unitOfWork.quotes.GetAllAsync(e => e.SignatureAccountant!, e => e.SignatureUserSecetary!, x => x.SignatureSuperVisor!, x => x.SignatureManager!);
        }
        public async Task<IEnumerable<quote>> GetAllAsync(bool hasAddOrEdit)
        {
            if (hasAddOrEdit)
            { // get all Confirmed and Not
                return await _unitOfWork.quotes.GetAllAsync(e => e.SignatureAccountant!, e => e.SignatureUserSecetary!, x => x.SignatureSuperVisor!, x => x.SignatureManager!);
            }
            // get only Confirmed As Details readonly
            return await _unitOfWork.quotes.GetAllAsync(x=>x.quoteIsConfirmed==true , e => e.SignatureAccountant!, e => e.SignatureUserSecetary!, x => x.SignatureSuperVisor!, x => x.SignatureManager!);
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
            var lastId = await _unitOfWork.quotes.Table.Select(x => (int?)x.Id).MaxAsync() ?? 0;

            var getLastCode = await _unitOfWork.quotes.Table.Where(x => x.Id == lastId).Select(x => x.quoteCode).FirstOrDefaultAsync();

            if (!int.TryParse(getLastCode, out var numericCode))
                numericCode = 0; // start 1

            return (numericCode + 1).ToString("D4");
        }

        //public async Task<quote?> GetByIdAsync(int id)
        //{
        //    return await _unitOfWork.quotes
        //    .GetByIdAsync(
        // e => e.Id == id,
        // includes: new Expression<Func<quote, object>>[]
        // {
        //        e => e.Items,
        //        e => e.SignatureUser!,
        //        e => e.SignatureManager!,
        //        e => e.Department!
        // });
        //}
        public async Task<quote?> GetByIdAsync(int id)
        {
            return await _unitOfWork.quotes
                .GetByIdAsync(e => e.Id == id, e => e.quotesItems,e => e.SignatureAccountant!, e => e.SignatureUserSecetary!, x => x.SignatureSuperVisor!, x => x.SignatureManager!);
        }
        public async Task<IEnumerable<int>> GetAllYearsInDb()
        {
            var allRecords = await _unitOfWork.quotes.GetAllAsync();
            var allYears = allRecords
                .Where(e => e.Date.HasValue)
                .Select(e => e.Date.Value.Year)
                .Distinct()
                .OrderBy(y => y)
                .ToList();

            return allYears;
        }

        public async Task<int> AddAsync(quote entity)
        {
            int n = 0;
            decimal totalAll = 0.00m;

            // 🔹 Add new items
            foreach (var item in entity.quotesItems)
            {
                // sum Total
                if (item.Quantity == null) item.Quantity = 0;

                //------ End Summition

                n++;
                //// Set the ID manually
                item.quoteId = entity.Id;
            }


            entity.OrderTotal = totalAll;

            var codeExists = _unitOfWork.quotes.Table.Any(x => x.quoteCode == entity.quoteCode);
            if (codeExists)
            {
                entity.quoteCode = await GetNewCodeAsync();
            }
            var entit = await _unitOfWork.quotes.AddAsync(entity);
            await _unitOfWork.CompleteAsync();
            return entit.Id;
        }

        public async Task UpdateAsync(quote entity)
        {
            var existing = await _unitOfWork.quotes.GetByIdAsync(
                e => e.Id == entity.Id
            );

            if (existing == null)
                return;

            // 🔹 Delete all old items
            var oldItems = await _unitOfWork.quotesItems.GetAllAsync(x => x.quoteId == entity.Id);
            _unitOfWork.quotesItems.RemoveRange(oldItems);


            int n = 0;
            decimal totalAll = 0.00m;
            decimal VATTotal = 0.00m;

            // 🔹 Add new items
            foreach (var item in entity.quotesItems)
            {
                // sum Total
                //if (item.SinglePrice == null) item.SinglePrice = 0.00m;
                if (item.Quantity == null) item.Quantity = 0;
                //var totalRecord = item.SinglePrice.Value * item.Quantity.Value;
                //totalAll = totalAll + totalRecord;
                //------ End Summition

                n++;
                //// Set the ID manually
                // item.quoteItemId = n;
                item.quoteId = existing.Id;
                await _unitOfWork.quotesItems.AddAsync(item);
            }


            entity.OrderTotal = totalAll;

            // 🔹 Update global values (without items)
            _unitOfWork.quotes.UpdateValues(existing, entity);

            //🔹 Save changes
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _unitOfWork.quotes.GetByIdAsync(id);
            if (entity != null)
            {
                _unitOfWork.quotesItems.RemoveRange(entity.quotesItems);
                _unitOfWork.quotes.Delete(entity);
                await _unitOfWork.CompleteAsync();
            }
        }

        public async Task<bool> CheckIsMonthRegistedBefore(int id, DateOnly? dateOnly)
        {
            if (dateOnly == null) { return true; }
            var allPreviousWithMonth = await _unitOfWork.quotes.GetAllAsync(x => x.Date != null && x.Date.Value.Year == dateOnly.Value.Year && x.Date.Value.Month == dateOnly.Value.Month && x.Id != id);
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

    public async Task<(bool success, string? message)> ValidateOtpAsync(int id, string code, string role)
    {
        var success = await OTPHelper.ValidateOtpAsync(_httpContextAccessor, _unitOfWork, code);
        if (!success)
            return (false, "Invalid OTP");

        var quote = await GetByIdAsync(id);
        if (quote == null)
            return (false, "Record not found");

        // Get the latest signature of the current user
        var userId = _httpContextAccessor.HttpContext.User.GetUserId();
        var allSignatures = await _unitOfWork.Signatures.GetAllAsync();
        var latestSignature = allSignatures
            .Where(s => s.UserId == userId)
            .OrderByDescending(s => s.CreatedAt)
            .FirstOrDefault();

        if (latestSignature == null)
            return (false, "Signature not found");

        if (role == "Supervisor")
        {
            quote.SignatureSuperVisorId = latestSignature.Id;
            _unitOfWork.quotes.UpdateValues(quote, quote);
        }
        else if (role == "Accountant")
        {
            if (quote.SignatureSuperVisor == null || quote.SignatureSuperVisorId == null)
                return (false, "يجب أن يوقع مشرف الانشطة قبلك");
            quote.SignatureAccountantId = latestSignature.Id;
            _unitOfWork.quotes.UpdateValues(quote, quote);
        }
        else if (role == "Secetary")
        {
            if (quote.SignatureAccountant == null || quote.SignatureAccountantId == null)
                return (false, "يجب أن يوقع المحاسب قبلك");
            quote.SignatureUserSecetaryId = latestSignature.Id;
            _unitOfWork.quotes.UpdateValues(quote, quote);
        }
        else if (role == "Manager")
        {
            if (quote.SignatureUserSecetary == null || quote.SignatureUserSecetaryId == null)
                return (false, "يجب أن يوقع السكرتير قبلك");
            quote.SignatureManagerId = latestSignature.Id;
            _unitOfWork.quotes.UpdateValues(quote, quote);
        }
        else
        {
            return (false, "Invalid role");
        }

        await _unitOfWork.CompleteAsync();
        return (true, null);
    }

    }
}
