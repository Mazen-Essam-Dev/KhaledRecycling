using Application.Helpers;
using Application.Interfaces.Admin;
using Application.Services.Admin;
using AutoMapper;
using Domain.DTOs;
using Domain.DTOs.Admin.CashDisbursementVoucher;
using Domain.Entities.CashDisbursementVoucher;
using Domain.Enums;
using Domain.Resources;
using FougeraClub.Areas.Admin.ViewModels.CashDisbursementVoucher;
using FougeraClub.Areas.Admin.ViewModels.SMS;
using FougeraClub.Attributes;
using FougeraClub.Helpers;
using FougeraClub.Middelware;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Globalization;


namespace FougeraClub.Areas.Admin.Controllers
{
    [AdminAuthorize]
    [Area("Admin")]
    public class CashDisbursementVoucherController : Controller
    {
        private readonly ICashDisbursementVoucherService _service;
        private readonly IMapper _mapper;
        private readonly IHubContext<Hub.NotificationHub> _hubContext;
        private readonly INotificationService _notificationService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<Infrastructure.Identity.ApplicationUser> _userManager;

        public CashDisbursementVoucherController(
            ICashDisbursementVoucherService CashDisbursementVoucherService,
            IMapper mapper,
            IHubContext<Hub.NotificationHub> hubContext,
            INotificationService notificationService,
            IHttpContextAccessor httpContextAccessor,
            UserManager<Infrastructure.Identity.ApplicationUser> userManager)
        {
            _service = CashDisbursementVoucherService;
            _mapper = mapper;
            _hubContext = hubContext;
            _notificationService = notificationService;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
        }
        [YesGet]
        public async Task<IActionResult> Index(string? searchTerm, DateOnly? dateFrom, DateOnly? dateTo, int page = 1, int pageSize = 50)
        {
            var lang = SessionHelper.GetCurrentLanguage();
            var CashDisbursementVouchers = await _service.GetAllAsync();

            var model = _mapper.Map<List<CashDisbursementVoucherVM>>(CashDisbursementVouchers).AsQueryable();

            foreach (var item in model)
            {
                var result1 = await _service.GetExchangeProofSignaturesAsync(item.Id);

                if (result1.Item1 != 0 && result1.Item2 != 0)
                    item.ExchangeProofApprovalDone = true;
                else
                    item.ExchangeProofApprovalDone = false;

                item.AcknowledgmentReceiptApprovalDone = await _service.GetAcknowledgmentReceiptSignaturesAsync(item.Id);
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                if (int.TryParse(searchTerm, out int documentNo))
                {
                    model = model.Where(c => c.DocumentNo == documentNo);
                }
            }
            if (dateFrom.HasValue)
            {
                model = model.Where(c => c.Date >= dateFrom.Value);
            }
            if (dateTo.HasValue)
            {
                model = model.Where(c => c.Date <= dateTo.Value);
            }

            var paginated = PaginatedList<CashDisbursementVoucherVM>.Create(model.OrderByDescending(m => m.Id), page, pageSize);

            // Calling from Ajax Return PartialView
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_ListPartial", paginated);
            }

