namespace Domain.Enums
{
    using System.ComponentModel.DataAnnotations;

    public enum DocumentType
    {
        [Display(Name = "Outgoing", ResourceType = typeof(Resources.Resource1))]
        Outgoing = 1,
        [Display(Name = "Incoming", ResourceType = typeof(Resources.Resource1))]
        Incoming = 2,


    }

}
