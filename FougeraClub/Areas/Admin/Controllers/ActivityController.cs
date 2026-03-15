using Application.Helpers;
using Application.Interfaces.Admin;
using Application.Services.Admin;
using AutoMapper;
using Domain.DTOs;
using Domain.Entities;
using Domain.Enums;
using Domain.Resources;
using FougeraClub.Areas.Admin.ViewModels.Activity;
using FougeraClub.Areas.Admin.ViewModels.Member;
using FougeraClub.Attributes;
using FougeraClub.Helpers;
using FougeraClub.Middelware;
using Infrastructure.Repositories.InterfacesDB;
using Microsoft.AspNetCore.Mvc;


namespace FougeraClub.Areas.Admin.Controllers
{
    [AdminAuthorize]
    [Area("Admin")]
    public class ActivityController : Controller
    {
        #region Properties
        private readonly IActivityService _ActivityService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IExcelReportService<Activity> _excelReportService;
        #endregion

        #region Constructor
        public ActivityController(IActivityService ActivityService, IUnitOfWork unitOfWork, IMapper mapper, IExcelReportService<Activity> excelReportService)
        {
            _ActivityService = ActivityService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _excelReportService = excelReportService;
        }
        #endregion

        #region Actions
        [YesGet]
        public async Task<IActionResult> Index(string? searchTerm, DateOnly? dateFrom, DateOnly? dateTo, int page = 1, int pageSize = 50)
        {
            var lang = SessionHelper.GetCurrentLanguage();

            var allActivitys = await _ActivityService.GetAllAsync();
            var ActivityVMs = _mapper.Map<List<ActivityVM>>(allActivitys).AsQueryable();

            var allSubscriptions = await _unitOfWork.Subscriptions.GetAllAsync(s => s.SubscribedInType == SubscriptionType.Activity);

            foreach (var Activity in ActivityVMs)
            {
                var days1 = (Activity.EndDate?.ToDateTime(TimeOnly.MinValue) - Activity.StartDate?.ToDateTime(TimeOnly.MinValue));
                int? days = ((days1?.Days) ?? 0) + 1;
                Activity.ActivityDuration = days;
                int? SubscriptionCount = allSubscriptions.Where(s => s.SubscribedInId == Activity.Id).Count();
                Activity.SubscriptionCount = SubscriptionCount;
            }


            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                ActivityVMs = ActivityVMs.Where(c =>
                    (!string.IsNullOrEmpty(c.TitleAr) && c.TitleAr.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(c.TitleEn) && c.TitleEn.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                );
            }

            var allDepartments = await _unitOfWork.Departments.GetAllAsync();
            ViewBag.Departments = SelectListHelper.BindSelectList(allDepartments.ToList()).Distinct();


            // ✅ Date filtering — replace `c.YourDateProperty` with your actual property
            if (dateFrom.HasValue)
            {
                ActivityVMs = ActivityVMs.Where(c => c.StartDate >= dateFrom.Value);
            }
            if (dateTo.HasValue)
            {
                ActivityVMs = ActivityVMs.Where(c => c.StartDate <= dateTo.Value);
            }
            ViewBag.CountRecords = ActivityVMs?.Count();

            var paginated = PaginatedList<ActivityVM>.Create(ActivityVMs?.OrderByDescending(m => m.Id), page, pageSize, searchTerm);

            ViewBag.dateFrom = dateFrom?.ToString("yyyy-MM-dd");
            ViewBag.dateTo = dateTo?.ToString("yyyy-MM-dd");
            //// Calling from Ajax Return PartialView
            //if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            //{
            //    return PartialView("_ListPartial", paginated);
            //}

            return View(paginated);
        }
        public async Task<IActionResult> AddEdit(int? id)
        {
            var vm = new ActivityVM();

            if (id.HasValue && id.Value != 0)
            {
                var Activity = await _ActivityService.GetByIdAsync(id.Value);
                if (Activity == null) return NotFound();
                vm = _mapper.Map<ActivityVM>(Activity);
            }


            var trainers = await _unitOfWork.Trainers.GetAllAsync();
            var departments = await _unitOfWork.Departments.GetAllAsync();
            var users = await _unitOfWork.Users.GetAllAsync();
            vm.Attachment_OldPath = vm.AttachmentPath;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEdit(ActivityVM model)
        {
            var FolderEntityWillSaveIn = "Activities";
            #region Validate Is File is PDF And MG // Validate PDF

                IFormFile? AttachmentFile_Temp = !string.IsNullOrEmpty(model.Attachment_TempFilePath) ? FileHelper.ConvertToIFormFile(model.Attachment_TempFilePath) : model.Attachment;
                string? Attachment_path = !string.IsNullOrEmpty(model.Attachment_TempFilePath) ? model.Attachment_TempFilePath : model.AttachmentPath;
                var Attachment_Text = await FileHelper.CheckFileIsPdf_5Mg_Async(AttachmentFile_Temp);
                if (Attachment_Text != "OK" && Attachment_Text != "null")
                {
                    ModelState.AddModelError("Attachment", Attachment_Text);
                    FileHelper.DeleteImageFile(Attachment_path);
                }
            #endregion Validate Is File is PDF And MG // Validate PDF

            #region validation fails // !ModelState.IsValid
                // If validation fails
                if (!ModelState.IsValid)
                {
                    ModelState.Remove("Attachment_TempFilePath"); // its Important To Bind New Data Temp
                    ModelState.Remove("Attachment_OldPath"); // its Important To Bind New Data Temp
                    ModelState.Remove("Attachment"); // its Important To Bind New Data Temp
                    if (Attachment_Text != "OK" && Attachment_Text != "null") { ModelState.AddModelError("Attachment", Attachment_Text); }

                // If the user uploads a new file → cache it before returning
                if (model.Attachment != null)
                        model.Attachment_TempFilePath = await FileHelper.SaveTempAsync(model.Attachment);

                    // Refill dropdowns if validation fails
                    var trainers = await _unitOfWork.Trainers.GetAllAsync();
                    var departments = await _unitOfWork.Departments.GetAllAsync();
                    var users = await _unitOfWork.Users.GetAllAsync();

                    return View(model);
                }
            #endregion validation fails // !ModelState.IsValid

            // ---------------------------
            //  PROCESS FINAL FILE
            // ---------------------------
            #region PROCESS FINAL FILE Handeling Save in newpath From Temp
                string finalFileName = null;

                // 1) If there is a new file uploaded by the user
                if (model.Attachment != null)
                {
                FileHelper.DeleteImageFile(model.Attachment_OldPath);
                model.AttachmentPath = await FileHelper.SaveImageAsync(model.Attachment, FolderEntityWillSaveIn);
                }

                //2) If there is no new upload but there is a temporarily saved file
                else if (!string.IsNullOrEmpty(model.Attachment_TempFilePath))
                {
                FileHelper.DeleteImageFile(model.Attachment_OldPath);
                model.AttachmentPath = FileHelper.MoveTempToFinal(
                        model.Attachment_TempFilePath,
                        out finalFileName,
                        FolderEntityWillSaveIn
                    );
                }

                // 3) If neither this nor that → use the old file (Edit only)
                else
                {
                    model.AttachmentPath = model.Attachment_OldPath;
                }
            #endregion PROCESS FINAL FILE Handeling Save in newpath From Temp
            // ---------------------------------------
            // Map model → entity After Added New Path


            // ---------------------------------------
            var entity = _mapper.Map<Activity>(model);
            entity.AttachmentPath = model.AttachmentPath;
            // ---------------------------------------
            // CREATE OR UPDATE
            // ---------------------------------------
            if (model.Id == 0)
            {
                model.Id = await _ActivityService.AddAsync(entity, null);
                return RedirectToAction("Index");
            }
            else
            {
                await _ActivityService.UpdateAsync(entity, null);
                return RedirectToAction(nameof(AddEdit), new { id = model.Id });
            }
        }
        
        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> Details(int id)
        {
            var activity = await _ActivityService.GetByIdAsync(id);
            if (activity == null)
                return NotFound();
            var model = _mapper.Map<ActivityVM>(activity);
            var days1 = (model.EndDate?.ToDateTime(TimeOnly.MinValue) - model.StartDate?.ToDateTime(TimeOnly.MinValue));
            int? days = ((days1?.Days) ?? 0) + 1;
            model.ActivityDuration = days;
            if (!FileHelper.IsFileExist(model.AttachmentPath)) model.AttachmentPath = null;

            return PartialView("_DetailsPartial", model);
        }
        [YesGet]
        public async Task<IActionResult> MembersActivity(int activityId, int page = 1, int pageSize = 50)
        {
            var activity = await _unitOfWork.Activities.GetByIdAsync(activityId);
            var (Members, subscriptions) = await _ActivityService.GetAllMembersOfActivityAsync(activityId);
            var MembersVM = _mapper.Map<List<MemberVM>>(Members);

            var paginated = PaginatedList<MemberVM>.Create(MembersVM, page, pageSize, null);


            var MembersActivityVM = new SubscribedMemberInActivitiesVM
            {
                Activity = _mapper.Map<ActivityVM>(activity),
                Members_Paginated = paginated,
                Subscriptions = subscriptions
            };
            // Calling from Ajax Return PartialView
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_ListPartialMemberSubsInEvents", MembersActivityVM);
            }

            return View(MembersActivityVM);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _ActivityService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> Print(string? searchTerm, DateOnly? dateFrom, DateOnly? dateTo)
        {
            var allActivitys = await _ActivityService.GetAllAsync();
            var ActivityVMs = _mapper.Map<List<ActivityVM>>(allActivitys).AsQueryable();

            var allSubscriptions = await _unitOfWork.Subscriptions.GetAllAsync(s => s.SubscribedInType == SubscriptionType.Activity);


            foreach (var Activity in ActivityVMs)
            {
                var days1 = (Activity.EndDate?.ToDateTime(TimeOnly.MinValue) - Activity.StartDate?.ToDateTime(TimeOnly.MinValue));
                int? days = ((days1?.Days) ?? 0) + 1;
                Activity.ActivityDuration = days;
                int? SubscriptionCount = allSubscriptions.Where(s => s.SubscribedInId == Activity.Id).Count();
                Activity.SubscriptionCount = SubscriptionCount;
            }

            #region search by word
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                ActivityVMs = ActivityVMs.Where(c =>
                    (!string.IsNullOrEmpty(c.TitleAr) && c.TitleAr.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(c.TitleEn) && c.TitleEn.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                );
            }
            #endregion

            #region search by date
            // ✅ Date filtering — replace `c.YourDateProperty` with your actual property
            if (dateFrom.HasValue)
            {
                ActivityVMs = ActivityVMs.Where(c => c.StartDate >= dateFrom.Value);
            }
            if (dateTo.HasValue)
            {
                ActivityVMs = ActivityVMs.Where(c => c.StartDate <= dateTo.Value);
            }
            #endregion


            return View(ActivityVMs?.OrderByDescending(m => m.Id));
        }

        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> ExportToExcel(string? searchTerm, DateOnly? dateFrom, DateOnly? dateTo)
        {
            var allActivitys = await _ActivityService.GetAllAsync();
            var ActivityVMs = _mapper.Map<List<ActivityVM>>(allActivitys).AsQueryable();

            var allSubscriptions = await _unitOfWork.Subscriptions.GetAllAsync(s => s.SubscribedInType == SubscriptionType.Activity);


            foreach (var Activity in ActivityVMs)
            {
                var days1 = (Activity.EndDate?.ToDateTime(TimeOnly.MinValue) - Activity.StartDate?.ToDateTime(TimeOnly.MinValue));
                int? days = ((days1?.Days) ?? 0) + 1;
                Activity.ActivityDuration = days;
                int? SubscriptionCount = allSubscriptions.Where(s => s.SubscribedInId == Activity.Id).Count();
                Activity.SubscriptionCount = SubscriptionCount;
            }

            #region search by word
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                ActivityVMs = ActivityVMs.Where(c =>
                    (!string.IsNullOrEmpty(c.TitleAr) && c.TitleAr.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(c.TitleEn) && c.TitleEn.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                );
            }
            #endregion

            #region search by date
            // ✅ Date filtering — replace `c.YourDateProperty` with your actual property
            if (dateFrom.HasValue)
            {
                ActivityVMs = ActivityVMs.Where(c => c.StartDate >= dateFrom.Value);
            }
            if (dateTo.HasValue)
            {
                ActivityVMs = ActivityVMs.Where(c => c.StartDate <= dateTo.Value);
            }
            #endregion


            return View(ActivityVMs);
        }
        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> createExcelReport_Download(string? searchTerm, DateOnly? dateFrom, DateOnly? dateTo)
        {
            // ---- Start Get Data As Print
            var allActivitys = await _ActivityService.GetAllAsync();
            var ActivityVMs = _mapper.Map<List<ActivityVM>>(allActivitys).AsQueryable();

            var allSubscriptions = await _unitOfWork.Subscriptions.GetAllAsync(s => s.SubscribedInType == SubscriptionType.Activity);


            foreach (var Activity in ActivityVMs)
            {
                var days1 = (Activity.EndDate?.ToDateTime(TimeOnly.MinValue) - Activity.StartDate?.ToDateTime(TimeOnly.MinValue));
                int? days = ((days1?.Days) ?? 0) + 1;
                Activity.ActivityDuration = days;
                int? SubscriptionCount = allSubscriptions.Where(s => s.SubscribedInId == Activity.Id).Count();
                Activity.SubscriptionCount = SubscriptionCount;
            }

            #region search by word
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                ActivityVMs = ActivityVMs.Where(c =>
                    (!string.IsNullOrEmpty(c.TitleAr) && c.TitleAr.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(c.TitleEn) && c.TitleEn.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                );
            }
            #endregion

            #region search by date
            // ✅ Date filtering — replace `c.YourDateProperty` with your actual property
            if (dateFrom.HasValue)
            {
                ActivityVMs = ActivityVMs.Where(c => c.StartDate >= dateFrom.Value);
            }
            if (dateTo.HasValue)
            {
                ActivityVMs = ActivityVMs.Where(c => c.StartDate <= dateTo.Value);
            }
            #endregion
            // ---- End Get Data As Print

            var boolStatus = false;
            byte[]? fileBytes = null;
            var pathNewFile = "";
            try
            {

                var lang = SessionHelper.GetCurrentLanguage();
                //var allActivitys = await _ActivityService.GetAllAsync();
                var allData_list = ActivityVMs?.OrderByDescending(m => m.Id);
                var ListTitles = new List<string>
                {
                    Resource1.ActivityTitle,Resource1.StartDate,Resource1.Location,Resource1.ActivityDuration,Resource1.Ages,Resource1.SubscriptionCount,
                };
                if (allData_list != null || allData_list?.Count() > 0)
                {
                    var excelDataDTO = allData_list.Select(single => new ExcelDataDTO
                    {
                        t1 = (lang == "ar" ? single.TitleAr : single.TitleEn),
                        t2 = single.StartDate.HasValue ? single.StartDate.Value.ToString("d").Replace("/","-") : "",
                        t3 = single.Location,
                        t4 = single.ActivityDuration,
                        //t5 = $"{Resource2.Above} {single.MinimumAge} {Resource1.Year1}",
                        t5 = $"{single.MinimumAge}",
                        t6 = single.SubscriptionCount,
                    }).ToList();

                    if (lang == "ar")
                    {
                        (boolStatus, fileBytes) = ExcelStaticReport.ExcelReportArEn_<ExcelDataDTO>(excelDataDTO, ListTitles, 0, "ar");
                    }
                    else
                    {
                        (boolStatus, fileBytes) = ExcelStaticReport.ExcelReportArEn_<ExcelDataDTO>(excelDataDTO, ListTitles, 0, "en");
                    }
                }

                FileContentResult? Excelfile = null;
                if (fileBytes != null && fileBytes.Length > 0 && boolStatus == true)
                {
                    var fileExcelName = Resource1.EventsList;
                    Excelfile = File(fileBytes,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        $"{fileExcelName}_{AppDubaiTime.Now:yyyyMMdd_HHmmss}.xlsx");
                }
                return Excelfile;


            }
            catch (Exception ex)
            {
                return RedirectToAction("Index");
            }
        }

        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> PrintMembersActivity(int ActivityId)
        {
            var activity = await _unitOfWork.Activities.GetByIdAsync(ActivityId);
            var (Members, subscriptions) = await _ActivityService.GetAllMembersOfActivityAsync(ActivityId);
            var MembersVM = _mapper.Map<List<MemberVM>>(Members);
            var MembersActivityVM = new MembersActivityVM
            {
                Activity = _mapper.Map<ActivityVM>(activity),
                Members = MembersVM,
                Subscriptions = subscriptions
            };

            return View(MembersActivityVM);
        }
        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> createExcelReport_Download_MembersActivity(int ActivityId)
        {
            var activity = await _unitOfWork.Activities.GetByIdAsync(ActivityId);
            var (Members, subscriptions) = await _ActivityService.GetAllMembersOfActivityAsync(ActivityId);

            // ---- End Get Data As Print

            var boolStatus = false;
            byte[]? fileBytes = null;
            var pathNewFile = "";
            try
            {

                var lang = SessionHelper.GetCurrentLanguage();
                //var allActivitys = await _ActivityService.GetAllAsync();
                var allData_list = Members;
                var ListTitles = new List<string>
        {
            Resource1.subscriberName,Resource1.NationalityId,Resource1.SubscriptionHistory,
            Resource1.Age,Resource1.Mobile,Resource1.Email,
        };
                if (allData_list != null || allData_list?.Count() > 0)
                {
                    var excelDataDTO = allData_list.Select(single => new ExcelDataDTO
                    {
                        t1 = (lang == "ar" ? single.FullNameAr : single.FullNameEn),
                        t2 = (single.Nationality != null) ? (lang == "ar" ? single.Nationality.NameAr : single.Nationality.NameEn) : "",
                        t3 = (subscriptions != null && subscriptions.Count() > 0 && subscriptions.Where(x => x.MemberId == single.Id).FirstOrDefault() != null && subscriptions.Where(x => x.MemberId == single.Id).FirstOrDefault().ParticipationDate.HasValue) ? (subscriptions.Where(x => x.MemberId == single.Id).FirstOrDefault().ParticipationDate.Value.ToString("d")?.Replace("/","-")) : "",
                        t4 = single.Age,
                        t5 = single.PhoneNumber,
                        t6 = single.Email,
                    }).ToList();

                    if (lang == "ar")
                    {
                        (boolStatus, fileBytes) = ExcelStaticReport.ExcelReportArEn_<ExcelDataDTO>(excelDataDTO, ListTitles, 0, "ar");
                    }
                    else
                    {
                        (boolStatus, fileBytes) = ExcelStaticReport.ExcelReportArEn_<ExcelDataDTO>(excelDataDTO, ListTitles, 0, "en");
                    }
                }

                FileContentResult? Excelfile = null;
                if (fileBytes != null && fileBytes.Length > 0 && boolStatus == true)
                {
                    var fileExcelName = Resource1.ParticipantsInActivity + " : " + (lang == "ar" ? activity?.TitleAr : activity?.TitleEn);
                    Excelfile = File(fileBytes,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        $"{fileExcelName}_{AppDubaiTime.Now:yyyyMMdd_HHmmss}.xlsx");
                }
                return Excelfile;


            }
            catch (Exception ex)
            {
                return RedirectToAction("Index");
            }
        }
        #endregion
    }

}
