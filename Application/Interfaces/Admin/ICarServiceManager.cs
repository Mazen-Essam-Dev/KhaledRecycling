using Domain.Entities;
using Domain.DTOs.Admin.CarService;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces.Admin
{
    public interface ICarServiceManager
    {
        Task<IEnumerable<CarServiceEntity>> GetAllAsync();
        Task<CarServiceEntity?> GetByIdAsync(int id);
        Task<(int, int)> AddAsync(CarServiceEntity entity, IFormFile? file);
        Task UpdateAsync(CarServiceEntity entity, IFormFile? file);
        Task DeleteAsync(int id);
        Task UploadAttachmentsAsync(CarServiceAttachmentsDTO model);
        Task<CarServiceAttachmentsDTO> GetAttachmentsAsync(int id);
    }
}
