# CashDisbursementVoucherType Enum Explanation

## Overview
`CashDisbursementVoucherType` is an **enumeration** used to define the **type of payment** in a financial disbursement voucher.  
It is used to control and standardize allowed values (Cash, Checks, Transfer) instead of relying on free text.

---

## Enum Definition
```csharp
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
