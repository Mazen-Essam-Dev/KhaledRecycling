//using Application.Helpers;
//using Application.Interfaces.Admin;
//using DocumentFormat.OpenXml.InkML;
//using Domain.Entities;
//using Domain.Entities.MaterialOrder;
//using Infrastructure.Repositories.InterfacesDB;
//using Microsoft.AspNetCore.Hosting;
//using Microsoft.AspNetCore.Http;
//using Microsoft.EntityFrameworkCore;
//using System.Linq.Expressions;

//namespace Application.Services.Admin
//{
//    public class MaterialOrderService : IMaterialOrderService
//    {
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IWebHostEnvironment _env;
//        private readonly string FileName = "MaterialOrders";
//        private readonly IHttpContextAccessor _httpContextAccessor;



//        public MaterialOrderService(IUnitOfWork unitOfWork, IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor)
//        {
//            _unitOfWork = unitOfWork;
//            _env = env;
//            _httpContextAccessor = httpContextAccessor;
//        }

//        public async Task<IEnumerable<MaterialOrder>> GetAllAsync()
//        {
//            return await _unitOfWork.MaterialOrders.GetAllAsync(e => e.SignatureUser!, e => e.SignatureUser!,x=>x.Department!);
//        }

  
//        public async Task<string> GetNewCodeAsync()
//        {
//            var lastId = await _unitOfWork.MaterialOrders.Table.Select(x => (int?)x.Id).MaxAsync() ?? 0;

//            var getLastCode = await _unitOfWork.MaterialOrders.Table.Where(x => x.Id == lastId).Select(x => x.MaterialOrderCode).FirstOrDefaultAsync();

//            if (!int.TryParse(getLastCode, out var numericCode))
//                numericCode = 0; // start 1

//            return (numericCode + 1).ToString("D4");
//        }

//        //public async Task<MaterialOrder?> GetByIdAsync(int id)
//        //{
//        //    return await _unitOfWork.MaterialOrders
//        //    .GetByIdAsync(
//        // e => e.Id == id,
//        // includes: new Expression<Func<MaterialOrder, object>>[]
//        // {
//        //        e => e.Items,
//        //        e => e.SignatureUser!,
//        //        e => e.SignatureManager!,
//        //        e => e.Department!
//        // });
//        //}
//        public async Task<MaterialOrder?> GetByIdAsync(int id)
//        {
//            return await _unitOfWork.MaterialOrders
//                .GetByIdAsync(e => e.Id == id, e => e.Items, e => e.SignatureUser!, e => e.SignatureManager!, e => e.Department!);
//        }
//        public async Task<IEnumerable<int>> GetAllYearsInDb()
//        {
//            var allRecords = await _unitOfWork.MaterialOrders.GetAllAsync();
//            var allYears = allRecords
//                .Where(e => e.Date.HasValue)
//                .Select(e => e.Date.Value.Year)
//                .Distinct()
//                .OrderBy(y => y)
//                .ToList();

//            return allYears;
//        }

//        public async Task<int> AddAsync(MaterialOrder entity)
//        {
//            int n = 0;
//            decimal totalAll = 0.00m;

//            // 🔹 Add new items
//            foreach (var item in entity.Items)
//            {
//                // sum Total
//                if (item.SinglePrice == null) item.SinglePrice = 0.00m;
//                if (item.Quantity == null) item.Quantity = 0;
//                var totalRecord = item.SinglePrice.Value * item.Quantity.Value;
//                totalAll = totalAll + totalRecord;
//                //------ End Summition

//                n++;
//                //// Set the ID manually
//                //item.MaterialOrderItemId = n;
//                item.MaterialOrderId = entity.Id;
//            }


//            entity.OrderTotal = totalAll;

//            var codeExists = _unitOfWork.MaterialOrders.Table.Any(x => x.MaterialOrderCode == entity.MaterialOrderCode);
//            if (codeExists)
//            {
//                entity.MaterialOrderCode = await GetNewCodeAsync();
//            }
//            var entit = await _unitOfWork.MaterialOrders.AddAsync(entity);
//            await _unitOfWork.CompleteAsync();
//            return entit.Id;
//        }

//        public async Task UpdateAsync(MaterialOrder entity)
//        {
//            var existing = await _unitOfWork.MaterialOrders.GetByIdAsync(
//                e => e.Id == entity.Id
//            );

//            if (existing == null)
//                return;

