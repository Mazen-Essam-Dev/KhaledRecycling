using Domain.Resources;
using System.ComponentModel.DataAnnotations;

public class LocalizedRequiredAttribute : RequiredAttribute
{
    public LocalizedRequiredAttribute(string resourceKey)
    {
        ErrorMessageResourceType = typeof(Resource1);
        ErrorMessageResourceName = resourceKey;
        if (string.IsNullOrEmpty(ErrorMessageResourceName))
        {
            ErrorMessageResourceType = typeof(Resource2);
            ErrorMessageResourceName = resourceKey;
        }
    }

}
