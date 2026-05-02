using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Contracts;
using System.Net.NetworkInformation;

namespace Infrastructure.Identity;

// Add profile data for application users by adding properties to the ApplicationUser class
public class ApplicationUser : IdentityUser
{
    [MaxLength(250)]
    public string? FullNameAr { get; set; }
    [MaxLength(250)]
    public string? FullNameEn { get; set; }
    public int? FKUserType { get; set; }
    public string? Phone1 { get; set; }
    public string? Phone2 { get; set; }
    public string? Address { get; set; }
    public string? CommercialRegistrationNumber { get; set; }
    public string? TaxRecordNumber { get; set; }
    public int? StatusId { get; set; }

    [ForeignKey("FKUserType")]
    public UserType? UserType { get; set; }

    [ForeignKey("StatusId")]
    public Status? Status { get; set; }

    public ICollection<Domain.Entities.Contract.Contract>? Contracts { get; set; }
    public byte[]? ProfilePicture { get; set; }

    public virtual Signature? Signature { get; set; }
}


