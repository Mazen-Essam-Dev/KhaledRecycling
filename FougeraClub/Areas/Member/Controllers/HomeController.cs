using Application.Helpers;
using Application.Interfaces.Member;
using AutoMapper;
using Domain.HelperForDomain;
using Domain.Resources;
using KhaledTeamRecycling.Areas.Member.ViewModels;
using KhaledTeamRecycling.Attributes;
using KhaledTeamRecycling.Helpers;
using KhaledTeamRecycling.Middelware;
using Infrastructure.Attributes;
using Infrastructure.Repositories.InterfacesDB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KhaledTeamRecycling.Areas.Member.Controllers
{
    [Area("Member")]
    [Route("Member/[controller]/[action]")]
    public class HomeController : Controller
    {
        #region properties
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAccountService _accountService;

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        #endregion

        #region constractor
        public HomeController(IUnitOfWork unitOfWork, IAccountService accountService, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _accountService = accountService;
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





            var model = new HomeActivityCourseVM
            {
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
