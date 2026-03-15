using Application.Helpers;
using Application.Interfaces.Admin;
using Application.Services.Admin;
using AutoMapper;
using Domain.DTOs;
using Domain.DTOs.Admin.PurchaseOrder;
using Domain.Entities.PurchaseOrder;
using Domain.Resources;
using FougeraClub.Areas.Admin.ViewModels.PurchaseOrder;
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
    public class PurchaseOrderController : Controller
    {
        private readonly IPurchaseOrderService _service;
        private readonly IMapper _mapper;
        private readonly IHubContext<Hub.NotificationHub> _hubContext;
        private readonly INotificationService _notificationService;
        private readonly UserManager<Infrastructure.Identity.ApplicationUser> _userManager;

        public PurchaseOrderController(
            IPurchaseOrderService monthlyAdministrativeReportService,
            IMapper mapper,
            IHubContext<Hub.NotificationHub> hubContext,
            INotificationService notificationService,
            UserManager<Infrastructure.Identity.ApplicationUser> userManager)
        {
            _service = monthlyAdministrativeReportService;
            _mapper = mapper;
            _hubContext = hubContext;
            _notificationService = notificationService;
            _userManager = userManager;
        }
        [YesGet]
        public async Task<IActionResult> Index(int? selectedSupplier, DateOnly? dateFrom, DateOnly? dateTo, int page = 1, int pageSize = 50)
        {
            var allPurchaseOrders = await _service.GetAllAsync();

            if (selectedSupplier != null && selectedSupplier.HasValue)
            {
                allPurchaseOrders = allPurchaseOrders.Where(c => c.SupplierId == selectedSupplier.Value);
            }

            // ✅ Date filtering — replace `c.YourDateProperty` with your actual property
            if (dateFrom.HasValue)
            {
                allPurchaseOrders = allPurchaseOrders.Where(c => c.Date >= dateFrom.Value);
            }
            if (dateTo.HasValue)
            {
                allPurchaseOrders = allPurchaseOrders.Where(c => c.Date <= dateTo.Value);
            }

            var suppliers = await _service.GetAllSuppliersAsync();
            ViewBag.suppliers = suppliers;
            var allPurchaseOrderVM = _mapper.Map<IEnumerable<PurchaseOrderVM>>(allPurchaseOrders);
            var paginated = PaginatedList<PurchaseOrderVM>.Create(allPurchaseOrderVM.ToList(), page, pageSize, null);
            ViewBag.SelectedType = selectedSupplier;
            ViewBag.dateFrom = dateFrom?.ToString("yyyy-MM-dd");
            ViewBag.dateTo = dateTo?.ToString("yyyy-MM-dd");

            TempData["FromCarIndex"] = "true";

            // Calling from Ajax Return PartialView
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_ListPartial", paginated);
            }

            return View(paginated);
        }

        public async Task<IActionResult> AddEdit(long? id)
        {
            var vm = new PurchaseOrderVM();
            vm.PurchaseOrderCode = await _service.GetNewCodeAsync();
            if (id.HasValue && id.Value != 0) //Edit
            {
                var purchaseOrder = await _service.GetByIdAsync(id.Value);
                if (purchaseOrder == null) return NotFound();
                vm = _mapper.Map<PurchaseOrderVM>(purchaseOrder);
            }

            var suppliers = await _service.GetAllSuppliersAsync();
            vm.suppliers = suppliers;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEdit(PurchaseOrderVM model)
        {

            if (!ModelState.IsValid)
            {
                var suppliers = await _service.GetAllSuppliersAsync();
                model.suppliers = suppliers;
                if (model.PurchaseOrderCode == null)
                    model.PurchaseOrderCode = await _service.GetNewCodeAsync();

                return View(model);
            }

            var entity = _mapper.Map<PurchaseOrder>(model);

            if (model.Id == 0)
            {
                model.Id = await _service.AddAsync(entity);
                await _hubContext.Clients.Groups("Manager")
   .SendAsync("ReceiveNotification", new
   {
       Title = "",
       Message = ""
   });
                await _notificationService.SendNotificationToRoleAsync(

      "أمر شراء جديد",
      $"يوجد أمر شراء رقم {model?.PurchaseOrderCode} جديد جاهز للإعتماد",
      2
  );
                return RedirectToAction(nameof(Index)); // After Add New
            }
            else
            {
                await _service.UpdateAsync(entity);
            }

            return RedirectToAction(nameof(AddEdit), new { model.Id }); // After Edit
        }

        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> Details(long? id)
        {
            if (id.HasValue && id.Value != 0)
            {
                var item = await _service.GetByIdAsync(id.Value);
                if (item == null) return NotFound();
                var vm = _mapper.Map<PurchaseOrderVM>(item);
                var suppliers = await _service.GetAllSuppliersAsync();
                vm.suppliers = suppliers;
                // Fetch manager full name from ApplicationUser using Signature.UserId
                if (vm.Signature != null && !string.IsNullOrEmpty(vm.Signature.UserId))
                {
                    var user = await _userManager.FindByIdAsync(vm.Signature.UserId);
                    vm.ManagerUserName = user?.FullNameAr ?? user?.FullNameEn ?? user?.UserName ?? user?.Email ?? "";
                }
                return View(vm);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(long id)
        {
            await _service.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> Print(int? selectedSupplier, DateOnly? dateFrom, DateOnly? dateTo)
        {
            var allPurchaseOrders = await _service.GetAllAsync();

            if (selectedSupplier != null && selectedSupplier.HasValue)
            {
                allPurchaseOrders = allPurchaseOrders.Where(c => c.SupplierId == selectedSupplier.Value);
            }

            // ✅ Date filtering — replace `c.YourDateProperty` with your actual property
            if (dateFrom.HasValue)
            {
                allPurchaseOrders = allPurchaseOrders.Where(c => c.Date >= dateFrom.Value);
            }
            if (dateTo.HasValue)
            {
                allPurchaseOrders = allPurchaseOrders.Where(c => c.Date <= dateTo.Value);
            }

            var suppliers = await _service.GetAllSuppliersAsync();
            ViewBag.suppliers = suppliers;
            var allPurchaseOrderVM = _mapper.Map<IEnumerable<PurchaseOrderVM>>(allPurchaseOrders);
            ViewBag.SelectedType = selectedSupplier;
            ViewBag.dateFrom = dateFrom?.ToString("yyyy-MM-dd");
            ViewBag.dateTo = dateTo?.ToString("yyyy-MM-dd");

            return View(allPurchaseOrderVM);
        }

        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> PrintDetails(long? id)
        {
            if (id.HasValue && id.Value != 0)
            {
                var item = await _service.GetByIdAsync(id.Value);
                if (item == null) return NotFound();
                var vm = _mapper.Map<PurchaseOrderVM>(item);
                var suppliers = await _service.GetAllSuppliersAsync();
                vm.suppliers = suppliers;
                return View(vm);
            }
            else
            {
                return NotFound();
            }
        }
        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> createExcelReport_Download(int? selectedSupplier, DateOnly? dateFrom, DateOnly? dateTo)
        {
            var allPurchaseOrders = await _service.GetAllAsync();

            if (selectedSupplier != null && selectedSupplier.HasValue)
            {
                allPurchaseOrders = allPurchaseOrders.Where(c => c.SupplierId == selectedSupplier.Value);
            }

            // ✅ Date filtering — replace `c.YourDateProperty` with your actual property
            if (dateFrom.HasValue)
            {
                allPurchaseOrders = allPurchaseOrders.Where(c => c.Date >= dateFrom.Value);
            }
            if (dateTo.HasValue)
            {
                allPurchaseOrders = allPurchaseOrders.Where(c => c.Date <= dateTo.Value);
            }

            var suppliers = await _service.GetAllSuppliersAsync();
            ViewBag.suppliers = suppliers;
            var allPurchaseOrderVM = _mapper.Map<IEnumerable<PurchaseOrderVM>>(allPurchaseOrders);
            ViewBag.SelectedType = selectedSupplier;

            // ---- End Get Data As Print

            var boolStatus = false;
            byte[]? fileBytes = null;
            var pathNewFile = "";
            try
            {
                var lang = SessionHelper.GetCurrentLanguage();
                var allData_list = allPurchaseOrderVM;
                var ListTitles = new List<string>
                {
                    Resource1.PurchaseOrderCode,Resource1.SupplierName,Resource1.Date
                };
                if (allData_list != null || allData_list?.Count() > 0)
                {
                    var excelDataDTO = allData_list.Select(single => new ExcelDataDTO
                    {
                        t1 = single.PurchaseOrderCode,
                        t2 = (suppliers?.Where(x => single.SupplierId == x.Id)?.FirstOrDefault() != null) ? (lang == "ar" ? suppliers?.Where(x => single.SupplierId == x.Id)?.FirstOrDefault()?.SupplierNameAr : suppliers?.Where(x => single.SupplierId == x.Id)?.FirstOrDefault()?.SupplierNameEn) : "",
                        t3 = (single.Date.HasValue ? (lang == "ar" ? single.Date.Value.ToString("d")?.Replace("/","-") : single.Date.Value.ToString("d")?.Replace("/","-")) : ""),
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
                    var fileExcelName = Resource1.PurchaseOrderList;
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

        #region sms approval
        [IgnoreAction]
        [NoLogging]
        [HttpPost]
        public async Task<IActionResult> SendOtp() // GetSignature
        {
            try
            {
                var status= await _service.SendOtpAsync();
                return Json(new { success = status });
            }
            catch (Exception ex)
            {
                return Json(new { success = false });
            }
        }


        [HttpPost]
        [IgnoreAction]
        public async Task<IActionResult> ValidateOtp([FromBody] OtpValidationRequest request)    // ValidateOTPSignature
        {
            if (request == null || string.IsNullOrEmpty(request.Code))
                return Json(new { success = false, message = "Invalid data." });

            bool isValid = await _service.ValidateOtpAsync(request.Id, request.Code);

            if (isValid)
                return Json(new { success = true });

            return Json(new { success = false, message = "Incorrect OTP code." });
        }

        #endregion

        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> GetPurchaseOrderAttachments(long id)
        {
            var attachmentsDTO = await _service.GetAttachmentsAsync(id);
            var model = _mapper.Map<PurchaseOrderAttachmentsVM>(attachmentsDTO);
            return PartialView("_PurchaseOrderAttachmentsModal", model);
        }

        [IgnoreAction]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadPurchaseOrderFiles(PurchaseOrderAttachmentsVM model)
        {
            if (model == null)
                return Json(new { success = false, message = "Files uploaded not done" });
            var dto = _mapper.Map<PurchaseOrderAttachmentsDTO>(model);
            await _service.UploadAttachmentsAsync(dto);

            return RedirectToAction("Index");
        }

    }
}