using Domain.Entities;
using FougeraClub.Helpers;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace FougeraClub.Areas.Admin.ViewModels.Course;

public class SubscribedMemberCourseVMTrainer
{
    public PaginatedList<SubscribedMemberCourseVM> paginated { get; set; }
    public bool UserIsTrainer { get; set; }

    public string? trainerSelect { get; set; }
    public Trainer? Trainer { get; set; }
    public List<SelectListItem>? TrainersList { get; set; } = new();
}

public class SubscribedMemberCourseVM : IValidatableObject
{
    public bool UserIsTrainer { get; set; }
    [RegularExpression(@"^[\u0621-\u064A0-9 ]+$", ErrorMessage = "يجب أن يحتوي على حروف عربية وأرقام فقط")]
    [LocalizedRequired("Required"), LocalizedMaxLength(100, "MaxLength_100")]
    public string? MemberFullNameAr { get; set; }
    [RegularExpression(@"^[a-zA-Z0-9 ]+$", ErrorMessage = "must contain only English letters and numbers")]
    [LocalizedRequired("Required"), LocalizedMaxLength(100, "MaxLength_100")]
    public string? MemberFullNameEn { get; set; }
    [LocalizedRequired("Required"), LocalizedMaxLength(18, "Mast_Length_18")]
    public string? MemberIdNumber { get; set; }
    //[LocalizedRequired("Required")]
    public int? MemberId { get; set; }
    public int? MemberCode { get; set; }

    [LocalizedRequired("Required")]
    public int? MemberGenderId { get; set; }
    [LocalizedRequired("Required")]
    public int? NationalityId { get; set; }
    [RegularExpression(@"^[\u0621-\u064A0-9 ]+$", ErrorMessage = "يجب أن يحتوي على حروف عربية وأرقام فقط")]
    [LocalizedRequired("Required"), LocalizedMaxLength(100, "MaxLength_100")]
    public string? NationalityNameAr { get; set; }
    [RegularExpression(@"^[a-zA-Z0-9 ]+$", ErrorMessage = "must contain only English letters and numbers")]
    [LocalizedRequired("Required"), LocalizedMaxLength(100, "MaxLength_100")]
    public string? NationalityNameEn { get; set; }
    public int? DepartmentId { get; set; }
    [RegularExpression(@"^[\u0621-\u064A0-9 ]+$", ErrorMessage = "يجب أن يحتوي على حروف عربية وأرقام فقط")]
    [LocalizedRequired("Required"), LocalizedMaxLength(100, "MaxLength_100")]
    public string? DepartmentNameAr { get; set; }
    [RegularExpression(@"^[a-zA-Z0-9 ]+$", ErrorMessage = "must contain only English letters and numbers")]
    [LocalizedRequired("Required"), LocalizedMaxLength(100, "MaxLength_100")]
    public string? DepartmentNameEn { get; set; }
    [LocalizedRequired("Required"), LocalizedMaxLength(100, "MaxLength_100")]
    [RegularExpression(@"^[\u0621-\u064A0-9 ]+$", ErrorMessage = "يجب أن يحتوي على حروف عربية وأرقام فقط")]
    public string? CourseTitleAr { get; set; }
    [RegularExpression(@"^[a-zA-Z0-9 ]+$", ErrorMessage = "must contain only English letters and numbers")]
    [LocalizedRequired("Required"), LocalizedMaxLength(100, "MaxLength_100")]
    public string? CourseTitleEn { get; set; }
    public DateTime? SubscriptionDate { get; set; }
    public DateOnly? CourseStartDate { get; set; }
    public DateOnly? CourseEndDate { get; set; }
    [RegularExpression(@"^[\u0621-\u064A0-9 ]+$", ErrorMessage = "يجب أن يحتوي على حروف عربية وأرقام فقط")]
    public string? TrainerFullNameAr { get; set; }
    [RegularExpression(@"^[a-zA-Z0-9 ]+$", ErrorMessage = "must contain only English letters and numbers")]
    public string? TrainerFullNameEn { get; set; }
    public int SubscriptionId { get; set; }
    public bool? Acceptance { get; set; }
    public bool? IsAttendance { get; set; }
    public int? SelectedRate { get; set; }

    public string? SubNotes { get; set; }

    public int? CourseID { get; set; }

    public bool isCoursehasAcceptedOrRejectedMember { get; set; }

    public int? MemberTypeId { get; set; }
    public virtual MemberType? MemberType { get; set; }
    public string? MemberTypeAr { get; set; }
    public string? MemberTypeEn { get; set; }

    #region Check End Date > must be after the Start Date
    // must : IValidatableObject
    // Ex -> public class CourseVM : IValidatableObject
    // add Check End Date > must be after the Start Date
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (CourseStartDate.HasValue && CourseEndDate.HasValue)
        {
            if (CourseEndDate <= CourseStartDate)
            {
                if (SessionHelper.GetCurrentLanguage() == "ar")
                {
                    yield return new ValidationResult(
                        "تاريخ النهاية يجب أن يكون بعد تاريخ البداية",
                        new[] { nameof(CourseEndDate) }  //so that the error appears under EndDate
                    );
                }
                else
                {
                    yield return new ValidationResult(
                        "The End Date must be after the Start Date",
                        new[] { nameof(CourseEndDate) }  //so that the error appears under EndDate
                    );
                }

            }
        }
    }
    #endregion
}





