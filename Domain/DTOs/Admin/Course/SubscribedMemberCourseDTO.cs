using Domain.Entities;

namespace Domain.DTOs.Admin.Course
{
    public class SubscribedMemberCourseDTO
    {
        public string? MemberFullNameAr { get; set; }
        public string? MemberFullNameEn { get; set; }
        public int? MemberId { get; set; }
        public int? MemberCode { get; set; }

        public string? MemberIdNumber { get; set; }
        public int? MemberGenderId { get; set; }
        public int? NationalityId { get; set; }
        public string? NationalityNameAr { get; set; }
        public string? NationalityNameEn { get; set; }
        public int? DepartmentId { get; set; }
        public int? CourseID { get; set; }
        public string? DepartmentNameAr { get; set; }
        public string? DepartmentNameEn { get; set; }
        public string? CourseTitleAr { get; set; }
        public string? CourseTitleEn { get; set; }
        public DateTime? SubscriptionDate { get; set; }
        public DateOnly? CourseStartDate { get; set; }
        public DateOnly? CourseEndDate { get; set; }
        public string? TrainerFullNameAr { get; set; }
        public string? TrainerFullNameEn { get; set; }
        public int SubscriptionId { get; set; }
        public bool? Acceptance { get; set; }
        public bool? Attendance { get; set; }
        public int? SelectedRate { get; set; }

        public string? SubNotes { get; set; }
        public bool isCoursehasAcceptedOrRejectedMember { get; set; }

        public string? MemberTypeAr { get; set; }
        public string? MemberTypeEn { get; set; }

        public int? MemberTypeId { get; set; }
        public MemberType? MemberType { get; set; }

    }
}
