using Domain.Entities;
using FougeraClub.Helpers;
using System.ComponentModel.DataAnnotations;

namespace FougeraClub.Areas.Member.ViewModels
{
    public class ActivityVM : IValidatableObject
    {
        public int Id { get; set; }
        [RegularExpression(@"^[\u0621-\u064A0-9 ]+$", ErrorMessage = "يجب أن يحتوي على حروف عربية وأرقام فقط")]
        [LocalizedRequired("Required"), LocalizedMaxLength(100, "MaxLength_100")]
        public string? TitleAr { get; set; }
        [RegularExpression(@"^[a-zA-Z0-9 ]+$", ErrorMessage = "must contain only English letters and numbers")]
        [LocalizedRequired("Required"), LocalizedMaxLength(100, "MaxLength_100")]
        public string? TitleEn { get; set; }
        [LocalizedRequired("Required")]
        public DateOnly? StartDate { get; set; }
        [LocalizedRequired("Required")]
        public DateOnly? EndDate { get; set; }
        [LocalizedRequired("Required")]
        public string? MinimumAge { get; set; }
        [LocalizedRequired("Required"), LocalizedMaxLength(100, "MaxLength_100")]
        public string? Location { get; set; }
        
        public string? Description { get; set; }
        public string? AttachmentPath { get; set; }
        public virtual ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
        public bool Subscribed { get; set; }

        public int? ActivityDuration { get; set; }

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
