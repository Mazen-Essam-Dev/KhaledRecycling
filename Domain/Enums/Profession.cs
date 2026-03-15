using System.ComponentModel.DataAnnotations;

namespace Domain.Enums
{
    public enum Profession
    {
        [Display(Name = "Employee", ResourceType = typeof(Resources.Resource2))]
        Employee = 1,
        [Display(Name = "Student", ResourceType = typeof(Resources.Resource2))]
        Student,
        [Display(Name = "UnEmployed", ResourceType = typeof(Resources.Resource2))]
        UnEmployed,
        [Display(Name = "Other", ResourceType = typeof(Resources.Resource1))]
        Other,
    }
}
