using Domain.Resources;
using FougeraClub.Attributes;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace FougeraClub.Areas.Admin.ViewModels.Account
{
    public class AdminVM
    {
        public string? Id { get; set; }

        [LocalizedRequired("Required"), LocalizedMaxLength(200, "MaxLength_200")]
        [RegularExpression(@"^[\u0621-\u064A0-9 ]+$", ErrorMessage = "يجب أن يحتوي على حروف عربية وأرقام فقط")]
        public string? FullNameAr { get; set; }
        [LocalizedRequired("Required"), LocalizedMaxLength(200, "MaxLength_200")]
        [RegularExpression(@"^[a-zA-Z0-9 ]+$", ErrorMessage = "must contain only English letters and numbers")]
        public string? FullNameEn { get; set; }
     
        [LocalizedRequired("Required")]
        [Remote(action: "CheckUsernameIfExists", controller: "Account", areaName: "Admin", AdditionalFields = nameof(Id))]
        [Unique(typeof(ApplicationUser), nameof(Email), ErrorMessageResourceType = typeof(Resource2), ErrorMessageResourceName = "EnterAnotherUserName")]
        public string Username { get; set; }

        [LocalizedRequired("Required")]
        [EmailAddress(ErrorMessageResourceType = typeof(Resource2), ErrorMessageResourceName = "EmailInvalid")]
        [Unique(typeof(ApplicationUser), nameof(Username), ErrorMessageResourceType = typeof(Resource2), ErrorMessageResourceName = "EnterAnotherEmail")]
        public string? Email { get; set; }

        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[@$!#%*?&])[A-Za-z\d@$!%#*?&]{8,}$",
            ErrorMessageResourceType = typeof(Resource1), ErrorMessageResourceName = "PasswordShouldBe2")]
        public string? PasswordHash { get; set; }

        [DataType(DataType.Password)]
        [Compare("PasswordHash", ErrorMessageResourceType = typeof(Resource2), ErrorMessageResourceName = "PasswordMismatch")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[@$!#%*?&])[A-Za-z\d@$!%#*?&]{8,}$",
            ErrorMessageResourceType = typeof(Resource1), ErrorMessageResourceName = "PasswordShouldBe2")]
        public string? ConfirmPassword { get; set; }

        [Phone(ErrorMessageResourceType = typeof(Resource1), ErrorMessageResourceName = "PhoneNumberisNotValid")]
        [LocalizedRequired("Required"), LocalizedMaxLength(10, "PhoneInCorrect") /*, LocalizedMinLength(10, "PhoneInCorrect")*/]
        public string? PhoneNumber { get; set; }


        [LocalizedRequired("Required")]
        public string? RoleId { get; set; }
        public string? Role { get; set; }
        public int? RoleNumber { get; set; }
        public IEnumerable<SelectListItem>? RolesList { get; set; }

        public SignatureVM? Signature { get; set; }
        public bool IsTrainer { get; set; } = false;
    }
}
