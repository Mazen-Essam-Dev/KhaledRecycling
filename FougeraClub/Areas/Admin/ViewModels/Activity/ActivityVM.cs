using Domain.Entities;
using Domain.Resources;
using FougeraClub.Attributes;
using FougeraClub.Helpers;
using System.ComponentModel.DataAnnotations;

namespace FougeraClub.Areas.Admin.ViewModels.Activity;


public class ActivityVM : IValidatableObject
{
    public IEnumerable<Question> Questions { get; set; } = new List<Question>();

    public int Id { get; set; }

    [Required(ErrorMessageResourceType = typeof(Resource1), ErrorMessageResourceName = "Required")
               , MaxLength(200, ErrorMessageResourceType = typeof(Resource1), ErrorMessageResourceName = "MaxLength_200")]
    [RegularExpression(@"^[\u0621-\u064A0-9 ]+$", ErrorMessage = "يجب أن يحتوي على حروف عربية وأرقام فقط")]
    public string? TitleAr { get; set; }

    [Required(ErrorMessageResourceType = typeof(Resource1), ErrorMessageResourceName = "Required")
               , MaxLength(200, ErrorMessageResourceType = typeof(Resource1), ErrorMessageResourceName = "MaxLength_200")]
    [RegularExpression(@"^[a-zA-Z0-9 ]+$", ErrorMessage = "must contain only English letters and numbers")]
    public string? TitleEn { get; set; }
    [Required(ErrorMessageResourceType = typeof(Resource1), ErrorMessageResourceName = "Required")]
    public DateOnly? StartDate { get; set; }
    [Required(ErrorMessageResourceType = typeof(Resource1), ErrorMessageResourceName = "Required")]
    public DateOnly? EndDate { get; set; }
    [Required(ErrorMessageResourceType = typeof(Resource1), ErrorMessageResourceName = "Required")]
    public string? MinimumAge { get; set; }

    [Required(ErrorMessageResourceType = typeof(Resource1), ErrorMessageResourceName = "Required")
               , MaxLength(200, ErrorMessageResourceType = typeof(Resource1), ErrorMessageResourceName = "MaxLength_200")]
    public string? Location { get; set; }
    public string? Description { get; set; }
    public string? Achievement { get; set; }


    [MaxLength(300, ErrorMessageResourceType = typeof(Resource1), ErrorMessageResourceName = "MaxLength_300")]
    public string? AttachmentPath { get; set; }
    public IFormFile? Attachment { get; set; }

    // Temporary uploaded file
    public string? Attachment_TempFilePath { get; set; }

    // For Edit: store old file from DB
    public string? Attachment_OldPath { get; set; }

    public bool IsSubscribed { get; set; }
    public int? ActivityDuration { get; set; }
    public int? SubscriptionCount { get; set; }

    #region Check End Date > must be after the Start Date
    // must : IValidatableObject
    // Ex -> public class CourseVM : IValidatableObject
    // add Check End Date > must be after the Start Date
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        // Disallow StartDate earlier than today
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
                        new[] { nameof(EndDate) } //so that the error appears under EndDate
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





