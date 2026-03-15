using Domain.Resources;
using System.ComponentModel.DataAnnotations;

public class LocalizedMinLengthAttribute : MinLengthAttribute
{
    public LocalizedMinLengthAttribute(int length, string resourceKey) : base(length)
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
