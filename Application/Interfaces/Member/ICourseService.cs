using Domain.Entities;

namespace Application.Interfaces.Member
{
    public interface ICourseService
    {
        Task SubscribeAsync(int CourseId, string username);
        Task<bool> AddRateAsync(int CourseId, string username,int? rate);
        Task<(IEnumerable<Course> Courses, IEnumerable<Subscription> Subscriptions)> GetAllAsync(string username);
        Task<Course?> GetByIdAsync(int id);
        Task AddAsync(Course Course);
        Task UpdateAsync(Course Course);
        Task DeleteAsync(int id);
        bool Exists(int id);

    }
}
