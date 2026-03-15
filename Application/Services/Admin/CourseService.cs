using Application.Interfaces.Admin;
using DocumentFormat.OpenXml.Presentation;
using Domain.DTOs.Admin;
using Domain.DTOs.Admin.Course;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Identity;
using Infrastructure.Repositories.InterfacesDB;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Admin
{
    public class CourseService : ICourseService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _env;

        public CourseService(IUnitOfWork unitOfWork, IWebHostEnvironment env)
        {
            _unitOfWork = unitOfWork;
            _env = env;
        }

        public async Task<IEnumerable<Course>> GetAllAsync()
        {
            return await _unitOfWork.Courses.GetAllAsync(t => t.Trainer, t => t.Department);
        }

        public async Task<IEnumerable<Course>?> GetAllCoursesOfMemberAsync(int memberId)
        {
            var allCourses = await _unitOfWork.Courses.GetAllAsync(c => c.Department);
            var memberSubscriptions = await _unitOfWork.Subscriptions
                .GetAllAsync(s => s.MemberId == memberId && s.SubscribedInType == SubscriptionType.Course);

            var memberCourses = allCourses
                .Where(course => memberSubscriptions.Any(s => s.SubscribedInId == course.Id));

            return memberCourses;
        }

        public async Task<Course?> GetByIdAsync(int id)
        {
            return await _unitOfWork.Courses
                .GetByIdAsync(e => e.Id == id);
        }

        public async Task<int> AddAsync(Course entity, IFormFile? file)
        {
            if (file != null)
            {
                entity.AttachmentPath = await SaveImageAsync(file);
            }

            var entit = await _unitOfWork.Courses.AddAsync(entity);
            await _unitOfWork.CompleteAsync();
            return entit.Id;
        }

        public async Task UpdateAsync(Course entity, IFormFile? file)
        {
            var existing = await _unitOfWork.Courses.GetByIdAsync(entity.Id);
            if (existing == null) return;

            //if (file != null)
            //{
            //    // Delete the old image
            //    DeleteImageFile(existing.AttachmentPath);

            //    // Save new image
            //    entity.AttachmentPath = await SaveImageAsync(file);
            //}
            //else
            //{
            //    // Keep the old image if none uploaded
            //    entity.AttachmentPath = existing.AttachmentPath;
            //}

            _unitOfWork.Courses.UpdateValues(existing, entity);
            await _unitOfWork.CompleteAsync();
        }

        //public async Task DeleteAsync(int id)
        //{
        //    var entity = await _unitOfWork.Courses.GetByIdAsync(id);
        //    if (entity != null)
        //    {
        //        // Delete image from disk
        //        DeleteImageFile(entity.AttachmentPath);

        //        _unitOfWork.Courses.Delete(entity);
        //        await _unitOfWork.CompleteAsync();
        //    }
        //}

        public async Task DeleteAsync(int id)
        {
            var allHisSubs = await _unitOfWork.Subscriptions.GetAllAsync(s => s.SubscribedInType == SubscriptionType.Course && s.SubscribedInId == id);

            if (allHisSubs != null && !allHisSubs.Any(x => x.Acceptance == true)) // not has Accepetance for any one
            {
                var entity = await _unitOfWork.Courses.GetByIdAsync(id);
                if (entity != null)
                {
                    if (allHisSubs != null && allHisSubs.Count() > 0)
                    {
                        _unitOfWork.Subscriptions.RemoveRange(allHisSubs);
                    }

                    // Delete image from disk
                    DeleteImageFile(entity.AttachmentPath);

                    _unitOfWork.Courses.Delete(entity);
                    await _unitOfWork.CompleteAsync();
                }
            }
        }

        public async Task DeleteSubscriptionAsync(int id)
        {
            var entity = await _unitOfWork.Subscriptions.GetByIdAsync(id);
            if (entity != null)
            {
                _unitOfWork.Subscriptions.Delete(entity);
                await _unitOfWork.CompleteAsync();
            }
        }

        public async Task<string> SaveImageAsync(IFormFile file)
        {
            var folder = Path.Combine(_env.WebRootPath, "uploads/Courses");
            Directory.CreateDirectory(folder);
            var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            var fullPath = Path.Combine(folder, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"uploads/Courses/{fileName}";
        }
        public void DeleteImageFile(string? relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath)) return;

            var fullPath = Path.Combine(_env.WebRootPath, relativePath.Replace("/", Path.DirectorySeparatorChar.ToString()));
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }
        public async Task<(IEnumerable<MemberEntity>?, IEnumerable<Subscription>?)> GetAllMembersOfCourseAsync(int courseId)
        {
            var allMembers = await _unitOfWork.Members.GetAllAsync(x => x.Nationality);
            var membersSubscriptionsCourse = await _unitOfWork.Subscriptions
                .GetAllAsync(s => s.SubscribedInId == courseId && s.SubscribedInType == SubscriptionType.Course && s.Acceptance == true);

            var courseMembers = allMembers
                .Where(member => membersSubscriptionsCourse.Any(s => s.MemberId == member.Id && s.Acceptance == true));

            return (courseMembers, membersSubscriptionsCourse);
        }
        private async Task<string> GetLastCertificateSerial()
        {
            var lastCertificateSerial = await _unitOfWork.Subscriptions.Table
               .Where(s => s.SubscribedInType == SubscriptionType.Course)
               .Select(x => x.CertificateSerial)
               .Where(x => !string.IsNullOrEmpty(x))
               .Distinct()
               .OrderByDescending(x => x)
               .FirstOrDefaultAsync();

            int newCertificateSerialInt;

            if (!int.TryParse(lastCertificateSerial, out newCertificateSerialInt))
            {
                newCertificateSerialInt = 0; // null or any string
            }

            newCertificateSerialInt++;

            var newCertificateSerialStr = newCertificateSerialInt.ToString("000");
            return newCertificateSerialStr;
        }
        public async Task<bool> UpdateAttendance(int courseId, int memberId, bool attendance)
        {
            var subscription = await _unitOfWork.Subscriptions.GetByColumnAsync(s =>
                s.SubscribedInId == courseId &&
                s.SubscribedInType == SubscriptionType.Course &&
                s.MemberId == memberId);

            var newCertificateSerialStr = await GetLastCertificateSerial();

            if (subscription != null)
            {
                if(attendance==true && subscription.CertificateSerial==null)
                    subscription.CertificateSerial = newCertificateSerialStr;

                subscription.Attendance = attendance;
                await _unitOfWork.CompleteAsync(); // <-- Save changes
                return true;
            }

            return false; // Subscription not found
        }
        public async Task<bool> UpdateAcceptance(AcceptanceDTO dto)
        {
            var subscription = await _unitOfWork.Subscriptions.GetByIdAsync(dto.SubscriptionId);
            if (subscription == null) return false;

            subscription.Acceptance = dto.State;
            subscription.Notes = dto.Notes;

            await _unitOfWork.CompleteAsync();
            return true;
        }

        public async Task<IEnumerable<SubscribedMemberCourseDTO>> SubscribedMembersInCourses_Trainers(int ThisTrainerId)
        {
            var thisTrainer = await _unitOfWork.Trainers.GetByIdAsync(ThisTrainerId);
            var members = await _unitOfWork.Members.GetAllAsync(m => m.Nationality,b=>b.MemberType);
            var subscriptions = await _unitOfWork.Subscriptions.GetAllAsync(
                s => s.SubscribedInType == SubscriptionType.Course
            );

            var subsHas = subscriptions.Where(s => s.Acceptance.HasValue).DistinctBy(x => x.SubscribedInId).Select(x => x.SubscribedInId); // Not Null Accepted Or Rejected

            if (members == null || subscriptions == null || thisTrainer == null || members?.Count() == 0 || subscriptions?.Count() == 0)
            {
                return Enumerable.Empty<SubscribedMemberCourseDTO>();
            }

            var allCoursesFortisDepartment = await _unitOfWork.Courses
                .GetAllAsync(c => c.DepartmentId == thisTrainer.DepartmentId,
                             c => c.Department, c => c.Trainer);

            var courses = allCoursesFortisDepartment
                .Where(c => subscriptions.Any(s => s.SubscribedInId == c.Id) && c.DepartmentId == thisTrainer.DepartmentId)
                .ToList();

            if (courses == null || courses?.Count == 0)
            {
                return Enumerable.Empty<SubscribedMemberCourseDTO>();
            }
            var users = await _unitOfWork.Users.Table.Select(user => new UserSmallDTO
            {
                Id = user.Id,
                FullNameAr = user.FullNameAr,
                FullNameEn = user.FullNameEn
            }).ToListAsync();

            var result = (from sub in subscriptions
                          let course = courses.FirstOrDefault(c => c.Id == sub.SubscribedInId)
                          let member = members.FirstOrDefault(m => m.Id == sub.MemberId)
                          let trainerUser = users.FirstOrDefault(u => u.Id == course?.Trainer?.UserId)
                          select new SubscribedMemberCourseDTO
                          {
                              MemberIdNumber = member?.IdNumber,
                              MemberId = member?.Id,
                              MemberTypeId = member?.MemberTypeId,
                              MemberCode = member?.Code,
                              MemberFullNameAr = member?.FullNameAr,
                              MemberFullNameEn = member?.FullNameEn,
                              MemberGenderId = member?.GenderId,
                              NationalityId = member?.NationalityId,
                              NationalityNameAr = member?.Nationality?.NameAr,
                              NationalityNameEn = member?.Nationality?.NameEn,
                              MemberTypeAr = member?.MemberType?.NameAr,
                              MemberTypeEn = member?.MemberType?.NameEn,
                              DepartmentId = course?.DepartmentId,
                              DepartmentNameAr = course?.Department?.NameAr,
                              DepartmentNameEn = course?.Department?.NameEn,
                              CourseTitleAr = course?.TitleAr,
                              CourseTitleEn = course?.TitleEn,
                              CourseID = course?.Id,
                              SubscriptionDate = sub?.ParticipationDate,
                              CourseStartDate = course?.StartDate,
                              CourseEndDate = course?.EndDate,
                              TrainerFullNameAr = trainerUser?.FullNameAr,
                              TrainerFullNameEn = trainerUser?.FullNameEn,
                              SubscriptionId = sub.Id,
                              Acceptance = sub?.Acceptance,
                              Attendance = sub?.Attendance,
                              SelectedRate = sub?.Rate,
                              SubNotes = sub?.Notes,
                              isCoursehasAcceptedOrRejectedMember = (subsHas != null && subsHas.Any(x => x == course?.Id)) ? true : false,
                          }).ToList();

            result = result.Where(dto => dto.DepartmentId != null && (dto.TrainerFullNameAr != null || dto.TrainerFullNameEn != null) ).ToList();

            return result;
        }

        public async Task<IEnumerable<SubscribedMemberCourseDTO>> SubscribedMembersInCourses_Admin()
        {
            var members = await _unitOfWork.Members.GetAllAsync(m => m.Nationality,b=>b.MemberType);
            var subscriptions = await _unitOfWork.Subscriptions.GetAllAsync(
                s => s.SubscribedInType == SubscriptionType.Course
            );

            var subsHas = subscriptions.Where(s => s.Acceptance.HasValue).DistinctBy(x => x.SubscribedInId).Select(x => x.SubscribedInId); // Not Null Accepted Or Rejected

            var courses = await _unitOfWork.Courses.GetAllAsync(c => c.Department, c => c.Trainer);

            var users = await _unitOfWork.Users.Table.Select(user => new UserSmallDTO
            {
                Id = user.Id,
                FullNameAr = user.FullNameAr,
                FullNameEn = user.FullNameEn
            }).ToListAsync();

            var result = (from sub in subscriptions
                          let course = courses.FirstOrDefault(c => c.Id == sub.SubscribedInId)
                          let member = members.FirstOrDefault(m => m.Id == sub.MemberId)
                          let trainerUser = users.FirstOrDefault(u => u.Id == course?.Trainer?.UserId)
                          select new SubscribedMemberCourseDTO
                          {
                              MemberIdNumber = member?.IdNumber,
                              MemberId = member?.Id,
                              MemberTypeId = member?.MemberTypeId,
                              MemberCode = member?.Code,
                              MemberFullNameAr = member?.FullNameAr,
                              MemberFullNameEn = member?.FullNameEn,
                              MemberGenderId = member?.GenderId,
                              NationalityId = member?.NationalityId,
                              NationalityNameAr = member?.Nationality?.NameAr,
                              NationalityNameEn = member?.Nationality?.NameEn,
                              MemberTypeAr = member?.MemberType?.NameAr,
                              MemberTypeEn = member?.MemberType?.NameEn,
                              DepartmentId = course?.DepartmentId,
                              DepartmentNameAr = course?.Department?.NameAr,
                              DepartmentNameEn = course?.Department?.NameEn,
                              CourseTitleAr = course?.TitleAr,
                              CourseTitleEn = course?.TitleEn,
                              CourseID = course?.Id,
                              SubscriptionDate = sub?.ParticipationDate,
                              CourseStartDate = course?.StartDate,
                              CourseEndDate = course?.EndDate,
                              TrainerFullNameAr = trainerUser?.FullNameAr,
                              TrainerFullNameEn = trainerUser?.FullNameEn,
                              SubscriptionId = sub.Id,
                              Acceptance = sub?.Acceptance,
                              Attendance = sub?.Attendance,
                              SelectedRate = sub?.Rate,
                              SubNotes = sub?.Notes,
                              isCoursehasAcceptedOrRejectedMember = (subsHas!=null && subsHas.Any(x=> x == course?.Id)) ? true : false,
                          }).ToList();

            return result;
        }

        public async Task<CertificateDTO> GetCertificateData(int subscriptionId)
        {
            var subscription = await _unitOfWork.Subscriptions.GetByIdAsync(s => s.Id == subscriptionId);
            var course = await _unitOfWork.Courses.GetByIdAsync(s => s.Id == subscription.SubscribedInId);
            var member = await _unitOfWork.Members.GetByIdAsync(subscription.MemberId);

            var dto = new CertificateDTO
            {
                MemberNameAr = member?.FullNameAr,
                MemberNameEn = member?.FullNameEn,
                GenderId = member?.GenderId,
                CourseTitleAr = course?.TitleAr,
                CourseTitleEn = course?.TitleEn,
                StartDate = course?.StartDate,
                EndDate = course?.EndDate,
                CertificateSerial = subscription?.CertificateSerial
            };
            return dto;
        }

    }

}
