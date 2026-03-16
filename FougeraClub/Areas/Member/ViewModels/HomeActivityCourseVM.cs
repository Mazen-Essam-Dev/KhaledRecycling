using KhaledTeamRecycling.Areas.Admin.ViewModels.Activity;
using KhaledTeamRecycling.Areas.Admin.ViewModels.Course;

namespace KhaledTeamRecycling.Areas.Member.ViewModels
{
    public class HomeActivityCourseVM
    {
        public string? MemberName { get; set; }
        public IEnumerable<CourseVM>? Courses { get; set; }
        public IEnumerable<ActivityVM>? Activities { get; set; }
    }
}
