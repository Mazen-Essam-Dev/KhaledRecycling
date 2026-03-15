using Domain.Enums;
using Domain.HelperForDomain;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

public class Subscription
{
    public int Id { get; set; }

    public int MemberId { get; set; }
    public virtual MemberEntity Member { get; set; } = null!;

    public int SubscribedInId { get; set; }

    public int? Rate { get; set; }
    public bool? Attendance { get; set; }


    public SubscriptionType SubscribedInType { get; set; }

    public DateTime? ParticipationDate { get; set; } = AppDubaiTime1.Now;

    // Not mapped by EF – handled manually in code
    [NotMapped]
    public Course? Course { get; set; }

    [NotMapped]
    public Activity? Activity { get; set; }
    public bool? Acceptance { get; set; }

    public string? CertificateSerial { get; set; }

    public string? Notes { get; set; }

}