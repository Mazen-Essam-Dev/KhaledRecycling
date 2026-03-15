using System.ComponentModel.DataAnnotations;

namespace Domain.Enums
{
    public enum ExpensesGatesEnum
    {
        [Display(Name = "ChapterOne", ResourceType = typeof(Resources.Resource2))]
        ChapterOne = 1,

        [Display(Name = "ChapterTwo", ResourceType = typeof(Resources.Resource2))]
        ChapterTwo,

        [Display(Name = "ChapterThree", ResourceType = typeof(Resources.Resource2))]
        ChapterThree,
    }

}
