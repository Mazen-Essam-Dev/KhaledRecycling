using Domain.Enums;

namespace Application.Interfaces.Admin
{
    public interface IScientificProjectsService
    {
       
        Task<bool> SendOtpAsync(int id, string role);
        Task<(bool success, string? message)> ValidateOtpAsync(int id, string code, string role, System.Security.Claims.ClaimsPrincipal user);
    }

}
