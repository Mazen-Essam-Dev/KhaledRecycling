using Application.Interfaces.Admin;
using Infrastructure.Repositories.InterfacesDB;
using Domain.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Admin
{
    public class SupplierService : ISupplierService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _env;

        public SupplierService(IUnitOfWork unitOfWork, IWebHostEnvironment env)
        {
            _unitOfWork = unitOfWork;
            _env = env;
        }
        public async Task<IEnumerable<Supplier>> GetAllAsync()
        {
            var context = _unitOfWork.Context;

            var suppliers = await _unitOfWork.Suppliers.GetQueryable()
                .Select(s => new Supplier
                {
                    Id = s.Id,
                    SupplierNameAr = s.SupplierNameAr,
                    SupplierNameEn = s.SupplierNameEn,
                    VATNumber = s.VATNumber,
                    Phone = s.Phone,
                    Mobile = s.Mobile,
                    Email = s.Email,
                    Address = s.Address,
                    Description = s.Description,
                    HasRelatedData =
                        context.ExpenseAndReceiptAndOther.Any(e => e.SupplierId == s.Id) ||
                        context.PurchaseOrders.Any(po => po.SupplierId == s.Id)
                        ,
                    SupplierCategoryId = s.SupplierCategoryId,
                    //SupplierCategory = new SupplierCategory
                    //{
                    //    Id = s.SupplierCategory != null ? s.SupplierCategory.Id : 0,
                    //    NameAr = s.SupplierCategory != null ? s.SupplierCategory.NameAr : string.Empty,
                    //    NameEn = s.SupplierCategory != null ? s.SupplierCategory.NameEn : string.Empty
                    //}
                })
                .AsNoTracking()
                .ToListAsync();

           
            return suppliers;
        }

        public async Task<IEnumerable<SupplierCategory>> GetAllSupplierCategoryAsync()
        {
            return await _unitOfWork.SupplierCategorys.GetAllAsync();
        }

        public async Task<Supplier?> GetByIdAsync(int id)
        {
            return await _unitOfWork.Suppliers
                .GetByIdAsync(e => e.Id == id);
        }

        public async Task<int> AddAsync(Supplier entity)
        {
            var entit= await _unitOfWork.Suppliers.AddAsync(entity);
            await _unitOfWork.CompleteAsync();
            return entit.Id;
        }

        public async Task UpdateAsync(Supplier entity)
        {
            var existing = await _unitOfWork.Suppliers.GetByIdAsync(entity.Id);
            if (existing == null) return;

            _unitOfWork.Suppliers.UpdateValues(existing, entity);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _unitOfWork.Suppliers.GetByIdAsync(id);
            if (entity != null)
            {
                _unitOfWork.Suppliers.Delete(entity);
                await _unitOfWork.CompleteAsync();
            }
        }


        public async Task<string> SaveImageAsync(IFormFile file)
        {
            var folder = Path.Combine(_env.WebRootPath, "uploads/Suppliers");
            Directory.CreateDirectory(folder);
            var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            var fullPath = Path.Combine(folder, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"uploads/Suppliers/{fileName}";
        }
        public void DeleteImageFile(string? relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath)) return;

            var fullPath = Path.Combine(_env.WebRootPath, relativePath.Replace("/", Path.DirectorySeparatorChar.ToString()));
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }

    }

}
