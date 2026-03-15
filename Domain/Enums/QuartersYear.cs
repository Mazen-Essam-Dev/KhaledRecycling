namespace Domain.Enums
{
    using System.ComponentModel.DataAnnotations;

    public enum QuartersYear
    {
        [Display(Name = "Quareter1", ResourceType = typeof(Resources.Resource1))]
        Quareter1 = 1,
        [Display(Name = "Quareter2", ResourceType = typeof(Resources.Resource1))]
        Quareter2 = 2,
        [Display(Name = "Quareter3", ResourceType = typeof(Resources.Resource1))]
        Quareter3 = 3,
        [Display(Name = "Quareter4", ResourceType = typeof(Resources.Resource1))]
        Quareter4 = 4,
        [Display(Name = "yearFull", ResourceType = typeof(Resources.Resource1))]
        yearFull = 5,

    }

}
