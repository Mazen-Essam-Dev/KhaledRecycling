using Domain.Resources;
using System.ComponentModel.DataAnnotations;

public class IntAttribute : RangeAttribute
{
    public IntAttribute(string resourceKey) : base(0 , int.MaxValue)
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
