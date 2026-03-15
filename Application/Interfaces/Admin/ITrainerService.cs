using Domain.DTOs.Admin;
using Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces.Admin
{
    public interface ITrainerService
    {
        Task<IEnumerable<Trainer>> GetAllAsync();
        Task<int?> GetThisTrainerId_IfTrainer_else_0(string? UserEMail);
        Task<List<TrainersNameDTO>?> GetAllTrainerNamesAr_En_only();
        Task<List<TrainersNameDTO>?> GetAllUsersNamesAr_En_only();
        Task<List<TrainersNameDTO>?> GetAllUsersNotTrainers_NamesAr_En_only();
        Task<List<TrainersNameDTO>?> GetAllUsersNotTrainers_NamesAr_En_only(string currentUserId);
        Task<Trainer?> GetByIdAsync(int id);
        Task<Trainer?> GetByIdAsync_byuserId(string id);
        Task<int> AddAsync(Trainer entity, IFormFile? file);
        Task UpdateAsync(Trainer entity, IFormFile? file);
        Task DeleteAsync(int id);
        Task<string> SaveImageAsync(IFormFile file);
        void DeleteImageFile(string? relativePath);

    }

}
