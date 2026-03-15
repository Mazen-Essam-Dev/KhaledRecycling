using Domain.Resources;
using System.ComponentModel.DataAnnotations;

namespace FougeraClub.Attributes;

public class NotInThePastAttribute : ValidationAttribute
{
    public NotInThePastAttribute()
    {
        ErrorMessageResourceType = typeof(Resource2);
        ErrorMessageResourceName = "DateInThePast"; // Add key in resource file
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        // Try to detect if this is an edit (Id exists and > 0)
        var idProperty = validationContext.ObjectType.GetProperty("Id");
        if (idProperty != null)
        {
            var idValue = idProperty.GetValue(validationContext.ObjectInstance);
            if (idValue is int id && id > 0)
            {
                // Edit mode -> skip validation
                return ValidationResult.Success;
            }
        }

        if (value is DateOnly date && date < DateOnly.FromDateTime(DateTime.Today))
        {
            return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
        }
        return ValidationResult.Success;
    }
}
