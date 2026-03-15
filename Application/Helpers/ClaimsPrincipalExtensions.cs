using Infrastructure.Repositories.InterfacesDB;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Application.Helpers
{
    public static class ClaimsPrincipalExtensions
    {
        public static string? GetUserId(this ClaimsPrincipal user)
        {
            return user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        public static async Task<string?> GetUserPhoneNumberAsync(this ClaimsPrincipal user, IUnitOfWork unitOfWork)
        {
            var userId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return null;

            var phoneNumber = await unitOfWork.Users.Table
                .Where(u => u.Id == userId)
                .Select(u => u.PhoneNumber)
                .FirstOrDefaultAsync();

            return phoneNumber;
        }
    }
}

