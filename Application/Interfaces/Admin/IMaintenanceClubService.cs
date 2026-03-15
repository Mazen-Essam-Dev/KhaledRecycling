using Domain.DTOs;
using Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces.Admin
{
    public interface IMaintenanceClubService
    {
        Task<IEnumerable<MaintenanceClub>> GetAllAsync();
        Task<MaintenanceClub?> GetByIdAsync(int id);
        Task<int> AddAsync(MaintenanceClub entity);
        Task UpdateAsync(MaintenanceClub entity);
        Task DeleteAsync(int id);
        Task<MaintenanceClubAttachmentsDTO> GetAttachmentsAsync(int maintenanceClubId);
        Task UploadAttachmentsAsync(MaintenanceClubAttachmentsDTO dto);
    }

}
