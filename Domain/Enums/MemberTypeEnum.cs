using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums
{
    public enum MemberTypeEnum
    {
        [Display(Name = "Member", ResourceType = typeof(Resources.Resource2))]
        Member = 1,

        [Display(Name = "Subscriber", ResourceType = typeof(Resources.Resource2))]
        Subscriber
    }
}
