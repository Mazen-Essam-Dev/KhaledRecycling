using Application.Helpers;
using Application.Interfaces.Member;
using AutoMapper;
using Domain.Resources;
using FougeraClub.Areas.Member.ViewModels;
using FougeraClub.Attributes;
using FougeraClub.Middelware;
using Infrastructure.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace FougeraClub.Areas.Member.Controllers
{

    [MemberAuthorize]
    [Area("Member")]
    [Route("Member/[controller]/[action]")]
    public class ActivityController : Controller
    {
        #region properties
        private readonly IActivityService _activityService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        #endregion

        #region constructor
        public ActivityController(IActivityService activityService, IHttpContextAccessor httpContextAccessor, IMapper mapper)
        {
            _activityService = activityService;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }
        #endregion

        #region actions
        [YesGet]
        public async Task<IActionResult> Index()
        {
            var session = _httpContextAccessor?.HttpContext?.Session;
            var username = session?.GetString("Email");
            var (Activities, Subscriptions) = await _activityService.GetAllAsync(username);
            var model = _mapper.Map<List<ActivityVM>>(Activities);
            foreach (var vm in model)
            {
                vm.Subscribed = Subscriptions.Any(e => e.SubscribedInId == vm.Id);
            }

            // Order activities by StartDate ascending so earliest dates show first
            model = model.OrderBy(x => x.StartDate ?? DateOnly.FromDateTime(DateTime.MaxValue)).ToList();
            return View(model);
        }
        [YesGet]
        public async Task<IActionResult> Details(int id)
        {
            var activity1 = await _activityService.GetByIdAsync(id);
            if (activity1 == null)
                return NotFound();

            var activity = _mapper.Map<ActivityVM>(activity1);
            activity.AttachmentPath = activity.AttachmentPath?.Replace("~", "");
            if (!FileHelper.IsFileExist(activity.AttachmentPath)) activity.AttachmentPath = null;

            var days1 = (activity.EndDate?.ToDateTime(TimeOnly.MinValue) - activity.StartDate?.ToDateTime(TimeOnly.MinValue));
            int? days = ((days1?.Days) ?? 0) + 1;
            activity.ActivityDuration = days;

            return PartialView("_DetailsPartial", activity);
        }

        public async Task<IActionResult> Subscribe(int? id)
        {
            if (id == null) return NotFound();

            var activity = await _activityService.GetByIdAsync(id.Value);
            if (activity == null) return NotFound();

            return View(activity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Subscribe(int id)
        {
            if (id != 0)
            {
                var session = _httpContextAccessor?.HttpContext?.Session;
                var email = session?.GetString("Email");
                await _activityService.SubscribeAsync(id, email);
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Subscribe), new { id });
        }
        [IgnoreAction]
        public async Task<IActionResult> CheckAge(int activityId)
        {
            if (activityId != 0)
            {
                var session = _httpContextAccessor?.HttpContext?.Session;
                var email = session?.GetString("Email");
                var result = await _activityService.CheckAgeAsync(activityId, email);
                if (!result)
                {
                    return Json(new { valid = result, message = Resource2.AgeRequirementNotMet });
                }
                return Json(new { valid = result });
            }
            return NotFound();
        }
        #endregion
    }
}
