using Domain.DTOs.Admin.Course;
using Domain.Entities;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces.Admin
{
    public interface ICourseService
    {
        Task<IEnumerable<Course>> GetAllAsync();
        Task<IEnumerable<Course>?> GetAllCoursesOfMemberAsync(int memberId);
        Task<Course?> GetByIdAsync(int id);
        Task<int> AddAsync(Course entity, IFormFile? file);
        Task UpdateAsync(Course entity, IFormFile? file);
        Task DeleteAsync(int id);
        Task DeleteSubscriptionAsync(int id);
        Task<string> SaveImageAsync(IFormFile file);
        void DeleteImageFile(string? relativePath);
        Task<(IEnumerable<MemberEntity>?, IEnumerable<Subscription>?)> GetAllMembersOfCourseAsync(int courseId);
        Task<bool> UpdateAttendance(int courseId, int memberId, bool attendance);
        Task<bool> UpdateAcceptance(AcceptanceDTO dto);
        Task<IEnumerable<SubscribedMemberCourseDTO>> SubscribedMembersInCourses_Trainers(int ThisTrainerId);
        Task<IEnumerable<SubscribedMemberCourseDTO>> SubscribedMembersInCourses_Admin();
        Task<CertificateDTO> GetCertificateData(int subscriptionId);

    }

}
