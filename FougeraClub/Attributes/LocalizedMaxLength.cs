using Domain.Resources;
using System.ComponentModel.DataAnnotations;

public class LocalizedMaxLengthAttribute : MaxLengthAttribute
{
    public LocalizedMaxLengthAttribute(int length, string resourceKey) : base(length)
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
