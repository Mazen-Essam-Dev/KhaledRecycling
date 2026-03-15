using Domain.Entities;
using FougeraClub.Helpers;
using System.ComponentModel.DataAnnotations;

namespace FougeraClub.Areas.Member.ViewModels
{
    public class CourseVM : IValidatableObject
    {
        public int? isHasCode { get; set; }
        public bool? isHasIDCard { get; set; }
        public bool? isHasPassport { get; set; }
        public bool? isNotExpired { get; set; }
        public int? DepartmentId { get; set; }

        public int Id { get; set; }

        public string? RejectionNotes { get; set; }
        public string? TrainerId { get; set; }

        public string? Title { get; set; }
        [RegularExpression(@"^[\u0621-\u064A0-9 ]+$", ErrorMessage = "يجب أن يحتوي على حروف عربية وأرقام فقط")]
        public string? TitleAr { get; set; }
        [RegularExpression(@"^[a-zA-Z0-9 ]+$", ErrorMessage = "must contain only English letters and numbers")]
        public string? TitleEn { get; set; }

        public DateOnly? StartDate { get; set; }

        public DateOnly? EndDate { get; set; }

        public string? Location { get; set; }

        public string? Time { get; set; }

        public string? Description { get; set; }

        public string? AttachmentPath { get; set; }
        public Department? Department { get; set; }
        public int? selectedRate { get; set; }       
        public IEnumerable<CourseVM>? all_CoursesListVM { get; set; }
        public IEnumerable<Subscription>? all_SubscriptionsList { get; set; }
        public bool? Attendance { get; set; }
        public bool? Subscribed { get; set; }
        public bool? Accepted { get; set; }
        public int? SubscriptionId { get; set; }
        public string? Notes { get; set; }

        [MaxLength(300)]
        public string? IdImagePath { get; set; }
        public IFormFile? IdImage { get; set; }

        [MaxLength(300)]
        public string? PassportImagePath { get; set; }
        public IFormFile? PassportImage { get; set; }

        // Temporary uploaded file
        public string? IdImage_TempFilePath { get; set; }

        // For Edit: store old file from DB
        public string? IdImage_Old { get; set; }

        // Temporary uploaded file
        public string? PassportImage_TempFilePath { get; set; }

        // For Edit: store old file from DB
        public string? PassportImage_Old { get; set; }


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
