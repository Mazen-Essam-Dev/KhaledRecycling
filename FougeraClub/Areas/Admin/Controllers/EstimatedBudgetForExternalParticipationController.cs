using Application.Helpers;
using Application.Interfaces.Admin;
using Application.Services.Admin;
using AutoMapper;
using Domain.DTOs;
using Domain.Entities.EstimatedBudgetForExternalParticipation;
using Domain.Resources;
using FougeraClub.Areas.Admin.ViewModels.EstimatedBudgetForExternalParticipation;
using FougeraClub.Areas.Admin.ViewModels.SMS;
using FougeraClub.Attributes;
using FougeraClub.Helpers;
using FougeraClub.Middelware;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;


namespace FougeraClub.Areas.Admin.Controllers

{
    [AdminAuthorize]
    [Area("Admin")]
    public class EstimatedBudgetForExternalParticipationController : Controller
    {
        private readonly IEstimatedBudgetForExternalParticipationService _EstimatedBudgetForExternalParticipationService;
        private readonly IMapper _mapper;
        private readonly UserManager<Infrastructure.Identity.ApplicationUser> _userManager;
        private readonly INotificationService _notificationService;
        private readonly IHubContext<Hub.NotificationHub> _hubContext;

        public EstimatedBudgetForExternalParticipationController(IEstimatedBudgetForExternalParticipationService EstimatedBudgetForExternalParticipationService, IMapper mapper, UserManager<Infrastructure.Identity.ApplicationUser> userManager, INotificationService notificationService, IHubContext<Hub.NotificationHub> hubContext)
        {
            _EstimatedBudgetForExternalParticipationService = EstimatedBudgetForExternalParticipationService;
            _mapper = mapper;
            _userManager = userManager;
            _notificationService = notificationService;
            _hubContext = hubContext;
        }


        [YesGet]
        public async Task<IActionResult> Index(string? searchTerm, DateOnly? dateFrom, DateOnly? dateTo, int page = 1, int pageSize = 50)
        {
            var allEstimatedBudgetForExternalParticipations = await _EstimatedBudgetForExternalParticipationService.GetAllAsync();

            var EstimatedBudgetForExternalParticipationVMs = _mapper.Map<List<EstimatedBudgetForExternalParticipationVM>>(allEstimatedBudgetForExternalParticipations).AsQueryable();


            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                EstimatedBudgetForExternalParticipationVMs = EstimatedBudgetForExternalParticipationVMs.Where(c =>
                    (!string.IsNullOrEmpty(c.ParticipatingTitle) && c.ParticipatingTitle.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                );
            }


            // ✅ Date filtering — replace `c.YourDateProperty` with your actual property
            if (dateFrom.HasValue)
            {
                EstimatedBudgetForExternalParticipationVMs = EstimatedBudgetForExternalParticipationVMs.Where(c => c.CreationDate >= dateFrom.Value);
            }
            if (dateTo.HasValue)
            {
                EstimatedBudgetForExternalParticipationVMs = EstimatedBudgetForExternalParticipationVMs.Where(c => c.CreationDate <= dateTo.Value);
            }

            var paginated = PaginatedList<EstimatedBudgetForExternalParticipationVM>.Create(EstimatedBudgetForExternalParticipationVMs?.OrderByDescending(m => m.Id), page, pageSize, searchTerm);

            ViewBag.dateFrom = dateFrom?.ToString("yyyy-MM-dd");
            ViewBag.dateTo = dateTo?.ToString("yyyy-MM-dd");

            TempData["FromEstimatedBudgetForExternalParticipationIndex"] = "true";

            // Calling from Ajax Return PartialView
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_ListPartial", paginated);
            }

            return View(paginated);
        }
        [IgnoreAction]
        [NoLogging]
        [HttpPost]
        public async Task<IActionResult> SendOtp()
        {
            try
            {
                var resultStatus =  await _EstimatedBudgetForExternalParticipationService.SendOtpAsync();
                return Json(new { success = resultStatus });
            }
            catch (Exception)
            {
                return Json(new { success = false });
            }
        }

