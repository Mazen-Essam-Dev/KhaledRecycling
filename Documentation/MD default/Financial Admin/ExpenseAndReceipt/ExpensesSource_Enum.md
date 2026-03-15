using System.ComponentModel.DataAnnotations;

namespace Domain.Enums
{
    public enum ExpensesSourceEnum
    {
        [Display(Name = "Bank", ResourceType = typeof(Resources.Resource2))]
        Bank = 1,

        [Display(Name = "MiscellaneousExpenses", ResourceType = typeof(Resources.Resource2))]
        MiscellaneousExpenses,
    }
}
