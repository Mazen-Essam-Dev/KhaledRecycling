namespace Domain.Enums
{
    using System.ComponentModel.DataAnnotations;

    public enum Gender
    {
        [Display(Name = "Male", ResourceType = typeof(Resources.Resource2))]
        Male = 1,

        [Display(Name = "Female", ResourceType = typeof(Resources.Resource2))]
        Female 
    }

}
