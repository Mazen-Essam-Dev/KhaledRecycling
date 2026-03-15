using Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces.Admin
{
    public interface ISupplierService
    {
        Task<IEnumerable<Supplier>> GetAllAsync();
        Task<IEnumerable<SupplierCategory>> GetAllSupplierCategoryAsync();
        Task<Supplier?> GetByIdAsync(int id);
        Task<int> AddAsync(Supplier entity);
        Task UpdateAsync(Supplier entity);
        Task DeleteAsync(int id);
        Task<string> SaveImageAsync(IFormFile file);
        void DeleteImageFile(string? relativePath);

    }

}
