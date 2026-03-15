using System.ComponentModel.DataAnnotations;

namespace Domain.Enums
{
    public enum ItemType
    {

        [Display(Name = "مصروفات")]
        Expenses = 1,

        [Display(Name = "مقبوضات")]
        Receipts
    }

}
