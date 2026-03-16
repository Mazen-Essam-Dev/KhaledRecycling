using Domain.Entities;
using Domain.Entities.Employees;
using Domain.Resources;
using KhaledTeamRecycling.Attributes;
using KhaledTeamRecycling.Helpers;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace KhaledTeamRecycling.Areas.Admin.ViewModels.Employees
{
    public class EmployeeVM : IValidatableObject
    {
        // for index page 
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public IEnumerable<Employee>? all_EmployeeListVM { get; set; }
        public EmployeeAttachmentsVM? EmployeeAtachmentsVM { get; set; }
        public string? SearchString { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public int? TotalCount { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }
       
        // end index
        public int? NationalityId { get; set; }
        public Nationality? Nationality { get; set; }
        public List<SelectListItem>? NationalitiesList { get; set; } = new();
        public int? Code { get; set; }

        public int Id { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource1), ErrorMessageResourceName = "Required")
            , MaxLength(100, ErrorMessageResourceType = typeof(Resource1), ErrorMessageResourceName = "MaxLength_100")]
        //[RegularExpression(@"^[\u0621-\u064A0-9 ]+$", ErrorMessage = "يجب أن يحتوي على حروف عربية وأرقام فقط")]
        public string? FullNameAr { get; set; }
       // [Required(ErrorMessageResourceType = typeof(Resource1), ErrorMessageResourceName = "Required")
       //, MaxLength(100, ErrorMessageResourceType = typeof(Resource1), ErrorMessageResourceName = "MaxLength_100")]
       // [RegularExpression(@"^[a-zA-Z0-9 ]+$", ErrorMessage = "must contain only English letters and numbers")]
        public string? FullNameEn { get; set; }

        [Phone(ErrorMessageResourceType = typeof(Resource1), ErrorMessageResourceName = "PhoneNumberisNotValid")]
        [LocalizedMaxLength(10, "PhoneInCorrect") /*, LocalizedMinLength(10, "PhoneInCorrect")*/]
        [Unique(typeof(Employee), nameof(PhoneNumber), ErrorMessageResourceType = typeof(Resource2), ErrorMessageResourceName = "EnterAnotherPhoneNumber")]
        public string? PhoneNumber { get; set; }

        [EmailAddress(ErrorMessageResourceType = typeof(Resource1), ErrorMessageResourceName = "EmailAddress")
            , MaxLength(100, ErrorMessageResourceType = typeof(Resource1), ErrorMessageResourceName = "MaxLength_100")]
        [Unique(typeof(Employee), nameof(Email), ErrorMessageResourceType = typeof(Resource2), ErrorMessageResourceName = "EnterAnotherEmail")]
        public string? Email { get; set; }

        [MaxLength(200,ErrorMessageResourceType =typeof(Resource1), ErrorMessageResourceName = "MaxLength_200")]
        public string? Address { get; set; }
        public string? JobTitle1 { get; set; }
        public string? Nationality1 { get; set; }


        [MaxLength(50, ErrorMessageResourceType = typeof(Resource1), ErrorMessageResourceName = "MaxLength_50")]
        [Unique(typeof(Employee), nameof(PassportNumber), ErrorMessageResourceType = typeof(Resource2), ErrorMessageResourceName = "EnterAnotherPassportNumber")]
        public string? PassportNumber { get; set; }

        [NotInThePast]
        public DateOnly? PassportExpiryDate { get; set; }
        [Unique(typeof(Employee), nameof(NationalIdNumber), ErrorMessageResourceType = typeof(Resource2), ErrorMessageResourceName = "EnterAnotherIdNumber")]
        public string? NationalIdNumber { get; set; }
        public string? g1 { get; set; }
        public string? g2 { get; set; }
        public string? g3 { get; set; }
        public string? g4 { get; set; }

        [NotInThePast]
        public DateOnly? NationalIdExpiryDate { get; set; }
        public string? NationalIdLocation { get; set; }


        [Required(ErrorMessageResourceType = typeof(Resource1), ErrorMessageResourceName = "Required")
                  , MaxLength(50, ErrorMessageResourceType = typeof(Resource1), ErrorMessageResourceName = "MaxLength_50")]
        public string? BankAccountNumber { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource1), ErrorMessageResourceName = "Required")
                  , MaxLength(100, ErrorMessageResourceType = typeof(Resource1), ErrorMessageResourceName = "MaxLength_100")]
        public string? BankName { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource1), ErrorMessageResourceName = "Required")]
        public double? Salary { get; set; }

        public string? Notes { get; set; }

        public IFormFile? Photo { get; set; } // Personal photo
        public string? PhotoPath { get; set; } // To display the image after saving if you want

        // Temporary uploaded file
        public string? Photo_TempFilePath { get; set; }

        // For Edit: store old file from DB
        public string? Photo_OldPath { get; set; }

        [MaxLength(200, ErrorMessageResourceType = typeof(Resource1), ErrorMessageResourceName = "MaxLength_200")]
        public string? JobTitle { get; set; }

        #region Check End Date > must be after the Start Date
        // must : IValidatableObject
        // Ex -> public class CourseVM : IValidatableObject
        // add Check End Date > must be after the Start Date
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (StartDate.HasValue && EndDate.HasValue)
            {
                if (EndDate <= StartDate)
                {
                    if (SessionHelper.GetCurrentLanguage() == "ar")
                    {
                        yield return new ValidationResult(
                            "تاريخ النهاية يجب أن يكون بعد تاريخ البداية",
                            new[] { nameof(EndDate) }  //so that the error appears under EndDate
                        );
                    }
                    else
                    {
                        yield return new ValidationResult(
                            "The End Date must be after the Start Date",
                            new[] { nameof(EndDate) }  //so that the error appears under EndDate
                        );
                    }

                }
            }
        }
        #endregion

    }
}
