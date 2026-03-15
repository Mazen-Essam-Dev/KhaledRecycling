using Domain.Entities;
using FougeraClub.Areas.Admin.ViewModels.Member;
using FougeraClub.Areas.Admin.ViewModels.Trainers;
using FougeraClub.Helpers;

namespace FougeraClub.Areas.Admin.ViewModels.Course;

public class MembersCourseVM
{
    public CourseVM? Course = new CourseVM();
    public TrainersNameVM? Trainer = new TrainersNameVM();
    public IEnumerable<MemberVM>? Members = new List<MemberVM>();

    public PaginatedList<MemberVM>? Members_Paginated { get; set; }
    public IEnumerable<Subscription>? Subscriptions = new List<Subscription>();

    public bool UserIsTrainer { get; set; }

}





