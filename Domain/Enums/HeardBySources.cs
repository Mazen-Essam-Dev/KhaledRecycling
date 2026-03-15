using System.ComponentModel.DataAnnotations;

namespace Domain.Enums
{
    public enum HeardBySources
    {
        [Display(Name = "ByFriend", ResourceType = typeof(Resources.Resource2))]
        ByFriend = 1,
        [Display(Name = "ByInternet", ResourceType = typeof(Resources.Resource2))]
        ByInternet,
        [Display(Name = "ByAdvertisement", ResourceType = typeof(Resources.Resource2))]
        ByAdvertisement

    }
}
