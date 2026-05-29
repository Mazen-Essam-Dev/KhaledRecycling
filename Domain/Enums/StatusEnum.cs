using System.ComponentModel.DataAnnotations;

namespace Domain.Enums
{
    public enum StatusEnum
    {
        [Display(Name = "Pending", ResourceType = typeof(Resources.Resource2))]
        Pending,
        [Display(Name = "DoneStatus", ResourceType = typeof(Resources.Resource2))]
        DoneStatus,
        [Display(Name = "Refund", ResourceType = typeof(Resources.Resource2))]
        Refund
    }
}
