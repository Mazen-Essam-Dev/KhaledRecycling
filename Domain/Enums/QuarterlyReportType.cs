using System.ComponentModel.DataAnnotations;

namespace Domain.Enums
{
    public enum QuarterlyReportType
    {
        [Display(Name = "ActivitiesType", ResourceType = typeof(Resources.Resource2))]
        Activities = 1,
        [Display(Name = "AdministrativeType", ResourceType = typeof(Resources.Resource2))]
        Administrative,
        [Display(Name = "CollaborativeType", ResourceType = typeof(Resources.Resource2))]
        Collaborative,
    }
}
