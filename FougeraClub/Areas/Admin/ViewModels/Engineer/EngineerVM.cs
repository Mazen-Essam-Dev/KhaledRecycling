using Application.Helpers;
using Domain.Entities;
using Domain.Resources;
using FougeraClub.Attributes;
using FougeraClub.Helpers;
using Humanizer;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace FougeraClub.Areas.Admin.ViewModels.Engineer;

public class EngineerVM : IValidatableObject
{
    public int Id { get; set; }

    [LocalizedRequired("Required"), LocalizedMaxLength(100, "MaxLength_100")]
    public string? FullName { get; set; }

    public int? Code { get; set; }


    [LocalizedRequired("Required"), LocalizedMaxLength(100, "MaxLength_100")]
    public string? Specialization { get; set; }

    [LocalizedRequired("Required")]
    [Range(1900, 2100, ErrorMessage = "عام التخرج غير صحيح")]
    public int? GraduationYear { get; set; }
    [DataType(DataType.Date)]
    [InThePast]
    public DateTime? DateOfBirth { get; set; }

    [LocalizedRequired("Required"), LocalizedMaxLength(100, "MaxLength_100")]
    public string? Position { get; set; }
    [LocalizedRequired("Required")]
    public int? NationalityId { get; set; }
    public virtual Nationality? Nationality { get; set; }
    public List<SelectListItem>? NationalityList { get; set; } = new();

    [LocalizedRequired("Required"), LocalizedMaxLength(200, "MaxLength_200")]
    public string? WorkAddress { get; set; }

    [LocalizedRequired("Required"), LocalizedMaxLength(200, "MaxLength_200")]
    public string? Address { get; set; }

    [Phone(ErrorMessageResourceType = typeof(Resource1), ErrorMessageResourceName = "PhoneNumberisNotValid")]
    [LocalizedMaxLength(10, "PhoneInCorrect") /*, LocalizedMinLength(10, "PhoneInCorrect")*/]
    [Unique(typeof(Domain.Entities.Engineer), nameof(PhoneNumber), ErrorMessageResourceType = typeof(Resource2), ErrorMessageResourceName = "EnterAnotherPhoneNumber")]
    public string? PhoneNumber { get; set; }

        public string? Mobile { get; set; }


    [EmailAddress(ErrorMessageResourceType = typeof(Resource2), ErrorMessageResourceName = "Invalid")]
    [LocalizedMaxLength(200, "MaxLength_100")]
    [Unique(typeof(Domain.Entities.Engineer), nameof(Email), ErrorMessageResourceType = typeof(Resource2), ErrorMessageResourceName = "EnterAnotherEmail")]
    public string? Email { get; set; }

    public string? Notes { get; set; }

    public IFormFile? ProfileImage { get; set; }

    [MaxLength(255)]
    public string? ProfileImagePath { get; set; }

    public string? PassportNumber { get; set; }

    [DataType(DataType.Date)]
    public DateOnly? PassportExpiryDate { get; set; }

    public string? IdNumber { get; set; }
    public string? g1 { get; set; }
    public string? g2 { get; set; }
    public string? g3 { get; set; }
    public string? g4 { get; set; }

    [DataType(DataType.Date)]
    public DateOnly? IdExpiryDate { get; set; }

    public string? IdReleaseLocation { get; set; }

    public decimal? Salary { get; set; }

    public string? BankAccountNo { get; set; }

    public string? BankName { get; set; }

    // Temporary uploaded file
    public string? TempFilePath { get; set; }

    // For Edit: store old file from DB
    public string? OldPath { get; set; }

    #region Check Date Expired and DateOfBirth > 9
    // must : IValidatableObject
    // Ex -> public class CourseVM : IValidatableObject
    // add Check Date Expired
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var Datenow = AppDubaiTime.Now.AtMidnight(); // Get TodayDate --> AtMidnight()
        // Only validate expiry dates if the related document number is provided
        if (!string.IsNullOrWhiteSpace(IdNumber) && IdExpiryDate.HasValue)
        {
            if (IdExpiryDate?.ToDateTime(TimeOnly.MinValue) /*to Convert DateOnly to DateTime --> AtMidnight() */ <= Datenow)
            {
                if (SessionHelper.GetCurrentLanguage() == "ar")
                {
                    yield return new ValidationResult(
                        "عفواً تاريخ الهوية منتهي",
                        new[] { nameof(IdExpiryDate) }  //so that the error appears under ExpiryDate
                    );
                }
                else
                {
                    yield return new ValidationResult(
                        "Sorry, the identity date has expired",
                        new[] { nameof(IdExpiryDate) }  //so that the error appears under ExpiryDate
                    );
                }
            }
        }

        if (!string.IsNullOrWhiteSpace(PassportNumber) && PassportExpiryDate.HasValue)
        {
            if (PassportExpiryDate?.ToDateTime(TimeOnly.MinValue) /*to Convert DateOnly to DateTime --> AtMidnight() */ <= Datenow)
            {
                if (SessionHelper.GetCurrentLanguage() == "ar")
                {
                    yield return new ValidationResult(
                        "عفواً تاريخ الباسبور منتهي",
                        new[] { nameof(PassportExpiryDate) }  //so that the error appears under ExpiryDate
                    );
                }
                else
                {
                    yield return new ValidationResult(
                        "Sorry, the Passport date has expired",
                        new[] { nameof(PassportExpiryDate) }  //so that the error appears under ExpiryDate
                    );
                }
            }
        }
        var DateBefore_9Years = AppDubaiTime.Now.AddYears(-9).AtMidnight(); // Get TodayDate --> AtMidnight()
        if (DateOfBirth.HasValue)
        {
            if (DateOfBirth?.AtMidnight() /*to Convert DateOnly to DateTime --> AtMidnight() */ > DateBefore_9Years)
            {
                if (SessionHelper.GetCurrentLanguage() == "ar")
                {
                    yield return new ValidationResult(
                        "عفواً تاريخ الميلاد غير مسموح تحت 9 أعوام",
                        new[] { nameof(DateOfBirth) }  //so that the error appears under DateOfBirth
                    );
                }
                else
                {
                    yield return new ValidationResult(
                        "Sorry, Date of Birth is not allowed under 9 years old",
                        new[] { nameof(DateOfBirth) }  //so that the error appears under DateOfBirth
                    );
                }

            }
        }

    }
    #endregion
}
