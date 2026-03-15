using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums
{
    public enum ParticipatingType
    {
        [Display(Name = "meeting", ResourceType = typeof(Resources.Resource2))]
        meeting = 1,

        [Display(Name = "gallery", ResourceType = typeof(Resources.Resource2))]
        gallery,

        [Display(Name = "Competition", ResourceType = typeof(Resources.Resource2))]
        Competition
    }
}
