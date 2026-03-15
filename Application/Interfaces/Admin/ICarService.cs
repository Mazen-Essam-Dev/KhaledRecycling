using Domain.Entities;
using Domain.DTOs.Admin.Car;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces.Admin
{
    public interface ICarService
    {
        Task<IEnumerable<Car>> GetAllAsync();
        Task<Car?> GetByIdAsync(int id);
        Task<int> AddAsync(Car entity, IFormFile? file);
        Task UpdateAsync(Car entity, IFormFile? file);
        Task DeleteAsync(int id);
        Task UploadAttachmentsAsync(CarAttachmentsDTO model);
        Task<CarAttachmentsDTO> GetAttachmentsAsync(int id);
    }

}
