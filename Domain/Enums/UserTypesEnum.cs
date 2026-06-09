using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums
{
    public enum UserTypesEnum
    {
        [Display(Name = "Individual", ResourceType = typeof(Resources.Resource1))]
        Individual = 1,

        [Display(Name = "corporation", ResourceType = typeof(Resources.Resource1))]
        corporation,
        [Display(Name = "Factory", ResourceType = typeof(Resources.Resource1))]
        Factory,
        [Display(Name = "corporationNotClient", ResourceType = typeof(Resources.Resource1))]
        corporationNotClient,
    }
}