//            // 🔹 Delete all old items
//            var oldItems = await _unitOfWork.MaterialOrderItems.GetAllAsync(x => x.MaterialOrderId == entity.Id);
//            _unitOfWork.MaterialOrderItems.RemoveRange(oldItems);


//            int n = 0;
//            decimal totalAll = 0.00m;
//            decimal VATTotal = 0.00m;

//            // 🔹 Add new items
//            foreach (var item in entity.Items)
//            {
//                // sum Total
//                if (item.SinglePrice == null) item.SinglePrice = 0.00m;
//                if (item.Quantity == null) item.Quantity = 0;
//                var totalRecord = item.SinglePrice.Value * item.Quantity.Value;
//                totalAll = totalAll + totalRecord;
//                //------ End Summition

//                n++;
//                //// Set the ID manually
//                // item.MaterialOrderItemId = n;
//                item.MaterialOrderId = existing.Id;
//                await _unitOfWork.MaterialOrderItems.AddAsync(item);
//            }


//            entity.OrderTotal = totalAll;
//            entity.UserId = existing.UserId;

//            // 🔹 Update global values (without items)
//            _unitOfWork.MaterialOrders.UpdateValues(existing, entity);

//            //🔹 Save changes
//            await _unitOfWork.CompleteAsync();
//        }

//        public async Task DeleteAsync(int id)
//        {
//            var entity = await _unitOfWork.MaterialOrders.GetByIdAsync(id);
//            if (entity != null)
//            {
//                _unitOfWork.MaterialOrderItems.RemoveRange(entity.Items);
//                _unitOfWork.MaterialOrders.Delete(entity);
//                await _unitOfWork.CompleteAsync();
//            }
//        }

//        public async Task<bool> CheckIsMonthRegistedBefore(int id, DateOnly? dateOnly)
//        {
//            if (dateOnly == null) { return true; }
//            var allPreviousWithMonth = await _unitOfWork.MaterialOrders.GetAllAsync(x => x.Date != null && x.Date.Value.Year == dateOnly.Value.Year && x.Date.Value.Month == dateOnly.Value.Month && x.Id != id);
//            if (allPreviousWithMonth != null && allPreviousWithMonth.Count() > 0) { return true; }
//            return false;
//        }

//    //    public async Task<bool> SendOtpAsync()
//    //    {
//    //        var (status, code) = await OTPHelper.SaveOtpAsync(_httpContextAccessor, _unitOfWork);

//    //        if (status == false) return false;

//    //        var resultStatus = await _SMSForSendingOTPService.SendOtpSMSAsync(code);

//    //        return resultStatus.Item1;
//    //    }

//    //public async Task<(bool success, string? message)> ValidateOtpAsync(int id, string code, string role)
//    //{
//    //    var success = await OTPHelper.ValidateOtpAsync(_httpContextAccessor, _unitOfWork, code);
//    //    if (!success)
//    //        return (false, "Invalid OTP");

//    //    var materialOrder = await GetByIdAsync(id);
//    //    if (materialOrder == null)
//    //        return (false, "Record not found");

//    //    // Get the latest signature of the current user
//    //    var userId = _httpContextAccessor.HttpContext.User.GetUserId();
//    //    var allSignatures = await _unitOfWork.Signatures.GetAllAsync();
//    //    var latestSignature = allSignatures
//    //        .Where(s => s.UserId == userId)
//    //        .OrderByDescending(s => s.CreatedAt)
//    //        .FirstOrDefault();

//    //    if (latestSignature == null)
//    //        return (false, "Signature not found");

//    //    if (role == "Trainer")
//    //    {
//    //        materialOrder.SignatureUserId = latestSignature.Id;
//    //        _unitOfWork.MaterialOrders.UpdateValues(materialOrder, materialOrder);
//    //    }
//    //    else if (role == "Manager")
//    //    {
//    //        if (materialOrder.SignatureUser == null || materialOrder.SignatureUserId == null)
//    //            return (false, "يجب أن يوقع مقدم الطلب أولاً");
//    //        materialOrder.SignatureManagerId = latestSignature.Id;
//    //        _unitOfWork.MaterialOrders.UpdateValues(materialOrder, materialOrder);
//    //    }
//    //    else
//    //    {
//    //        return (false, "Invalid role");
//    //    }

//    //    await _unitOfWork.CompleteAsync();
//    //    return (true, null);
//    //}

//    }
//}
