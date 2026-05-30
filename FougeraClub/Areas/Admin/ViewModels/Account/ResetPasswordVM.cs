using Domain.Resources;
using Infrastructure.Identity;
using KhaledTeamRecycling.Attributes;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace KhaledTeamRecycling.Areas.Admin.ViewModels.Account
{
    public class ResetPasswordVM
    {
        public string? Id { get; set; }
        [LocalizedRequired("Required")]
        [EmailAddress(ErrorMessageResourceName = "InvalidEmailAddress",ErrorMessageResourceType = typeof(Resource2))]
        public string? Email { get; set; }
        [LocalizedRequired("Required")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
        [DataType(DataType.Password)]
        [Compare("Password",ErrorMessageResourceName = "PasswordMismatch", ErrorMessageResourceType = typeof(Resource2))]
        public string? ConfirmPassword { get; set; }

        [LocalizedRequired("Required")]
        public string? Username { get; set; }
    }
}