            return View(paginated);
        }
        public async Task<IActionResult> AddEdit(int? id)
        {
            var vm = new CashDisbursementVoucherVM();

            if (id.HasValue && id.Value != 0)
            {
                var item = await _service.GetByIdAsync(id.Value);
                if (item == null) return NotFound();
                vm = _mapper.Map<CashDisbursementVoucherVM>(item);
                vm.AcknowledgmentReceiptApprovalDone = await _service.GetAcknowledgmentReceiptSignaturesAsync(id.Value);
            }

            // If creating a new voucher, auto-generate the next DocumentNo (incremental)
            if ((!id.HasValue || id.Value == 0) && (vm.DocumentNo == null || vm.DocumentNo == 0))
            {
                vm.DocumentNo = await GetNextVoucherDocumentNoAsync();
            }

            vm.TypeEnumList = SelectListHelper.GetEnumSelectList<CashDisbursementVoucherType>();
            return View(vm);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEdit(CashDisbursementVoucherVM model)
        {

            if (!ModelState.IsValid)
            {
                model.TypeEnumList = SelectListHelper.GetEnumSelectList<CashDisbursementVoucherType>();
                return View(model);
            }

            var entity = _mapper.Map<CashDisbursementVoucher>(model);

            if (model.Id == 0)
            {
                // Auto-generate DocumentNo if not provided
                if ((!entity.DocumentNo.HasValue || entity.DocumentNo.Value == 0))
                {
                    entity.DocumentNo = await GetNextVoucherDocumentNoAsync();
                    model.DocumentNo = entity.DocumentNo;
                }
                model.Id = await _service.AddAsync(entity);
                await _hubContext.Clients.Groups("Accountant")
     .SendAsync("ReceiveNotification", new
     {
         Title = "",
         Message = ""
     });
                await _notificationService.SendNotificationToRoleAsync(

      "طلب صرف  جديد",
      $"يوجد طلب صرف رقم {model.DocumentNo} جاهز للإعتماد",
      (int)RoleNumber.Accountant
  );
                return RedirectToAction(nameof(Index)); // After Add New
            }
            else
            {
                await _hubContext.Clients.Groups("Accountant")
                    .SendAsync("ReceiveNotification", new
                    {
                    Title = "",
                    Message = ""
                    });
                await _notificationService.SendNotificationToRoleAsync(

                        "طلب صرف تم تحديثه",
                        $"يوجد طلب صرف رقم {model.DocumentNo} تم تحديثه جاهز للإعتماد",
                        (int)RoleNumber.Accountant
                    );
                await _service.UpdateAsync(entity);
            }

            return RedirectToAction(nameof(AddEdit), new { model.Id }); // After Edit 
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }


        #region ExchangeProof
        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> PrintCashDisbursementRequest(int? id)
        {
            var vm = new CashDisbursementVoucherVM();

            if (id.HasValue && id.Value != 0)
            {
                var item = await _service.GetByIdAsync(id.Value);
                if (item == null) return NotFound();
                vm = _mapper.Map<CashDisbursementVoucherVM>(item);
                if (vm.Type != null)
                    vm.TypeText = EnumHelper.GetDisplayName((CashDisbursementVoucherType)vm.Type);
                // Fetch and set ManagerFullName
                if (vm.DisbursementRequestSignature != null && !string.IsNullOrEmpty(vm.DisbursementRequestSignature.UserId))
                {
                    var manager = await _userManager.FindByIdAsync(vm.DisbursementRequestSignature.UserId);
                    vm.ManagerFullName = manager?.FullNameAr ?? manager?.FullNameEn ?? manager?.UserName ?? manager?.Email ?? "";
                }
                if (vm.DisbursementRequestAccountantSignature != null && !string.IsNullOrEmpty(vm.DisbursementRequestAccountantSignature.UserId))
                {
                    var Accountant = await _userManager.FindByIdAsync(vm.DisbursementRequestAccountantSignature.UserId);
                    vm.AccountantFullName = Accountant?.FullNameAr ?? Accountant?.FullNameEn ?? Accountant?.UserName ?? Accountant?.Email ?? "";
                }
            }
            return View(vm);
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
        public async Task<IActionResult> CashDisbursementRequestValidateOtp([FromBody] OtpValidationRequest request)
        {
            // request: { id, code, role }
            var result = await _service.DisbursementRequestValidateOtpAsync((int)request.Id, request.Code, request.Role, User);
            var report = await _service.GetByIdAsync((int)request.Id);

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

                      "طلب صرف  جديد",
                      $"يوجد طلب صرف رقم {report?.DocumentNo} جديد جاهز للإعتماد",
                      (int)RoleNumber.Manager
                  );
                    }
                    else
                    {
                                await _hubContext.Clients.Groups("Accountant")
                .SendAsync("ReceiveNotification", new
                {
                    Title = "",
                    Message = ""
                });
                                await _notificationService.SendNotificationToRoleAsync(

                      "سند صرف  جديد",
                      $"يوجد سند صرف رقم {report?.DocumentNo} جديد جاهز للإعتماد",
                      (int)RoleNumber.Accountant
                    );

                }
        }
            return Json(new { success = result.success, message = result.message });
        }


        [IgnoreAction]
        [HttpPost]
        public async Task<IActionResult> ExchangeProofRequestValidateOtp([FromBody] OtpValidationRequest request)
        {
            // request: { id, code, role }
            var result = await _service.ExchangeProofValidateOtpAsync((int)request.Id, request.Code, request.Role, User);
            var report = await _service.GetExchangeProofByIdAsyncSingle((int)request.Id);

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

                  "سند صرف  جديد",
                  $"يوجد سند صرف رقم {report?.DocumentNo} جديد جاهز للإعتماد",
                  2
              );
                        }
                        else
                        {
                            await _hubContext.Clients.Groups("Accountant")
            .SendAsync("ReceiveNotification", new
            {
                Title = "",
                Message = ""
            });
                            await _notificationService.SendNotificationToRoleAsync(

                  "اقرار استلام  جديد",
                  $"يوجد اقرار استلام رقم {report?.DocumentNo} جديد جاهز للإعتماد",
                  4
              );

                        }
        }
            return Json(new { success = result.success, message = result.message });
        }
        [IgnoreAction]
        [HttpPost]
        public async Task<IActionResult> AcknowledgmentReceiptRequestValidateOtp([FromBody] OtpValidationRequest request)
        {
            // request: { id, code, role }
            var result = await _service.AcknowledgmentReceiptValidateOtpAsync((int)request.Id, request.Code, request.Role, User);
            return Json(new { success = result.success, message = result.message });
        }
        [IgnoreAction]
        public async Task<IActionResult> ExchangeProof(int cashDisbursementVoucherId)
        {
            var vm = new ExchangeProofVM();
            var item = await _service.GetExchangeProofByIdAsync(cashDisbursementVoucherId);
            if (item != null)
            {
                vm = _mapper.Map<ExchangeProofVM>(item);
                // Fetch and set AccountantFullName
                if (vm.AccountantSigniture != null && !string.IsNullOrEmpty(vm.AccountantSigniture.UserId))
                {
                    var accountant = await _userManager.FindByIdAsync(vm.AccountantSigniture.UserId);
                    vm.AccountantFullName = accountant?.FullNameAr ?? accountant?.FullNameEn ?? accountant?.UserName ?? accountant?.Email ?? "";
                }
                // Fetch and set ManagerFullName
                if (vm.ManagerSigniture != null && !string.IsNullOrEmpty(vm.ManagerSigniture.UserId))
                {
                    var manager = await _userManager.FindByIdAsync(vm.ManagerSigniture.UserId);
                    vm.ManagerFullName = manager?.FullNameAr ?? manager?.FullNameEn ?? manager?.UserName ?? manager?.Email ?? "";
                }
            }
            else
            {
                vm.CashDisbursementVoucherId = cashDisbursementVoucherId;
                vm.DocumentNo = await _service.GetLastExchangeProofDocumentNo();
            }

            if (string.IsNullOrWhiteSpace(vm.Being) || string.IsNullOrWhiteSpace(vm.Bank) || string.IsNullOrWhiteSpace(vm.By) || string.IsNullOrWhiteSpace(vm.ItWas) ||
                string.IsNullOrWhiteSpace(vm.BasedOn) || vm.Date == null /*||  string.IsNullOrWhiteSpace(vm.Amount)*/ || !(vm.DocumentNo != null)
               )
            {
                vm.isSavedFull = false;
            }
            else
                vm.isSavedFull = true;

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

            vm.IsUserLogedInISManager = IsUserLogedInISManager;
            return View(vm);
        }
        [HttpPost]
        [IgnoreAction]
        public async Task<IActionResult> ExchangeProof(ExchangeProofVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var entity = _mapper.Map<ExchangeProof>(model);

            if (model.Id == 0)
            {
                await _hubContext.Clients.Groups("Accountant")
                .SendAsync("ReceiveNotification", new
                {
                    Title = "",
                    Message = ""
                });

                // Auto-generate DocumentNo if not set
                if (!entity.DocumentNo.HasValue || entity.DocumentNo.Value == 0)
                {
                    entity.DocumentNo = await _service.GetLastExchangeProofDocumentNo();
                }

                await _notificationService.SendNotificationToRoleAsync(
                    "سند صرف جديد",
                    $"يوجد سند صرف رقم {entity.DocumentNo} جديد جاهز للإعتماد",
                    (int)RoleNumber.Accountant
                );
                await _service.AddAsync(entity);
            }
            else
            {
                await _hubContext.Clients.Groups("Accountant")
                .SendAsync("ReceiveNotification", new
                {
                    Title = "",
                    Message = ""
                });

                await _notificationService.SendNotificationToRoleAsync(
                    "سند صرف تم تحديثه",
                    $"يوجد سند صرف رقم {model.DocumentNo} تم تحديثه جاهز للإعتماد",
                    (int)RoleNumber.Accountant
                );
                var exist = await _service.GetExchangeProofByIdAsync(entity.Id);
                entity.DocumentNo = exist != null ? exist.DocumentNo :entity.DocumentNo;
                await _service.UpdateAsync(entity);
            }
            return RedirectToAction(nameof(ExchangeProof), new { model.CashDisbursementVoucherId });
        }
        #endregion

        #region AcknowledgmentReceipt
        [IgnoreAction]
        public async Task<IActionResult> AcknowledgmentReceipt(int cashDisbursementVoucherId)
        {
            var vm = new AcknowledgmentReceiptVM();
            var item = await _service.GetAcknowledgmentReceiptByIdAsync(cashDisbursementVoucherId);
            if (item != null)
            {
                vm = _mapper.Map<AcknowledgmentReceiptVM>(item);
                // Fetch and set AccountantFullName
                if (vm.AccountantSigniture != null && !string.IsNullOrEmpty(vm.AccountantSigniture.UserId))
                {
                    var accountant = await _userManager.FindByIdAsync(vm.AccountantSigniture.UserId);
                    vm.AccountantFullName = accountant?.FullNameAr ?? accountant?.FullNameEn ?? accountant?.UserName ?? accountant?.Email ?? "";
                }
            }
            else
            {
                vm.CashDisbursementVoucherId = cashDisbursementVoucherId;
                // Set default value for new records
                vm.AcknowledgmentText = "نادى الفجيرة العلمى بتقديم مستندات المبلغ الذى استلمته من حكومة الفجيرة - النادى العلمى فى مده اقصاها انهاء خدمات من تاريخ استلامى لمبلغ العهدة";
                vm.HeaderDate = DateOnly.FromDateTime(AppDubaiTime.Now);
                // Get current month name in Arabic
                var monthNames = new[] { "يناير", "فبراير", "مارس", "أبريل", "مايو", "يونيو", "يوليو", "أغسطس", "سبتمبر", "أكتوبر", "نوفمبر", "ديسمبر" };
                vm.HeaderMonth = monthNames[AppDubaiTime.Now.Month - 1];
            }
            
            // Set default value for older records that don't have text
            if (string.IsNullOrWhiteSpace(vm.AcknowledgmentText))
            {
                vm.AcknowledgmentText = "نادى الفجيرة العلمى بتقديم مستندات المبلغ الذى استلمته من حكومة الفجيرة - النادى العلمى فى مده اقصاها انهاء خدمات من تاريخ استلامى لمبلغ العهدة";
            }
            
            if (string.IsNullOrWhiteSpace(vm.Title)|| string.IsNullOrWhiteSpace(vm.ThisTo) || string.IsNullOrWhiteSpace(vm.Recived) || string.IsNullOrWhiteSpace(vm.Cheque) ||
                string.IsNullOrWhiteSpace(vm.Bank) || string.IsNullOrWhiteSpace(vm.Being) || vm.Date==null || string.IsNullOrWhiteSpace(vm.Amount) || !(vm.PaymentVoucherNo > 0)
               )
            {
                vm.isSavedFull = false;
            }else
                vm.isSavedFull = true;

            return View(vm);
        }
        [HttpPost]
        [IgnoreAction]
        public async Task<IActionResult> AcknowledgmentReceipt(AcknowledgmentReceiptVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var entity = _mapper.Map<AcknowledgmentReceipt>(model);

            if (model.Id == 0)
            {
                await _service.AddAsync(entity);
            }
            else
            {
                await _service.UpdateAsync(entity);
            }
            return RedirectToAction(nameof(AcknowledgmentReceipt), new { model.CashDisbursementVoucherId });
        }
        #endregion

        #region Print & Export
        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> Print(string? searchTerm, DateOnly? dateFrom, DateOnly? dateTo)
        {
            var CashDisbursementVouchers = await _service.GetAllAsync();

            var model = _mapper.Map<List<CashDisbursementVoucherVM>>(CashDisbursementVouchers).AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                if (int.TryParse(searchTerm, out int documentNo))
                {
                    model = model.Where(c => c.DocumentNo == documentNo);
                }
            }
            if (dateFrom.HasValue)
            {
                model = model.Where(c => c.Date >= dateFrom.Value);
            }
            if (dateTo.HasValue)
            {
                model = model.Where(c => c.Date <= dateTo.Value);
            }

            return View(model.OrderByDescending(m => m.Id));
        }
        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> createExcelReport_Download(string? searchTerm, DateOnly? dateFrom, DateOnly? dateTo)
        {
            var CashDisbursementVouchers = await _service.GetAllAsync();

            var model = _mapper.Map<List<CashDisbursementVoucherVM>>(CashDisbursementVouchers).AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                if (int.TryParse(searchTerm, out int documentNo))
                {
                    model = model.Where(c => c.DocumentNo == documentNo);
                }
            }
            if (dateFrom.HasValue)
            {
                model = model.Where(c => c.Date >= dateFrom.Value);
            }
            if (dateTo.HasValue)
            {
                model = model.Where(c => c.Date <= dateTo.Value);
            }

            // ---- End Get Data As Print

            var boolStatus = false;
            byte[]? fileBytes = null;
            var pathNewFile = "";
            try
            {
                var lang = SessionHelper.GetCurrentLanguage();
                var allData_list = model.OrderByDescending(m => m.Id);
                var ListTitles = new List<string>
                {
                    Resource1.BondNo1,Resource2.Amount,Resource2.RequestDate
                };
                if (allData_list != null || allData_list?.Count() > 0)
                {
                    var excelDataDTO = allData_list.Select(single => new ExcelDataDTO
                    {
                        t1 = single.DocumentNo.HasValue ? single.DocumentNo.Value : "",
                        t2 = single.TotalAmount.HasValue ? single.TotalAmount.Value : "",
                        t3 = single.Date.HasValue ? single.Date.Value.ToString("d").Replace("/","-").ToString() : "",
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
                    var fileExcelName = Resource2.CashDisbursementVoucherList;
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

        #region Attachments
        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> GetCashDisbursementVoucherAttachments(int id)
        {
            var attachments = await _service.GetAttachmentsAsync(id);
            var attachmentsVM = _mapper.Map<List<CashDisbursementVoucherAttachmentVM>>(attachments);
            var model = new CashDisbursementVoucherAttachmentsVM
            {
                CashDisbursementVoucherId = id,
                Attachments = attachmentsVM
            };
            return PartialView("_CashDisbursementVoucherAttachmentsModal", model);
        }

        [IgnoreAction]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadCashDisbursementVoucherFiles(CashDisbursementVoucherAttachmentsVM model)
        {
            if (model == null)
                return Json(new { success = false, message = "Files uploaded not done" });
            var dto = _mapper.Map<CashDisbursementVoucherAttachmentsDTO>(model);
            await _service.UploadAttachmentsAsync(dto);

            return RedirectToAction("Index");
        }
        #endregion

        // Helper: compute next incremental DocumentNo based on existing CashDisbursementVoucher DocumentNo values
        private async Task<int?> GetNextVoucherDocumentNoAsync()
        {
            var all = await _service.GetAllAsync();
            if (all == null || !all.Any())
                return 1;

            var max = all.Max(x => x.DocumentNo ?? 0);
            return max + 1;
        }
    }
}