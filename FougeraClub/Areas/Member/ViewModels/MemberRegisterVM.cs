using Application.Helpers;
using Domain.Entities;
using Domain.Entities.Employees;
using Domain.Enums;
using Domain.Resources;
using KhaledTeamRecycling.Attributes;
using KhaledTeamRecycling.Helpers;
using Humanizer;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KhaledTeamRecycling.Areas.Member.ViewModels
{
    public class MemberRegisterVM : IValidatableObject
    {
        public int Id { get; set; }
        public int? Code { get; set; }

        [LocalizedRequired("Required"), LocalizedMaxLength(100, "MaxLength_100")]
        [RegularExpression(@"^[\u0621-\u064A0-9 ]+$", ErrorMessage = "يجب أن يحتوي على حروف عربية وأرقام فقط")]
        public string? FullNameAr { get; set; }
        [LocalizedRequired("Required"), LocalizedMaxLength(100, "MaxLength_100")]
        [RegularExpression(@"^[a-zA-Z0-9 ]+$", ErrorMessage = "must contain only English letters and numbers")]
        public string? FullNameEn { get; set; }
        //[LocalizedRequired("Required")]
        public int? NationalityId { get; set; }
        public List<SelectListItem>? Nationalities { get; set; }
        public List<SelectListItem>? NationalitiesOne { get; set; }
        [LocalizedRequired("Required")]
        public int? GenderId { get; set; }
        public SelectList? Genders { get; set; }
        public Gender? SelectedGenderEnum { get; set; }
        public IEnumerable<SelectListItem>? GenderEnumList { get; set; }


        [RegularExpression(@"\b\d{3}-\d{4}-\d{7}-\d\b", ErrorMessageResourceType = typeof(Resource1), ErrorMessageResourceName = "Mast_Length_18")]
        [LocalizedRequired("Required"), LocalizedMaxLength(18, "Mast_Length_18"), LocalizedMinLengthAttribute(18, "Mast_Length_18")]
        [Unique(typeof(MemberEntity), nameof(IdNumber), ErrorMessageResourceType = typeof(Resource2), ErrorMessageResourceName = "EnterAnotherIdNumber")]
        public string? IdNumber { get; set; }
        public string? g1 { get; set; }
        public string? g2 { get; set; }
        public string? g3 { get; set; }
        public string? g4 { get; set; }      

        [LocalizedRequired("Required")]
        [DataType(DataType.Date)]
        [NotInThePast]
        public DateOnly? IdExpiryDate { get; set; }
        [LocalizedRequired("Required")]
        [DataType(DataType.Date)]
        [InThePast]
        public DateOnly? DateOfBirth { get; set; }
        [LocalizedRequired("Required")]
        public int? Age { get; set; }

        [Phone(ErrorMessageResourceType = typeof(Resource1), ErrorMessageResourceName = "PhoneNumberisNotValid")]
        [LocalizedRequired("Required"), LocalizedMaxLength(10, "PhoneInCorrect") /*, LocalizedMinLength(10, "PhoneInCorrect")*/]
        [Unique(typeof(MemberEntity), nameof(PhoneNumber), ErrorMessageResourceType = typeof(Resource2), ErrorMessageResourceName = "EnterAnotherPhoneNumber")]
        public string? PhoneNumber { get; set; }

        [Phone(ErrorMessageResourceType = typeof(Resource1), ErrorMessageResourceName = "PhoneNumberisNotValid")]
        [/*LocalizedRequired("Required"),*/ LocalizedMaxLength(10, "PhoneInCorrect") /*, LocalizedMinLength(10, "PhoneInCorrect")*/]
        public string? FatherPhone { get; set; }

        [Phone(ErrorMessageResourceType = typeof(Resource1), ErrorMessageResourceName = "PhoneNumberisNotValid")]
        [/*LocalizedRequired("Required"),*/ LocalizedMaxLength(10, "PhoneInCorrect") /*, LocalizedMinLength(10, "PhoneInCorrect")*/]
        public string? MotherPhone { get; set; }

        [LocalizedRequired("Required"), LocalizedMaxLength(100, "MaxLength_100")]
        public string? AcademicQualification { get; set; }

        [LocalizedRequired("Required"), LocalizedMaxLength(100, "MaxLength_100")]
        public string? EducationInstitution { get; set; }
        [LocalizedRequired("Required")]
        public int? ProfessionId { get; set; }
        public SelectList? Professions { get; set; }
        public Profession? SelectedProfessionEnum { get; set; }
        public IEnumerable<SelectListItem>? ProfessionEnumList { get; set; }

        [LocalizedRequired("Required")]
        public int? CityId { get; set; }
        public List<SelectListItem>? Cities { get; set; }


        [LocalizedRequired("Required"), LocalizedMaxLength(50, "MaxLength_50")]
        public string? Profession { get; set; }

        [LocalizedRequired("Required"), LocalizedMaxLength(100, "MaxLength_100")]
        public string? ProfessionPlace { get; set; }

        [LocalizedRequired("Required"), LocalizedMaxLength(50, "MaxLength_50")]
        public string? GuardianProfession { get; set; }

        [LocalizedRequired("Required"), LocalizedMaxLength(200, "MaxLength_200")]
        public string? Address { get; set; }
        [LocalizedRequired("Required"), LocalizedMaxLength(500, "MaxLength_500")]
        public string? Hobby { get; set; }
        [LocalizedRequired("Required"), LocalizedMaxLength(500, "MaxLength_500")]
        public string? Languages { get; set; }

        public int? HeardBy { get; set; }
        public SelectList? HeardByOptions { get; set; }
        public HeardBySources? SelectedHeardByEnum { get; set; }
        public IEnumerable<SelectListItem>? HeardByEnumList { get; set; }

        [LocalizedRequired("Required")]
        public bool? License { get; set; }

        [LocalizedMaxLength(100, "MaxLength_100")]
        public string? Facebook { get; set; }

        [LocalizedMaxLength(100, "MaxLength_100")]
        public string? Xplatform { get; set; }

        [LocalizedMaxLength(100, "MaxLength_100")]
        public string? Instagram { get; set; }

        [LocalizedRequired("Required"), LocalizedMaxLength(100, "MaxLength_100")]
        [Unique(typeof(MemberEntity), nameof(Email), ErrorMessageResourceType = typeof(Resource2), ErrorMessageResourceName = "EnterAnotherEmail")]
        public string? Email { get; set; }

        [LocalizedRequired("Required")]
        [LocalizedMaxLength(500, "MaxLength_500")]
        [DataType(DataType.Password)]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[a-z])(?=.*\d)(?=.*[@$!#%*?&])[A-Za-z\d@$!%#*?&]{8,}$",
            ErrorMessageResourceType = typeof(Resource1), ErrorMessageResourceName = "PasswordShouldBe2")]
        public string? Password { get; set; }

        [LocalizedRequired("Required"), LocalizedMaxLength(500, "MaxLength_500")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessageResourceType = typeof(Resource1), ErrorMessageResourceName = "PasswordDoesNotMatch")]
        public string? ConfirmPassword { get; set; }
        [MaxLength(300)]
        public string? ProfileImagePath { get; set; }
        public IFormFile? ProfileImage { get; set; }
        [MaxLength(300)]
        public string? IdImagePath { get; set; }
        public IFormFile? IdImage { get; set; }

        [MaxLength(300)]
        public string? PassportImagePath { get; set; }
        public IFormFile? PassportImage { get; set; }

        [LocalizedRequired("Required")]
        public int? MemberTypeId { get; set; }
        public virtual MemberType? MemberType { get; set; }

        #region Check Date Expired and DateOfBirth > 9
        // must : IValidatableObject
        // Ex -> public class CourseVM : IValidatableObject
        // add Check Date Expired
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var Datenow = AppDubaiTime.Now.AtMidnight(); // Get TodayDate --> AtMidnight()
            if (IdExpiryDate.HasValue)
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
            var DateBefore_9Years = AppDubaiTime.Now.AddYears(-9).AtMidnight(); // Get TodayDate --> AtMidnight()
            if (DateOfBirth.HasValue)
            {
                if (DateOfBirth?.ToDateTime(TimeOnly.MinValue) /*to Convert DateOnly to DateTime --> AtMidnight() */ > DateBefore_9Years)
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
}


