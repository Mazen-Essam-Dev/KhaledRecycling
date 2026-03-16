using Domain.Resources;
using System.ComponentModel.DataAnnotations;

namespace KhaledTeamRecycling.Attributes;

public class InThePastAttribute : ValidationAttribute
{
    public InThePastAttribute()
    {
        ErrorMessageResourceType = typeof(Resource2);
        ErrorMessageResourceName = "DateMustBeInThePast"; // Add key in resource file
    }

    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value is DateOnly date && date >= DateOnly.FromDateTime(DateTime.Today))
        {
            return new ValidationResult(FormatErrorMessage(validationContext.DisplayName));
        }
        return ValidationResult.Success;
    }
}
