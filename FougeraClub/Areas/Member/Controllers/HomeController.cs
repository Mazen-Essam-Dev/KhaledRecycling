using Application.Helpers;
using Application.Interfaces.Member;
using AutoMapper;
using Domain.HelperForDomain;
using Domain.Resources;
using FougeraClub.Areas.Member.ViewModels;
using FougeraClub.Attributes;
using FougeraClub.Helpers;
using FougeraClub.Middelware;
using Infrastructure.Attributes;
using Infrastructure.Repositories.InterfacesDB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FougeraClub.Areas.Member.Controllers
{
    [Area("Member")]
    [Route("Member/[controller]/[action]")]
    public class HomeController : Controller
    {
        #region properties
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAccountService _accountService;
        private readonly IActivityService _activityService;
        private readonly ICourseService _courseService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        #endregion

        #region constractor
        public HomeController(IUnitOfWork unitOfWork, IAccountService accountService, IActivityService activityService, ICourseService courseService, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _accountService = accountService;
            _activityService = activityService;
            _courseService = courseService;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }
        #endregion

        #region actions
        [YesGet]
        [MemberAuthorize]
        public async Task<IActionResult> Index()
        {
            var lang = SessionHelper.GetCurrentLanguage();

            #region Toastr Succcesfully After Login
            // Get the previous link (the page the user came from)
            var previousUrl = Request.Headers["Referer"].ToString();

            ViewData["ShowToastrLoginSuccesfullyLoggedIn"] = false;
            if (!string.IsNullOrEmpty(previousUrl) && previousUrl.Contains("Member/Account/Login", StringComparison.OrdinalIgnoreCase))
            {
                // ✅ Here if the previous link is the login page
                // You can do any action (e.g. ViewData's Toastr view)
                ViewData["ShowToastrLoginSuccesfullyLoggedIn"] = true;
            }
            bool showToastr = ViewData["ShowToastrLoginSuccesfullyLoggedIn"] as bool? ?? false;
            HttpContext.Session.SetString("ShowToastrLoginSuccesfullyLoggedIn", showToastr.ToString());

            #endregion Toastr Succcesfully After Login

            var username = _httpContextAccessor?.HttpContext?.Session.GetString("Email");
            if (username == null) return NotFound();
            var user = await _unitOfWork.Members.GetByColumnAsync(u => u.Email == username);


            #region courses
            var (Courses, coursesSubscriptions) = await _courseService.GetAllAsync(username);
            DateOnly datenow = DateOnly.FromDateTime(AppDubaiTime1.Now.Date);
            Courses = Courses.Where(x=>x.StartDate >= datenow);
            var coursesVM = _mapper.Map<List<CourseVM>>(Courses);
            foreach (var vm in coursesVM)
            {
                vm.SubscriptionId = coursesSubscriptions.FirstOrDefault(e => e.SubscribedInId == vm.Id && e.MemberId == user.Id)?.Id;
                vm.Subscribed = coursesSubscriptions.Any(e => e.SubscribedInId == vm.Id);
                vm.selectedRate = coursesSubscriptions.FirstOrDefault(e => e.SubscribedInId == vm.Id)?.Rate;
                vm.Attendance = coursesSubscriptions.FirstOrDefault(e => e.SubscribedInId == vm.Id)?.Attendance;
                vm.Accepted = coursesSubscriptions.FirstOrDefault(e => e.SubscribedInId == vm.Id)?.Acceptance;
                vm.RejectionNotes = coursesSubscriptions.FirstOrDefault(e => e.SubscribedInId == vm.Id)?.Notes;
            }
            // show upcoming courses earliest first
            coursesVM = coursesVM.OrderBy(x => x.StartDate ?? DateOnly.FromDateTime(DateTime.MaxValue)).Take(5).ToList();
            #endregion

            #region activities
            var (Activities, activitiesSubscriptions) = await _activityService.GetAllAsync(username);
            // show upcoming activities earliest first
            Activities = Activities.OrderBy(x => x.StartDate ?? DateOnly.FromDateTime(DateTime.MaxValue)).Take(5);
            var activitiesVM = _mapper.Map<List<ActivityVM>>(Activities);
            foreach (var vm in activitiesVM)
            {
                vm.Subscribed = activitiesSubscriptions.Any(e => e.SubscribedInId == vm.Id);
            }
            #endregion

            var model = new HomeActivityCourseVM
            {
                Activities = activitiesVM,
                Courses = coursesVM,
            };

            //activities = activities.Where(x => x.StartDate > DateOnly.FromDateTime(AppDubaiTime.Now)).OrderBy(x => x.StartDate);

            model.MemberName = lang == "ar" ? user?.FullNameAr : user?.FullNameEn;


            return View(model);
        }
        public IActionResult Privacy()
        {
            return View();
        }

        // Used For Change Language [ Ar - En ]
        [HttpGet]
        public IActionResult ChangeLanguage(string lang)
        {
            HttpContext.Session.SetString("CurrentCulture", lang);

            // Redirect to previous page (Referer)
            var referer = Request.Headers["Referer"].ToString();
            return Redirect(referer ?? "/");
        }

        [IgnoreAction]
        [YesGet]
        [AllowAnonymous]
        public IActionResult HttpStatusCodeHandler(int statusCode)
        {
            switch (statusCode)
            {
                case 404:
                    ViewBag.ErrorMessage = Resource1.pageNotFound;
                    return View("Error404");
                case 403:
                    ViewBag.ErrorMessage = Resource1.YouNotAuthorizedAccessPage;
                    return View("Error403");
                case 401:
                    ViewBag.ErrorMessage = Resource1.YouNotAuthorizedAccessPage; // Not Loggined in now UN Authorization
                    return View("Error403");
                case 500:
                    return View("Error500");
                case 503:
                    return View("Error503");
                default:
                    ViewBag.ErrorMessage = Resource1.AnUnexpectedErrorOccurred;
                    return View("Error500");
            }
        }
        #endregion
    }
}
