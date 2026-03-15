using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Identity;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
    [MaxLength(250)]
    public string? FullNameAr { get; set; }
    [MaxLength(250)]
    public string? FullNameEn { get; set; }
    public byte[]? ProfilePicture { get; set; }

    public virtual Signature? Signature { get; set; }
}


