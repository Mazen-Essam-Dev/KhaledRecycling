using Application.Helpers;
using Application.Interfaces.Admin;
using Application.Services.Admin;
using AutoMapper;
using Domain.DTOs;
using Domain.DTOs.Admin.CashExchangeBond;
using Domain.Entities.CashExchangeBond;
using Domain.Enums;
using Domain.Resources;
using FougeraClub.Areas.Admin.ViewModels.CashExchangeBond;
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
    public class CashExchangeBondController : Controller
    {
        private readonly ICashExchangeBondService _service;
        private readonly IMapper _mapper;
        private readonly IHubContext<Hub.NotificationHub> _hubContext;
        private readonly INotificationService _notificationService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<Infrastructure.Identity.ApplicationUser> _userManager;

        public CashExchangeBondController(
            ICashExchangeBondService monthlyAdministrativeReportService,
            IMapper mapper,
            INotificationService notificationService,
            IHttpContextAccessor httpContextAccessor,
            IHubContext<Hub.NotificationHub> hubContext,
            UserManager<Infrastructure.Identity.ApplicationUser> userManager)
        {
            _service = monthlyAdministrativeReportService;
            _mapper = mapper;
            _notificationService = notificationService;
            _hubContext = hubContext;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }
        [YesGet]
        public async Task<IActionResult> Index(string? searchTerm, DateOnly? dateFrom, DateOnly? dateTo, int page = 1, int pageSize = 50)
        {
            var lang = SessionHelper.GetCurrentLanguage();

            var allData = await _service.GetAllAsync();
            var cashExchangeBondVM = _mapper.Map<List<CashExchangeBondVM>>(allData).AsQueryable();


            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                cashExchangeBondVM = cashExchangeBondVM.Where(c =>
                    (!string.IsNullOrEmpty(c.AboutText) && c.AboutText.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(c.Code) && c.Code.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(c.PersonName) && c.PersonName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                );
            }

            // ✅ Date filtering — replace `c.YourDateProperty` with your actual property
            if (dateFrom.HasValue)
            {
                cashExchangeBondVM = cashExchangeBondVM.Where(c => c.Date >= dateFrom.Value);
            }
            if (dateTo.HasValue)
            {
                cashExchangeBondVM = cashExchangeBondVM.Where(c => c.Date <= dateTo.Value);
            }
            ViewBag.CountRecords = cashExchangeBondVM?.Count();

            var paginated = PaginatedList<CashExchangeBondVM>.Create(cashExchangeBondVM?.OrderByDescending(m => m.Id), page, pageSize, searchTerm);

            RoleNumber ValidateRoleNumber()
            {
                var roleNumber = _httpContextAccessor.HttpContext?.Session.GetInt32("RoleNumber");
                if (roleNumber != null)
                {
                    return (RoleNumber)roleNumber;
                }
                return RoleNumber.NormalUser;
            }

            var IsUserLogedInISManager = ValidateRoleNumber() == RoleNumber.Manager;

            paginated.IsUserLogedInISManager = IsUserLogedInISManager;

            ViewBag.dateFrom = dateFrom?.ToString("yyyy-MM-dd");
            ViewBag.dateTo = dateTo?.ToString("yyyy-MM-dd");
            // Calling from Ajax Return PartialView
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_ListPartial", paginated);
            }

            return View(paginated);
        }


        public async Task<IActionResult> AddEdit(int? id)
        {
            var vm = new CashExchangeBondVM();

            if (id.HasValue && id.Value != 0)
            {
                var item = await _service.GetByIdAsync(id.Value);
                if (item == null) return NotFound();
                vm = _mapper.Map<CashExchangeBondVM>(item);
            }
            return View(vm);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEdit(CashExchangeBondVM model)
        {
            var isMonthRegistedBefore = await _service.CheckIsMonthRegistedBefore(model.Id, model.Date);

            if (!ModelState.IsValid /*|| isMonthRegistedBefore*/)
            {

                return View(model);
            }

            var entity = _mapper.Map<CashExchangeBond>(model);

            if (model.Id == 0)
            {
                model.Id = await _service.AddAsync(entity);

                await _hubContext.Clients.Groups("Accountant")
     .SendAsync("ReceiveNotification", new
     {
         Title = "",
         Message = ""
     });
                await _notificationService.SendNotificationToRoleAsync(

      "سند صرف نقدي جديد",
      $"يوجد سند صرف نقدي رقم {model.Code} جاهز للإعتماد",
      4
  );
                return RedirectToAction(nameof(Index)); // After Add New

            }
            else // Edit
            {
                await _hubContext.Clients.Groups("Accountant")
                .SendAsync("ReceiveNotification", new
                {
                    Title = "",
                    Message = ""
                });

                await _notificationService.SendNotificationToRoleAsync(
                    "سند صرف نقدي تم تحديثه",
                    $"يوجد سند صرف نقدي رقم {model.Code} تم تحديثه جاهز للإعتماد",
                    (int)RoleNumber.Accountant
                );
                await _service.UpdateAsync(entity);
            }

            return RedirectToAction(nameof(AddEdit), new { model.Id }); // After Edit 
        }

        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> Details(int? id)
        {
            ViewBag.arMoney = "";
            ViewBag.enMoney = "";
            if (id.HasValue && id.Value != 0)
            {
                var item = await _service.GetByIdAsync(id.Value);
                if (item == null) return NotFound();
                var vm = _mapper.Map<CashExchangeBondVM>(item);
                var allmoney = vm.Money;
                ViewBag.arMoney = TafqeetHelper.Tafqeet(allmoney.Value, CurrencyHelper.AED_Main_Ar, CurrencyHelper.AED_Sub_Ar);
                ViewBag.enMoney = TafqeetHelper.Tafqeet(allmoney.Value, CurrencyHelper.AED_Main_En, CurrencyHelper.AED_Sub_En);
                return View(vm);
            }
            else
            {
                return NotFound();
            }
        }

        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> CashExchangeBondReceipt(int? id)
        {
            ViewBag.arMoney = "";
            ViewBag.enMoney = "";
            if (id.HasValue && id.Value != 0)
            {
                var item = await _service.GetByIdAsync(id.Value);
                if (item == null) return NotFound();
                var vm = _mapper.Map<CashExchangeBondVM>(item);
                var allmoney = vm.Money;
                ViewBag.arMoney = TafqeetHelper.Tafqeet(allmoney.Value, CurrencyHelper.AED_Main_Ar, CurrencyHelper.AED_Sub_Ar);
                ViewBag.enMoney = TafqeetHelper.Tafqeet(allmoney.Value, CurrencyHelper.AED_Main_En, CurrencyHelper.AED_Sub_En);

                // Fetch and set AccountantFullName
                if (vm.AccountantSigniture != null && !string.IsNullOrEmpty(vm.AccountantSigniture.UserId))
                {
                    var accountant = await _userManager.FindByIdAsync(vm.AccountantSigniture.UserId);
                    vm.AccountantFullName = accountant?.FullNameAr ?? accountant?.FullNameEn ?? accountant?.UserName ?? accountant?.Email ?? "";
                }
                // Fetch and set ManagerFullName
                if (vm.ManagerSignature != null && !string.IsNullOrEmpty(vm.ManagerSignature.UserId))
                {
                    var manager = await _userManager.FindByIdAsync(vm.ManagerSignature.UserId);
                    vm.ManagerFullName = manager?.FullNameAr ?? manager?.FullNameEn ?? manager?.UserName ?? manager?.Email ?? "";
                }

                return View(vm);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> Print(string? searchTerm, DateOnly? dateFrom, DateOnly? dateTo, int page = 1, int pageSize = 50)
        {
            var lang = SessionHelper.GetCurrentLanguage();

            var allData = await _service.GetAllAsync();
            var cashExchangeBondVM = _mapper.Map<List<CashExchangeBondVM>>(allData).AsQueryable();


            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                cashExchangeBondVM = cashExchangeBondVM.Where(c =>
                    (!string.IsNullOrEmpty(c.AboutText) && c.AboutText.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(c.Code) && c.Code.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(c.PersonName) && c.PersonName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                );
            }

            // ✅ Date filtering — replace `c.YourDateProperty` with your actual property
            if (dateFrom.HasValue)
            {
                cashExchangeBondVM = cashExchangeBondVM.Where(c => c.Date >= dateFrom.Value);
            }
            if (dateTo.HasValue)
            {
                cashExchangeBondVM = cashExchangeBondVM.Where(c => c.Date <= dateTo.Value);
            }
            ViewBag.CountRecords = cashExchangeBondVM?.Count();

            ViewBag.dateFrom = dateFrom?.ToString("yyyy-MM-dd");
            ViewBag.dateTo = dateTo?.ToString("yyyy-MM-dd");


            return View(cashExchangeBondVM?.OrderByDescending(m => m.Id));
        }

        [IgnoreAction]
        [NoLogging]
        [HttpPost]
        public async Task<IActionResult> CheckDate(int id, DateOnly date)
        {

            // Example logic: only allow future dates
            var isMonthRegistedBefore = await _service.CheckIsMonthRegistedBefore(id, date);

            return Json(isMonthRegistedBefore);
        }

        [IgnoreAction]
        [NoLogging]
        [HttpPost]
        public IActionResult MoneyText(decimal? money)
        {
            if (money == null)
            {
                var responseNull = new
                {
                    arMoney = "",
                    enMoney = "",
                    status = false,
                };
                return Json(responseNull);
            }

            // Example 
            var ar = TafqeetHelper.Tafqeet(money.Value, CurrencyHelper.AED_Main_Ar, CurrencyHelper.AED_Sub_Ar);
            var en = TafqeetHelper.Tafqeet(money.Value, CurrencyHelper.AED_Main_En, CurrencyHelper.AED_Sub_En);
            bool status = true;
            var response = new
            {
                arMoney = ar,
                enMoney = en,
                status = true,
            };
            return Json(response);
        }

        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> createExcelReport_Download(string? searchTerm, DateOnly? dateFrom, DateOnly? dateTo, int page = 1, int pageSize = 50)
        {
            var lang = SessionHelper.GetCurrentLanguage();

            var allData = await _service.GetAllAsync();
            var cashExchangeBondVM = _mapper.Map<List<CashExchangeBondVM>>(allData).AsQueryable();


            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                cashExchangeBondVM = cashExchangeBondVM.Where(c =>
                    (!string.IsNullOrEmpty(c.AboutText) && c.AboutText.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(c.Code) && c.Code.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(c.PersonName) && c.PersonName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                );
            }

            // ✅ Date filtering — replace `c.YourDateProperty` with your actual property
            if (dateFrom.HasValue)
            {
                cashExchangeBondVM = cashExchangeBondVM.Where(c => c.Date >= dateFrom.Value);
            }
            if (dateTo.HasValue)
            {
                cashExchangeBondVM = cashExchangeBondVM.Where(c => c.Date <= dateTo.Value);
            }
            ViewBag.CountRecords = cashExchangeBondVM?.Count();

            ViewBag.dateFrom = dateFrom?.ToString("yyyy-MM-dd");
            ViewBag.dateTo = dateTo?.ToString("yyyy-MM-dd");


            // ---- End Get Data As Print

            var boolStatus = false;
            byte[]? fileBytes = null;
            var pathNewFile = "";
            try
            {
                lang = SessionHelper.GetCurrentLanguage();
                var allData_list = cashExchangeBondVM?.OrderByDescending(m => m.Id);
                var ListTitles = new List<string>
                {
                    @Resource1.DocumentNo2,@Resource1.Money,@Resource2.RequestDate
                };
                if (allData_list != null || allData_list?.Count() > 0)
                {
                    var excelDataDTO = allData_list.Select(single => new ExcelDataDTO
                    {
                        t1 = single.Code,
                        t2 = single.Money.HasValue ? single.Money.Value.ToString("0.00") : "",
                        t3 = single.Date.HasValue ? single.Date.Value.ToString("d").Replace("/","-") : "",
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
                    var fileExcelName = Resource1.CashExchangeBondList;
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
        public async Task<IActionResult> SendOtp(int id, string role)
        {
            // role: "trainer" or "manager"
            var result = await _service.SendOtpAsync(id, role);
            return Json(new { success = result });
        }
        [IgnoreAction]
        [HttpPost]
        public async Task<IActionResult> ValidateOtp([FromBody] FougeraClub.Areas.Admin.ViewModels.SMS.OtpValidationRequest request)
        {

            // request: { id, code, role }
            var result = await _service.ValidateOtpAsync((int)request.Id, request.Code, request.Role, User);

            var singleReport = await _service.GetByIdAsync((int)request.Id);

            if (result.success == true)
            {
                if (request.Role == "accountant")
                {
                    await _hubContext.Clients.Groups("Manager")
                        .SendAsync("ReceiveNotification", new
                        {
                            Title = "",
                            Message = ""
                        });
                    await _notificationService.SendNotificationToRoleAsync(
                          "سند صرف نقدي جديد",
                          $"يوجد سند صرف نقدي رقم {(singleReport?.Code) ?? "جديد"} جاهز للإعتماد",
                          2
                      );
                }
            }
            return Json(new { success = result.success, message = result.message });
        }

        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> GetCashExchangeBondAttachments(int id)
        {
            var attachmentsDTO = await _service.GetAttachmentsAsync(id);
            var model = _mapper.Map<CashExchangeBondAttachmentsVM>(attachmentsDTO);
            return PartialView("_CashExchangeBondAttachmentsModal", model);
        }

        [IgnoreAction]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadCashExchangeBondFiles(CashExchangeBondAttachmentsVM model)
        {
            if (model == null)
                return Json(new { success = false, message = "Files uploaded not done" });
            var dto = _mapper.Map<CashExchangeBondAttachmentsDTO>(model);
            await _service.UploadAttachmentsAsync(dto);

            return RedirectToAction("Index");
        }
    }
}