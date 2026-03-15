using System.ComponentModel.DataAnnotations;

namespace Domain.Enums
{
    public enum MissionType
    {
        [Display(Name = "Forum", ResourceType = typeof(Resources.Resource1))]
        Forum = 1,
        [Display(Name = "Conference", ResourceType = typeof(Resources.Resource1))]
        Conference,
        [Display(Name = "Exhibition", ResourceType = typeof(Resources.Resource1))]
        Exhibition,
        [Display(Name = "Competition", ResourceType = typeof(Resources.Resource1))]
        Competition,
        [Display(Name = "ClubEquipment", ResourceType = typeof(Resources.Resource1))]
        ClubEquipment,
        [Display(Name = "ExchangeOrDelivery", ResourceType = typeof(Resources.Resource1))]
        ExchangeOrDelivery,
    }
}