        [IgnoreAction]
        [HttpPost]
        public async Task<IActionResult> ValidateOtp([FromBody] OtpValidationRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Code))
                return Json(new { success = false, message = "Invalid data." });

            bool isValid = await _EstimatedBudgetForExternalParticipationService.ValidateOtpAsync((int)request.Id, request.Code);

            if (isValid)
                return Json(new { success = true });

            return Json(new { success = false, message = "Incorrect OTP code." });
        }
        public async Task<IActionResult> AddEdit(int? id)
        {
            var vm = new EstimatedBudgetForExternalParticipationVM();
            if (id == 0 || id == null)
            {
                return View(vm);
            }
            var EstimatedBudgetForExternalParticipation = await _EstimatedBudgetForExternalParticipationService.GetByIdAsync(id.Value);
            if (EstimatedBudgetForExternalParticipation == null) return NotFound();

            vm = _mapper.Map<EstimatedBudgetForExternalParticipationVM>(EstimatedBudgetForExternalParticipation);

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEdit(EstimatedBudgetForExternalParticipationVM model)
        {
            var allDetails_Records = model.EstimatedBudgetForExternalParticipationDetails;

            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var entity = _mapper.Map<EstimatedBudgetForExternalParticipation>(model);

            if (model.Id == 0)
            {
                model.Id = await _EstimatedBudgetForExternalParticipationService.AddAsync(entity);
                await _hubContext.Clients.Groups("Manager")
                  .SendAsync("ReceiveNotification", new
                  {
                      Title = "",
                      Message = ""
                  });
                await _notificationService.SendNotificationToRoleAsync(
                      "تقدير موازنة خارجية جديد",
                      $"يوجد تقدير موازنة خارجية رقم {(model?.Id)} جاهزة للإعتماد",
                      2
                  );
                return RedirectToAction(nameof(Index)); // After Add New
            }
            else
            {
                await _EstimatedBudgetForExternalParticipationService.UpdateAsync(entity);
                await _EstimatedBudgetForExternalParticipationService.UpdateNewDetailsParticipationsType(allDetails_Records, model.Id);
            }

            return RedirectToAction(nameof(AddEdit), new { model.Id }); // After Edit 
        }

        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> Details(int? id)
        {
            var vm = new EstimatedBudgetForExternalParticipationVM();
            if (id == 0 || id == null)
            {
                return View(vm);
            }
            var EstimatedBudgetForExternalParticipation = await _EstimatedBudgetForExternalParticipationService.GetByIdAsync(id.Value);
            if (EstimatedBudgetForExternalParticipation == null) return NotFound();
            vm = _mapper.Map<EstimatedBudgetForExternalParticipationVM>(EstimatedBudgetForExternalParticipation);

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
            var vm = new EstimatedBudgetForExternalParticipationVM();
            if (id == 0 || id == null)
            {
                return View(vm);
            }
            var EstimatedBudgetForExternalParticipation = await _EstimatedBudgetForExternalParticipationService.GetByIdAsync(id.Value);
            if (EstimatedBudgetForExternalParticipation == null) return NotFound();
            vm = _mapper.Map<EstimatedBudgetForExternalParticipationVM>(EstimatedBudgetForExternalParticipation);

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _EstimatedBudgetForExternalParticipationService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> Print(string? searchTerm, DateOnly? dateFrom, DateOnly? dateTo, int page = 1, int pageSize = 50)
        {
            var allEstimatedBudgetForExternalParticipations = await _EstimatedBudgetForExternalParticipationService.GetAllAsync();

            var EstimatedBudgetForExternalParticipationVMs = _mapper.Map<List<EstimatedBudgetForExternalParticipationVM>>(allEstimatedBudgetForExternalParticipations).AsQueryable();


            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                EstimatedBudgetForExternalParticipationVMs = EstimatedBudgetForExternalParticipationVMs.Where(c =>
                    (!string.IsNullOrEmpty(c.ParticipatingTitle) && c.ParticipatingTitle.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                );
            }


            // ✅ Date filtering — replace `c.YourDateProperty` with your actual property
            if (dateFrom.HasValue)
            {
                EstimatedBudgetForExternalParticipationVMs = EstimatedBudgetForExternalParticipationVMs.Where(c => c.CreationDate >= dateFrom.Value);
            }
            if (dateTo.HasValue)
            {
                EstimatedBudgetForExternalParticipationVMs = EstimatedBudgetForExternalParticipationVMs.Where(c => c.CreationDate <= dateTo.Value);
            }
            EstimatedBudgetForExternalParticipationVMs = EstimatedBudgetForExternalParticipationVMs.OrderByDescending(x => x.Id);
            ViewBag.dateFrom = dateFrom?.ToString("yyyy-MM-dd");
            ViewBag.dateTo = dateTo?.ToString("yyyy-MM-dd");

            TempData["FromEstimatedBudgetForExternalParticipationIndex"] = "true";
            return View(EstimatedBudgetForExternalParticipationVMs);
        }

        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> createExcelReport_Download(string? searchTerm, DateOnly? dateFrom, DateOnly? dateTo, int page = 1, int pageSize = 50)
        {
            var allEstimatedBudgetForExternalParticipations = await _EstimatedBudgetForExternalParticipationService.GetAllAsync();

            var EstimatedBudgetForExternalParticipationVMs = _mapper.Map<List<EstimatedBudgetForExternalParticipationVM>>(allEstimatedBudgetForExternalParticipations).AsQueryable();


            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                EstimatedBudgetForExternalParticipationVMs = EstimatedBudgetForExternalParticipationVMs.Where(c =>
                    (!string.IsNullOrEmpty(c.ParticipatingTitle) && c.ParticipatingTitle.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                );
            }


            // ✅ Date filtering — replace `c.YourDateProperty` with your actual property
            if (dateFrom.HasValue)
            {
                EstimatedBudgetForExternalParticipationVMs = EstimatedBudgetForExternalParticipationVMs.Where(c => c.CreationDate >= dateFrom.Value);
            }
            if (dateTo.HasValue)
            {
                EstimatedBudgetForExternalParticipationVMs = EstimatedBudgetForExternalParticipationVMs.Where(c => c.CreationDate <= dateTo.Value);
            }
            EstimatedBudgetForExternalParticipationVMs = EstimatedBudgetForExternalParticipationVMs.OrderByDescending(x => x.Id);

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
                var allData_list = EstimatedBudgetForExternalParticipationVMs;
                var ListTitles = new List<string>
        {
            Resource1.ParticipatingTitle,Resource1.Regulator,Resource1.BudgetDate,Resource1.ParticipatingCountry,
        };
                if (allData_list != null || allData_list?.Count() > 0)
                {
                    var excelDataDTO = allData_list.Select(single => new ExcelDataDTO
                    {
                        t1 = single.ParticipatingTitle,
                        t2 = single.Regulator,
                        t3 = (single.CreationDate.HasValue) ? single.CreationDate.Value.ToString("d").Replace("/","-") : "",
                        t4 = single.ParticipatingCountry,
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
                    var fileExcelName = Resource1.EstimatedBudgetForExternalParticipationList;
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
    }
}
