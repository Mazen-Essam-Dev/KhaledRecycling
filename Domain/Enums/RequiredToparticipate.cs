using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums
{
    public enum RequiredToparticipate
    {
        [Display(Name = "Participants1", ResourceType = typeof(Resources.Resource2))]
        Participants1 = 1,

        [Display(Name = "projects", ResourceType = typeof(Resources.Resource2))]
        projects,

    }
}
