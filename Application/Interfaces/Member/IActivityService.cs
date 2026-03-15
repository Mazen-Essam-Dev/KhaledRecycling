using Domain.Entities;

namespace Application.Interfaces.Member
{
    public interface IActivityService
    {
        Task<(IEnumerable<Activity> Activities, IEnumerable<Subscription> Subscriptions)> GetAllAsync(string username);
        Task<Activity?> GetByIdAsync(int id);
        Task AddAsync(Activity activity);
        Task UpdateAsync(Activity activity);
        Task SubscribeAsync(int activityId, string email);
        Task<bool> CheckAgeAsync(int activityId, string email);
        Task DeleteAsync(int id);
        bool Exists(int id);
    }
}
