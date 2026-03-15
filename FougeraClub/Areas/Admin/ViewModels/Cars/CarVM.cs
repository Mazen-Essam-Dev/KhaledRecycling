using Application.Helpers;
using FougeraClub.Helpers;
using Humanizer;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FougeraClub.Areas.Admin.ViewModels.Cars;

public class CarVM : IValidatableObject
{
    public int Id { get; set; }
    [LocalizedRequired("Required"), LocalizedMaxLength(20, "MaxLength_20")]
    public string? PlateNumber { get; set; }
    [LocalizedRequired("Required"), LocalizedMaxLength(100, "MaxLength_100")]
    public string? Type { get; set; }
    [LocalizedRequired("Required"), LocalizedMaxLength(100, "MaxLength_100")]
    public string? DriverName { get; set; }
    [LocalizedRequired("Required")]
    [Column(TypeName = "date")] 
    public DateOnly? OwnershipExpiryDate { get; set; }
    [LocalizedRequired("Required"), LocalizedMaxLength(100, "MaxLength_100")]
    public string? Model { get; set; }
    [LocalizedRequired("Required"), LocalizedMaxLength(100, "MaxLength_100")]
    public string? Color { get; set; }
    public string? Notes { get; set; }
    [MaxLength(255)]
    public string? AttachmentPath { get; set; }
    public IFormFile? Attachment { get; set; }

    // Temporary uploaded file
    public string? Attachment_TempFilePath { get; set; }

    // For Edit: store old file from DB
    public string? Attachment_OldPath { get; set; }

    public bool? inService;

    #region Check Date Expired Date
    // must : IValidatableObject
    // Ex -> public class CourseVM : IValidatableObject
    // add Check Date Expired
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var Datenow = AppDubaiTime.Now.AtMidnight(); // Get TodayDate --> AtMidnight()
        if (OwnershipExpiryDate.HasValue)
        {
            if (OwnershipExpiryDate?.ToDateTime(TimeOnly.MinValue) /*to Convert DateOnly to DateTime --> AtMidnight() */ <= Datenow)
            {
                if (SessionHelper.GetCurrentLanguage() == "ar")
                {
                    yield return new ValidationResult(
                        "عفواً التاريخ منتهي",
                        new[] { nameof(OwnershipExpiryDate) }  //so that the error appears under ExpiryDate
                    );
                }
                else
                {
                    yield return new ValidationResult(
                        "Sorry, the Owner ship date has expired",
                        new[] { nameof(OwnershipExpiryDate) }  //so that the error appears under ExpiryDate
                    );
                }

            }
        }        
    }
    #endregion
}




