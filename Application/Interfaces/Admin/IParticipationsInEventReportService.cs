using Domain.Entities.ParticipationsInEventReport;

namespace Application.Interfaces.Admin
{
    public interface IParticipationsInEventReportService
    {
        Task<IEnumerable<ParticipationsInEventReport>> GetAllAsync();
        Task<ParticipationsInEventReport?> GetByIdAsync(int id);
        Task<IEnumerable<int>> GetAllYearsInDb();
        Task<int> AddAsync(ParticipationsInEventReport entity);
        Task UpdateAsync(ParticipationsInEventReport entity);
        Task DeleteAsync(int id);
        Task<bool> SendOtpAsync(int id, string role);
        Task<(bool success, string? message)> ValidateOtpAsync(int id, string code, string role, System.Security.Claims.ClaimsPrincipal user);
    }

}
