using Domain.Entities;
using KhaledTeamRecycling.Helpers;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KhaledTeamRecycling.Areas.Admin.ViewModels.Course;

public class CourseVM : IValidatableObject
{
    public int Id { get; set; }

    [LocalizedRequired("Required")]
    public int DepartmentId { get; set; }
    public Department? Department { get; set; }
    public List<SelectListItem>? DepartmentsList { get; set; } = new();
    [LocalizedRequired("Required")]
    public int TrainerId { get; set; }
    public List<SelectListItem>? TrainersList { get; set; } = new();


    [LocalizedRequired("Required"), LocalizedMaxLength(200, "MaxLength_200")]
    [RegularExpression(@"^[\u0621-\u064A0-9 ]+$", ErrorMessage = "يجب أن يحتوي على حروف عربية وأرقام فقط")]
    public string TitleAr { get; set; }

    [LocalizedRequired("Required"), LocalizedMaxLength(200, "MaxLength_200")]
    [RegularExpression(@"^[a-zA-Z0-9 ]+$", ErrorMessage = "must contain only English letters and numbers")]
    public string TitleEn { get; set; }

    [LocalizedRequired("Required")]
    [Column(TypeName = "date")]
    public DateOnly? StartDate { get; set; }
    [LocalizedRequired("Required")]
    [Column(TypeName = "date")]
    public DateOnly? EndDate { get; set; }
    [LocalizedRequired("Required")]
    [LocalizedMaxLength(200, "MaxLength_200")]
    public string Location { get; set; }
    
    [LocalizedMaxLength(50, "MaxLength_50")]
    public string? Time { get; set; }
    
    [LocalizedRequired("Required")]
    [LocalizedMaxLength(500, "MaxLength_500")]
    public string Description { get; set; }

    [LocalizedMaxLength(300, "MaxLength_300")]
    public string? AttachmentPath { get; set; }
    public IFormFile? Attachment { get; set; }

    // Temporary uploaded file
    public string? Attachment_TempFilePath { get; set; }

    // For Edit: store old file from DB
    public string? Attachment_OldPath { get; set; }

    public bool IsSubscribed { get; set; }

    #region Check End Date > must be after the Start Date
    // must : IValidatableObject
    // Ex -> public class CourseVM : IValidatableObject
    // add Check End Date > must be after the Start Date
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        // Disallow StartDate older than today
        var minAllowed = DateOnly.FromDateTime(DateTime.Today);
        if (StartDate.HasValue && StartDate.Value < minAllowed)
        {
            if (SessionHelper.GetCurrentLanguage() == "ar")
            {
                yield return new ValidationResult(
                    "لا يمكن أن يكون تاريخ البداية أقدم من اليوم",
                    new[] { nameof(StartDate) }
                );
            }
            else
            {
                yield return new ValidationResult(
                    "The Start Date cannot be earlier than today",
                    new[] { nameof(StartDate) }
                );
            }
        }

        if (StartDate.HasValue && EndDate.HasValue)
        {
            if (EndDate < StartDate)
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





