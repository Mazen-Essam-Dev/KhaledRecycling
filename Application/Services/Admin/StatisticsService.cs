using Application.Interfaces.Admin;
using Domain.DTOs.Admin;
using Domain.Entities;
using Domain.Enums;
using Domain.Resources;
using Infrastructure.Repositories.InterfacesDB;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Application.Services.Admin
{
    public class StatisticsService : IStatisticsService
    {
        private readonly IMemberService _memberService;
        private readonly IUnitOfWork _unitOfWork;
        private static IHttpContextAccessor? _httpContextAccessor;

        public StatisticsService(IMemberService memberService, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            _memberService = memberService;
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;

        }

        public static string GetCurrentLanguage()
        {
            var lang = _httpContextAccessor?.HttpContext?.Session?.GetString("CurrentCulture");
            return lang ?? "ar";
        }

        public async Task<StatisticsDTO> GetStatisticsAsync()
        {
            var allMembers = await _memberService.GetAllSpesificAsync();
            var allCourses = await _unitOfWork.Courses
            .Table // It must be IQueryable for EF to project in SQL
            .Select(m => new Course
            {
                Id = m.Id,
                DepartmentId = m.DepartmentId,
            })
            .ToListAsync();

            var allDepartments = await _unitOfWork.Departments.GetAllAsync();

            //// Start TotalIn (Activities And Courses) Section
            var allSubscriptions = await _unitOfWork.Subscriptions.GetAllAsync();
            var allSubscriptionActivities = allSubscriptions.Where(x => x.SubscribedInType == SubscriptionType.Activity).DistinctBy(y => y.MemberId);
            var allSubscriptionCourses = allSubscriptions.Where(x => x.SubscribedInType == SubscriptionType.Course).DistinctBy(y => y.MemberId);
            var allCourses_HaveSubscripers = allSubscriptions.Where(x => x.SubscribedInType == SubscriptionType.Course).DistinctBy(y => y.SubscribedInId);
            var allMembersCourses = allMembers.Where(y => allSubscriptionCourses.Any(x => x.MemberId == y.Id)).ToList();
            var allmemersActivity = allMembers.Where(y => allSubscriptionActivities.Any(x => x.MemberId == y.Id)).ToList();
            var TotalInActivities = allSubscriptionActivities?.Count() ?? 0;
            var TotalInCourses = allSubscriptionCourses?.Count() ?? 0;
            //// End Section

            // hadling 0 under division
            var allMembersCount = allMembers?.Count() ?? 0;
            var valid1 = true;
            var valid_Activities = true;
            var valid_Courses = true;
            if (allMembersCount == 0) { allMembersCount = 1; TotalInCourses = 1; TotalInActivities = 1; valid1 = false; }
            if (TotalInCourses == 0) { TotalInCourses = 1; valid_Courses = false; }
            if (TotalInActivities == 0) { TotalInActivities = 1; valid_Activities = false; }

            #region Department Section 
            var CourseDepartmentsList = new List<CourseCategoryStat>();
            var lang = GetCurrentLanguage();
            foreach (var department in allDepartments)
            {
                var CoursesCountbyDepart = allCourses.Where(x => x.DepartmentId == department.Id);
                var CoursesCountbyDepart_Count = CoursesCountbyDepart?.Count() ?? 0;
                var CoursesCountbyDepart_subbed = CoursesCountbyDepart?.Where(x => allCourses_HaveSubscripers.Any(y => y.SubscribedInId == x.Id)).ToList();
                var CoursesCountbyDepart_subbed_Count = CoursesCountbyDepart_subbed?.Count() ?? 0;
                float PercentageCoursebyDepart = (CoursesCountbyDepart_Count == 0 || CoursesCountbyDepart_subbed_Count == 0)
                    ? 0.0f
                    : (float)Math.Round((float)CoursesCountbyDepart_subbed_Count / CoursesCountbyDepart_Count * 100, 1);

                CourseDepartmentsList.Add(new CourseCategoryStat
                {
                    Label = lang == "ar" ? department.NameAr : department.NameEn,
                    Count = CoursesCountbyDepart_Count, // Total number of courses within the department
                    Percentage = PercentageCoursebyDepart // Percentage of courses in which people participated / All courses within the same department
                });
            }
            #endregion

            //// Start Age In allmemersActivity Section
            var More20 = allmemersActivity?.Where(x => x.Age >= 20)?.Count() ?? 0;
            var from16_19 = allmemersActivity?.Where(x => x.Age >= 16 && x.Age <= 19)?.Count() ?? 0;
            var from13_15 = allmemersActivity?.Where(x => x.Age >= 13 && x.Age <= 15)?.Count() ?? 0;
            var from9_12 = allmemersActivity?.Where(x => x.Age >= 9 && x.Age <= 12)?.Count() ?? 0;

            float More20Percentage = (float)Math.Round((float)More20 / TotalInActivities * 100, 1);
            float from16_19Percentage = (float)Math.Round((float)from16_19 / TotalInActivities * 100, 1);
            float from13_15Percentage = (float)Math.Round((float)from13_15 / TotalInActivities * 100, 1);
            float from9_12Percentage = (float)Math.Round((float)from9_12 / TotalInActivities * 100, 1);

            //// Start Gender In allMembersCourses Section
            var MalesCount = allMembersCourses?.Where(x => x.GenderId == (int)Gender.Male)?.Count() ?? 0;
            var FemalesCount = allMembersCourses?.Where(x => x.GenderId == (int)Gender.Female)?.Count() ?? 0;
            float malePercentage = (float)Math.Round((float)MalesCount / TotalInCourses * 100, 1);
            float FemalePercentage = (float)Math.Round((float)FemalesCount / TotalInCourses * 100, 1);

            //// Start ResidentsAndCitizen In allMembersCourses Section
            var CitizensCount = allMembersCourses?.Where(x => x.NationalityId == 1)?.Count() ?? 0;
            var ResidentsCount = allMembersCourses?.Where(x => x.NationalityId != 1)?.Count() ?? 0;
            float CitizenPercentage = (float)Math.Round((float)CitizensCount / TotalInCourses * 100, 1);
            float ResidentPercentage = (float)Math.Round((float)ResidentsCount / TotalInCourses * 100, 1);

            if (!valid1) { allMembersCount = 0; TotalInActivities = 0; TotalInCourses = 0; }
            if (!valid_Activities) { TotalInActivities = 0; }
            if (!valid_Courses) { TotalInCourses = 0; }

            return new StatisticsDTO
            {
                TotalInActivities = TotalInActivities,
                TotalInCourses = TotalInCourses,

                AgeGroupStats = new List<AgeGroupStat>
                {
                    new AgeGroupStat { Label = Resource1.From20yearsAndMore, Percentage = More20Percentage, Count = More20 },
                    new AgeGroupStat { Label = "19 : 16", Percentage = from16_19Percentage, Count = from16_19 },
                    new AgeGroupStat { Label = "15 : 13", Percentage = from13_15Percentage, Count = from13_15 },
                    new AgeGroupStat { Label = "12 : 9", Percentage = from9_12Percentage, Count = from9_12 }
                },

                CourseDepartmentCategoryStats = CourseDepartmentsList,

                GenderStats = new List<GenderStat>
                {
                    new GenderStat { Label = Resource1.males, Percentage = malePercentage, Count = MalesCount },
                    new GenderStat { Label = Resource1.Females, Percentage = FemalePercentage, Count = FemalesCount }
                },

                ResidentsAndCitizensStats = new List<ResidentsAndCitizensStat>
                {
                    new ResidentsAndCitizensStat { Label = Resource1.residents, Percentage = ResidentPercentage, Count = ResidentsCount },
                    new ResidentsAndCitizensStat { Label = Resource1.citizens, Percentage = CitizenPercentage, Count = CitizensCount }
                }
            };
        }
    }
}
