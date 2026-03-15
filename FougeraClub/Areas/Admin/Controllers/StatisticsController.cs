using Application.Interfaces.Admin;
using AutoMapper;
using FougeraClub.Areas.Admin.ViewModels.Statistics;
using FougeraClub.Attributes;
using FougeraClub.Middelware;
using Microsoft.AspNetCore.Mvc;

namespace FougeraClub.Areas.Admin.Controllers
{
    [AdminAuthorize]
    [Area("Admin")]
    [Route("Admin/[controller]/[action]")]
    public class StatisticsController : Controller
    {
        private readonly IStatisticsService _statisticsService;
        private readonly IMapper _mapper;
        public StatisticsController(IStatisticsService statisticsService, IMapper mapper)
        {
            _statisticsService = statisticsService;
            _mapper = mapper;
        }

        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> Index()
        {
            #region Toastr Succcesfully After Login
            // Get the previous link (the page the user came from)
            var previousUrl = Request.Headers["Referer"].ToString();

            ViewData["ShowToastrLoginSuccesfullyLoggedIn_Admin"] = false;
            if (!string.IsNullOrEmpty(previousUrl) && previousUrl.Contains("Identity/Account/Login", StringComparison.OrdinalIgnoreCase))
            {
                // ✅ Here if the previous link is the login page
                // You can do any action (e.g. ViewData's Toastr view)
                ViewData["ShowToastrLoginSuccesfullyLoggedIn_Admin"] = true;
            }
            bool showToastr = ViewData["ShowToastrLoginSuccesfullyLoggedIn_Admin"] as bool? ?? false;
            HttpContext.Session.SetString("ShowToastrLoginSuccesfullyLoggedIn_Admin", showToastr.ToString());
            #endregion Toastr Succcesfully After Login

            var allStatistics = await _statisticsService.GetStatisticsAsync();

            var model = _mapper.Map<StatisticsVM>(allStatistics);

            return View(model);
        }

        [IgnoreAction]
        public async Task<IActionResult> Print()
        {
            var allStatistics = await _statisticsService.GetStatisticsAsync();

            var model = _mapper.Map<StatisticsVM>(allStatistics);

            return View(model);
        }

    }
}
