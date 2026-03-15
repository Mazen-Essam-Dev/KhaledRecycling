using System.ComponentModel.DataAnnotations;

namespace Domain.Enums
{
    public enum SMSStatus
    {
        [Display(Name = "Success", ResourceType = typeof(Resources.Resource2))]
        Success,
        [Display(Name = "Fail", ResourceType = typeof(Resources.Resource2))]
        Fail
    }
}
