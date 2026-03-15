using System.ComponentModel.DataAnnotations;

namespace Domain.Enums
{
    public enum TypeTableEnum
    {

        [Display(Name = "مصروفات")]
        Expenses = 1,

        [Display(Name = "مقبوضات")]
        Receipts
    }

}
