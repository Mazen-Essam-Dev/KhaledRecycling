using Domain.Entities;
using FougeraClub.Areas.Admin.ViewModels.Member;

namespace FougeraClub.Areas.Admin.ViewModels.Activity;

public class MembersActivityVM
{
    public ActivityVM? Activity = new ActivityVM();
    public IEnumerable<MemberVM>? Members = new List<MemberVM>();
    public IEnumerable<Subscription>? Subscriptions = new List<Subscription>();
}





