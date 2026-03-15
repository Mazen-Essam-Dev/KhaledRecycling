using Domain.Entities;
using FougeraClub.Areas.Admin.ViewModels.Member;
using FougeraClub.Helpers;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace FougeraClub.Areas.Admin.ViewModels.Activity
{
    public class SubscribedMemberInActivitiesVM
    {

        public ActivityVM? Activity = new ActivityVM();
        public PaginatedList<MemberVM>? Members_Paginated { get; set; }

        public IEnumerable<Subscription>? Subscriptions = new List<Subscription>();
    }
}
