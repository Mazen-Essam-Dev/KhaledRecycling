namespace FougeraClub.Areas.Member.ViewModels
{
    public class HomeActivityCourseVM
    {
        public string? MemberName { get; set; }
        public IEnumerable<CourseVM>? Courses { get; set; }
        public IEnumerable<ActivityVM>? Activities { get; set; }
    }
}
