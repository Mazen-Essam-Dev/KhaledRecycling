using Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces.Admin
{
    public interface IActivityService
    {
        Task<IEnumerable<Activity>> GetAllAsync();
        Task<Activity?> GetByIdAsync(int id);
        Task<int> AddAsync(Activity entity, IFormFile? file);
        Task UpdateAsync(Activity entity, IFormFile? file);
        Task DeleteAsync(int id);
        Task<(IEnumerable<MemberEntity>?, IEnumerable<Subscription>?)> GetAllMembersOfActivityAsync(int ActivityId);



    }

}
