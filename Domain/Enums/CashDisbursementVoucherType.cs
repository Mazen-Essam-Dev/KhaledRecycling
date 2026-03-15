using System.ComponentModel.DataAnnotations;

namespace Domain.Enums
{
    public enum CashDisbursementVoucherType
    {
        [Display(Name = "Cash", ResourceType = typeof(Resources.Resource2))]
        Cash = 1,
        [Display(Name = "Checks", ResourceType = typeof(Resources.Resource2))]
        Checks,
        [Display(Name = "Transfer", ResourceType = typeof(Resources.Resource2))]
        Transfer
    }
}
