using System.ComponentModel.DataAnnotations;

namespace Domain.Enums
{
    public enum StatusEnum
    {
        [Display(Name = "PendingStatus", ResourceType = typeof(Resources.Resource2))]
        PendingStatus,
        [Display(Name = "DoneWillDeleviryStatus", ResourceType = typeof(Resources.Resource2))]
        DoneWillDeleviryStatus,
        [Display(Name = "Canceled", ResourceType = typeof(Resources.Resource2))]
        Canceled,
        [Display(Name = "SuccessDoneStatus", ResourceType = typeof(Resources.Resource2))]
        Success
    }
}
