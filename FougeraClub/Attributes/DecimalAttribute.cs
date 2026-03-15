using Domain.Resources;
using System.ComponentModel.DataAnnotations;

public class DecimalAttribute : RangeAttribute
{
    public DecimalAttribute(string resourceKey) : base(0.00 , double.MaxValue)
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
