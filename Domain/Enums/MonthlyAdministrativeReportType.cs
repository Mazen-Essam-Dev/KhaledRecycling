using System.ComponentModel.DataAnnotations;

namespace Domain.Enums
{
    public enum MonthlyAdministrativeReportType
    {
        [Display(Name = "ActivitiesType", ResourceType = typeof(Resources.Resource2))]
        Activities = 1,
        [Display(Name = "AdministrativeType", ResourceType = typeof(Resources.Resource2))]
        Administrative
    }
}
