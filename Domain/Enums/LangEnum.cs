using System.ComponentModel.DataAnnotations;

namespace Domain.Enums
{
    public enum LangEnum
    {

        [Display(Name = "Ar", ResourceType = typeof(Resources.Resource1))] // Ar
        Ar = 1,

        [Display(Name = "En", ResourceType = typeof(Resources.Resource1))] // Ar
        En
    }

}
