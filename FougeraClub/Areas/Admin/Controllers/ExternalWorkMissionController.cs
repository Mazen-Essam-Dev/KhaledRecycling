using Application.Helpers;
using Application.Interfaces.Admin;
using Application.Services.Admin;
using AutoMapper;
using Domain.DTOs;
using Domain.Entities;
using Domain.Enums;
using Domain.Resources;
using FougeraClub.Areas.Admin.ViewModels.ExternalWorkMission;
using FougeraClub.Areas.Admin.ViewModels.SMS;
using FougeraClub.Attributes;
using FougeraClub.Helpers;
using FougeraClub.Middelware;
using Infrastructure.Repositories;
using Infrastructure.Repositories.InterfacesDB;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;


namespace FougeraClub.Areas.Admin.Controllers
{
    [AdminAuthorize]
    [Area("Admin")]
    public class ExternalWorkMissionController : Controller
    {
        private readonly IExternalWorkMissionService _ExternalWorkMissionService;
        private readonly IMapper _mapper;
        private readonly UserManager<Infrastructure.Identity.ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHubContext<Hub.NotificationHub> _hubContext;
        private readonly INotificationService _notificationService;

        public ExternalWorkMissionController(IExternalWorkMissionService ExternalWorkMissionService, IMapper mapper, UserManager<Infrastructure.Identity.ApplicationUser> userManager, IUnitOfWork unitOfWork , INotificationService notificationService, IHubContext<Hub.NotificationHub> hubContext)
        {
            _ExternalWorkMissionService = ExternalWorkMissionService;
            _mapper = mapper;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
            _hubContext = hubContext;
        }

        [YesGet]
        public async Task<IActionResult> Index(string? searchTerm, DateOnly? dateFrom, DateOnly? dateTo, int page = 1, int pageSize = 50)
        {
            var allEmployeesNames = await _ExternalWorkMissionService.GetAllEmplyeeNames();
            var EmployeesNamesHasTerm = allEmployeesNames;
            var EmpIds = new List<int>();
            ViewBag.EmployeesNames = allEmployeesNames;
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                EmployeesNamesHasTerm = EmployeesNamesHasTerm.Where(c =>
                    (!string.IsNullOrEmpty(c.FullNameAr) && c.FullNameAr.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(c.FullNameEn) && c.FullNameEn.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                );
            }
            if (EmployeesNamesHasTerm.Count() > 0) EmpIds = EmployeesNamesHasTerm.Select(x => x.Id).ToList();

            var allExternalWorkMissions = await _ExternalWorkMissionService.GetAllAsync();

            var ExternalWorkMissionVMs = _mapper.Map<List<ExternalWorkMissionVM>>(allExternalWorkMissions).AsQueryable();


            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                ExternalWorkMissionVMs = ExternalWorkMissionVMs.Where(c =>
                    ((c.EmployeeId != null) && EmpIds.Contains(c.EmployeeId ?? 0))
                );
            }


            // ✅ Date filtering — replace `c.YourDateProperty` with your actual property
            if (dateFrom.HasValue)
            {
                ExternalWorkMissionVMs = ExternalWorkMissionVMs.Where(c => c.MissionDate >= dateFrom.Value);
            }
            if (dateTo.HasValue)
            {
                ExternalWorkMissionVMs = ExternalWorkMissionVMs.Where(c => c.MissionDate <= dateTo.Value);
            }

            var paginated = PaginatedList<ExternalWorkMissionVM>.Create(ExternalWorkMissionVMs, page, pageSize, searchTerm);

            ViewBag.dateFrom = dateFrom?.ToString("yyyy-MM-dd");
            ViewBag.dateTo = dateTo?.ToString("yyyy-MM-dd");

            TempData["FromExternalWorkMissionIndex"] = "true";

            // Calling from Ajax Return PartialView
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_ListPartial", paginated);
            }

            return View(paginated);
        }

        public async Task<IActionResult> AddEdit(int? id)
        {
            var allEmployeesNames = await _ExternalWorkMissionService.GetAllEmplyeeNames();
            ViewBag.EmployeesNames = allEmployeesNames;
            ViewBag.selectedEmployeeJob = " ";
            var lang = SessionHelper.GetCurrentLanguage();

            var vm = new ExternalWorkMissionVM();
            if (id == 0 || id == null)
            {
                return View(vm);
            }
            var ExternalWorkMission = await _ExternalWorkMissionService.GetByIdAsync(id.Value);
            if (ExternalWorkMission == null) return NotFound();
            ViewBag.selectedEmployee = ExternalWorkMission.EmployeeId;

            ViewBag.selectedEmployeeJob = allEmployeesNames?.Where(e => e.Id == ExternalWorkMission.EmployeeId)?.FirstOrDefault()?.JobTitle;
            vm = _mapper.Map<ExternalWorkMissionVM>(ExternalWorkMission);

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> AddEdit(ExternalWorkMissionVM model, List<int>? MissionTypes)
        {
            var allEmployeesNames = await _ExternalWorkMissionService.GetAllEmplyeeNames();
            ViewBag.EmployeesNames = allEmployeesNames;
            var lang = SessionHelper.GetCurrentLanguage();

            ViewBag.selectedEmployeeJob = allEmployeesNames?.Where(e => e.Id == model.EmployeeId)?.FirstOrDefault()?.JobTitle;

            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var entity = _mapper.Map<ExternalWorkMission>(model);

            if (model.Id == 0)
            {
                model.Id = await _ExternalWorkMissionService.AddAsync(entity);
                await _ExternalWorkMissionService.UpdateNewMissionsType(MissionTypes, model.Id);
                await _hubContext.Clients.Groups("Manager")
                   .SendAsync("ReceiveNotification", new
                   {
                       Title = "",
                       Message = ""
                   });
                await _notificationService.SendNotificationToRoleAsync(
                      "مهمة عمل خارجية جديدة",
                      $"يوجد مهمة عمل خارجية رقم {model?.Id} جديدة جاهز للإعتماد",
                      2
                  );
                return RedirectToAction(nameof(Index)); // After Add New
            }
            else
                await _ExternalWorkMissionService.UpdateAsync(entity);

            await _ExternalWorkMissionService.UpdateNewMissionsType(MissionTypes, model.Id);

            return RedirectToAction(nameof(AddEdit), new { model.Id }); // After Edit 
        }

        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> Details(int? id)
        {
            var lang = SessionHelper.GetCurrentLanguage();
            var allEmployeesNames = await _ExternalWorkMissionService.GetAllEmplyeeNames();
            ViewBag.EmployeesNames = allEmployeesNames;
            ViewBag.selectedEmployeeJob = " ";
            var vm = new ExternalWorkMissionVM();
            if (id == 0 || id == null)
            {
                return View(vm);
            }
            var ExternalWorkMission = await _ExternalWorkMissionService.GetByIdAsync(id.Value);
            if (ExternalWorkMission == null) return NotFound();
            ViewBag.selectedEmployee = ExternalWorkMission.EmployeeId;
            lang = SessionHelper.GetCurrentLanguage();

            ViewBag.selectedEmployeeJob = allEmployeesNames?.Where(e => e.Id == ExternalWorkMission.EmployeeId)?.FirstOrDefault()?.JobTitle;

            vm = _mapper.Map<ExternalWorkMissionVM>(ExternalWorkMission);

            // Set ManagerFullName if signature exists
            if (vm.Signature != null && !string.IsNullOrEmpty(vm.Signature.UserId))
            {
                var manager = await _userManager.FindByIdAsync(vm.Signature.UserId);
                vm.ManagerFullName = manager?.FullNameAr ?? manager?.FullNameEn ?? manager?.UserName ?? manager?.Email ?? "";
            }

            return View(vm);
        }

        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> PrintDetails(int? id)
        {
            var allEmployeesNames = await _ExternalWorkMissionService.GetAllEmplyeeNames();
            ViewBag.EmployeesNames = allEmployeesNames;
            ViewBag.selectedEmployeeJob = " ";
            var vm = new ExternalWorkMissionVM();
            if (id == 0 || id == null)
            {
                return View(vm);
            }
            var ExternalWorkMission = await _ExternalWorkMissionService.GetByIdAsync(id.Value);
            if (ExternalWorkMission == null) return NotFound();
            ViewBag.selectedEmployee = ExternalWorkMission.EmployeeId;
            var lang = SessionHelper.GetCurrentLanguage();

            ViewBag.selectedEmployeeJob = allEmployeesNames?.Where(e => e.Id == ExternalWorkMission.EmployeeId)?.FirstOrDefault()?.JobTitle;
            vm = _mapper.Map<ExternalWorkMissionVM>(ExternalWorkMission);

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _ExternalWorkMissionService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [IgnoreAction]
        [NoLogging]
        [HttpPost]
        public async Task<IActionResult> SendOtp()
        {
            try
            {
                var status = await _ExternalWorkMissionService.SendOtpAsync();
                return Json(new { success = status });
            }
            catch (Exception)
            {
                return Json(new { success = false });
            }
        }

        [HttpPost]
        [IgnoreAction]
        public async Task<IActionResult> ValidateOtp([FromBody] OtpValidationRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Code))
                return Json(new { success = false, message = "Invalid data." });

            // Fix: Convert request.Id (long) to int for ValidateOtpAsync
            bool isValid = await _ExternalWorkMissionService.ValidateOtpAsync((int)request.Id, request.Code);

            if (isValid)
                return Json(new { success = true });

            return Json(new { success = false, message = "Incorrect OTP code." });
        }

        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> Print(string? searchTerm, DateOnly? dateFrom, DateOnly? dateTo, int page = 1, int pageSize = 50)
        {

            var allEmployeesNames = await _ExternalWorkMissionService.GetAllEmplyeeNames();
            var EmployeesNamesHasTerm = allEmployeesNames;
            var EmpIds = new List<int>();
            ViewBag.EmployeesNames = allEmployeesNames;
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                EmployeesNamesHasTerm = EmployeesNamesHasTerm.Where(c =>
                    (!string.IsNullOrEmpty(c.FullNameAr) && c.FullNameAr.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(c.FullNameEn) && c.FullNameEn.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                );
            }
            if (EmployeesNamesHasTerm.Count() > 0) EmpIds = EmployeesNamesHasTerm.Select(x => x.Id).ToList();

            var allExternalWorkMissions = await _ExternalWorkMissionService.GetAllAsync();

            var ExternalWorkMissionVMs = _mapper.Map<List<ExternalWorkMissionVM>>(allExternalWorkMissions).AsQueryable();


            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                ExternalWorkMissionVMs = ExternalWorkMissionVMs.Where(c =>
                    ((c.EmployeeId != null) && EmpIds.Contains(c.EmployeeId ?? 0))
                );
            }


            // ✅ Date filtering — replace `c.YourDateProperty` with your actual property
            if (dateFrom.HasValue)
            {
                ExternalWorkMissionVMs = ExternalWorkMissionVMs.Where(c => c.MissionDate >= dateFrom.Value);
            }
            if (dateTo.HasValue)
            {
                ExternalWorkMissionVMs = ExternalWorkMissionVMs.Where(c => c.MissionDate <= dateTo.Value);
            }

            ViewBag.dateFrom = dateFrom?.ToString("yyyy-MM-dd");
            ViewBag.dateTo = dateTo?.ToString("yyyy-MM-dd");

            TempData["FromExternalWorkMissionIndex"] = "true";
            return View(ExternalWorkMissionVMs);
        }

        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> createExcelReport_Download(string? searchTerm, DateOnly? dateFrom, DateOnly? dateTo, int page = 1, int pageSize = 50)
        {
            var allEmployeesNames = await _ExternalWorkMissionService.GetAllEmplyeeNames();
            var EmployeesNamesHasTerm = allEmployeesNames;
            var EmpIds = new List<int>();
            ViewBag.EmployeesNames = allEmployeesNames;
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                EmployeesNamesHasTerm = EmployeesNamesHasTerm.Where(c =>
                    (!string.IsNullOrEmpty(c.FullNameAr) && c.FullNameAr.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(c.FullNameEn) && c.FullNameEn.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                );
            }
            if (EmployeesNamesHasTerm.Count() > 0) EmpIds = EmployeesNamesHasTerm.Select(x => x.Id).ToList();

            var allExternalWorkMissions = await _ExternalWorkMissionService.GetAllAsync();

            var ExternalWorkMissionVMs = _mapper.Map<List<ExternalWorkMissionVM>>(allExternalWorkMissions).AsQueryable();


            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                ExternalWorkMissionVMs = ExternalWorkMissionVMs.Where(c =>
                    ((c.EmployeeId != null) && EmpIds.Contains(c.EmployeeId ?? 0))
                );
            }


            // ✅ Date filtering — replace `c.YourDateProperty` with your actual property
            if (dateFrom.HasValue)
            {
                ExternalWorkMissionVMs = ExternalWorkMissionVMs.Where(c => c.MissionDate >= dateFrom.Value);
            }
            if (dateTo.HasValue)
            {
                ExternalWorkMissionVMs = ExternalWorkMissionVMs.Where(c => c.MissionDate <= dateTo.Value);
            }

            ViewBag.dateFrom = dateFrom?.ToString("yyyy-MM-dd");
            ViewBag.dateTo = dateTo?.ToString("yyyy-MM-dd");
            // ---- End Get Data As Print

            var boolStatus = false;
            byte[]? fileBytes = null;
            var pathNewFile = "";
            try
            {

                var lang = SessionHelper.GetCurrentLanguage();
                //var allActivitys = await _ActivityService.GetAllAsync();
                var allData_list = ExternalWorkMissionVMs;
                var ListTitles = new List<string>
{
    Resource1.Name,Resource1.EmployeeJob,Resource1.Date,
    Resource1.MissionLocation,Resource1.MissionCountry,Resource1.MissionType1,
};
                if (allData_list != null || allData_list?.Count() > 0)
                {
                    var excelDataDTO = allData_list.Select(single => new ExcelDataDTO
                    {
                        t1 = (allEmployeesNames != null && allEmployeesNames.Count() > 0 && allEmployeesNames.Where(e => e.Id == single.EmployeeId).FirstOrDefault() != null) ? (lang == "ar" ? allEmployeesNames.Where(e => e.Id == single.EmployeeId).FirstOrDefault().FullNameAr : allEmployeesNames.Where(e => e.Id == single.EmployeeId).FirstOrDefault().FullNameEn) : "",
                        //t2 = (allEmployeesNames != null && allEmployeesNames.Count() > 0 && allEmployeesNames.Where(e => e.Id == single.EmployeeId).FirstOrDefault() != null && allEmployeesNames.Where(e => e.Id == single.EmployeeId).FirstOrDefault().Job != null) ? (lang == "ar" ? allEmployeesNames.Where(e => e.Id == single.EmployeeId).FirstOrDefault().Job.NameAr : allEmployeesNames.Where(e => e.Id == single.EmployeeId).FirstOrDefault().Job.NameEn) : "",
                        t2 = (allEmployeesNames != null && allEmployeesNames.Count() > 0 && allEmployeesNames.Where(e => e.Id == single.EmployeeId).FirstOrDefault() != null && allEmployeesNames.Where(e => e.Id == single.EmployeeId).FirstOrDefault().JobTitle != null) ? (allEmployeesNames.Where(e => e.Id == single.EmployeeId).FirstOrDefault().JobTitle) : "",
                        t3 = (single.MissionDate.HasValue) ? single.MissionDate.Value.ToString("d").Replace("/","-") : "",
                        t4 = single.MissionLocation,
                        t5 = single.MissionCountry,
                        t6 = (single.Missions != null && single.Missions.Count() > 0) ? string.Join(" - ", single.Missions.Select(m => EnumHelper.GetDisplayName((MissionType)m.MessionKey))) : "",
                    }).ToList();

                    if (lang == "ar")
                    {
                        (boolStatus, fileBytes) = ExcelStaticReport.ExcelReportArEn_(excelDataDTO, ListTitles, 0, "ar");
                    }
                    else
                    {
                        (boolStatus, fileBytes) = ExcelStaticReport.ExcelReportArEn_(excelDataDTO, ListTitles, 0, "en");
                    }
                }

                FileContentResult? Excelfile = null;
                if (fileBytes != null && fileBytes.Length > 0 && boolStatus == true)
                {
                    var fileExcelName = Resource1.ExternalWorkMissionList;
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
        [NoLogging]
        [HttpPost]
        public async Task<IActionResult> GetJob(int? empId)
        {
            var responseNull = new
            {
                jobTitle = "",
                status = false,
            };
            if (empId == null) return Json(responseNull);

            var empJobTitle = await _unitOfWork.Employees.Table.Where(x => x.Id == empId).Select(e => e.JobTitle).FirstOrDefaultAsync();
            var jobTitle = empJobTitle;
            // Example 

            var response = new
            {
                jobTitle = jobTitle,
                status = true,
            };
            return Json(response);
        }
    }
}
