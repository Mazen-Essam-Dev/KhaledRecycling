using Application.Helpers;
using Application.Interfaces.Admin;
using Application.Services.Admin;
using AutoMapper;
using Domain.DTOs;
using Domain.Entities;
using Domain.Entities.quote;
using Domain.Enums;
using Domain.Resources;
using FougeraClub.Areas.Admin.ViewModels.quote;
using FougeraClub.Areas.Admin.ViewModels.quote.Details;
using FougeraClub.Areas.Admin.ViewModels.SMS;
using FougeraClub.Attributes;
using FougeraClub.Helpers;
using FougeraClub.Middelware;
using Infrastructure.Identity;
using Infrastructure.Repositories.InterfacesDB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;


namespace FougeraClub.Areas.Admin.Controllers
{
    [AdminAuthorize]
    [Area("Admin")]
    public class quoteController : Controller
    {
        private readonly IquoteService _service;
        private readonly ITrainerService _TrainerService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHubContext<Hub.NotificationHub> _hubContext;
        private readonly INotificationService _notificationService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<Infrastructure.Identity.ApplicationUser> _userManager;
        private readonly IAuthorizationService _authorizationService;


        public quoteController(
            IquoteService monthlyAdministrativeReportService,
            IMapper mapper,
            ITrainerService trainerService,
            IHubContext<Hub.NotificationHub> hubContext,
            IHttpContextAccessor httpContextAccessor,
            INotificationService notificationService,IUnitOfWork unitOfWork,
             IAuthorizationService authorizationService,
        UserManager<Infrastructure.Identity.ApplicationUser> userManager)
        {
            _service = monthlyAdministrativeReportService;
            _TrainerService = trainerService;
            _mapper = mapper;
            _hubContext = hubContext;
            _notificationService = notificationService;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _authorizationService = authorizationService;
        }

        [IgnoreAction]
        [NoLogging]
        public async Task<(string, int,int)> CheckLoggedUserIfTrainerAndReturnData()
        {
            // Get the UserEMail of User Logged in
            var User = _httpContextAccessor.HttpContext?.User;
            var UserEMail = User?.Identity?.Name;

            int ThisTrainerId = 0;
            int ThisTrainerDepartmentId = 0;
            if (UserEMail == null) return (" ", 0,0);
            var ThisUser = await _unitOfWork.Users.GetByIdAsync(x => x.UserName == UserEMail);
            if (ThisUser != null)
            {
                var ThisTrainer = await _unitOfWork.Trainers.GetByIdAsync(x => x.UserId == ThisUser.Id);
                if (ThisTrainer != null) {
                    ThisTrainerId = ThisTrainer.Id;
                    ThisTrainerDepartmentId = ThisTrainer.DepartmentId;
                }
            }
            if (ThisTrainerId > 0) return (ThisUser?.Id != null ? ThisUser.Id : " ", ThisTrainerId, ThisTrainerDepartmentId);

            return (ThisUser?.Id != null ? ThisUser.Id : " ", 0,0);
        }
        
        [IgnoreAction]
        public RoleNumber ValidateRoleNumber()
        {
            var roleNumber = _httpContextAccessor.HttpContext?.Session.GetInt32("RoleNumber");
            if (roleNumber != null)
            {
                return (RoleNumber)roleNumber;
            }
            return RoleNumber.NormalUser;
        }

        [IgnoreAction]
        public bool ValidatePermission(string controller, string action)
        {
            var policyName = $"{controller}.{action}";

            var user = _httpContextAccessor.HttpContext?.User;
            if (user == null || !user.Identity?.IsAuthenticated == true)
                return false;

            var result = _authorizationService.AuthorizeAsync(user, null, policyName).Result;
            return result.Succeeded;
        }

        [YesGet]
        public async Task<IActionResult> Index(string? searchTerm, DateOnly? dateFrom, DateOnly? dateTo, int page = 1, int pageSize = 50)
        {
            var addPermission = ValidatePermission("quote", "Add");
            var editPermission = ValidatePermission("quote", "Edit"); 
            var AddOREditPermission = addPermission || editPermission;

            var allquotes = await _service.GetAllAsync(AddOREditPermission);


            // ✅ Date filtering — replace `c.YourDateProperty` with your actual property
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                allquotes = allquotes.Where(c =>  (!string.IsNullOrEmpty(c.quoteCode) && c.quoteCode.Contains(searchTerm,StringComparison.OrdinalIgnoreCase))
                || (!string.IsNullOrEmpty(c.OrderText) && c.OrderText.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                );
            }
            if (dateFrom.HasValue)
            {
                allquotes = allquotes.Where(c => c.Date >= dateFrom.Value);
            }
            if (dateTo.HasValue)
            {
                allquotes = allquotes.Where(c => c.Date <= dateTo.Value);
            }

            var allquoteVM = _mapper.Map<IEnumerable<quoteVM>>(allquotes);
            var paginated = PaginatedList<quoteVM>.Create(allquoteVM.ToList(), page, pageSize, null);

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

        public async Task<IActionResult> AddEdit(int? id)
        {
            var vm = new quoteVM();

            vm.quoteCode = await _service.GetNewCodeAsync();
            if (id.HasValue && id.Value != 0) //Edit
            {
                var quote = await _service.GetByIdAsync(id.Value);
                if (quote == null) return NotFound();
                vm = _mapper.Map<quoteVM>(quote);
            }         

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEdit(quoteVM model)
        {
            (string userLoggedInId, int TrainerId, int DepartmentId) = await CheckLoggedUserIfTrainerAndReturnData();

      
            if (!ModelState.IsValid)
            {
                var suppliers = await _service.GetAllSuppliersAsync();
                if (model.quoteCode == null)
                    model.quoteCode = await _service.GetNewCodeAsync();

          
                return View(model);
            }

            var entity = _mapper.Map<quote>(model);

            if (model.Id == 0)
            {
                model.Id = await _service.AddAsync(entity);
                await _hubContext.Clients.Groups("ActivitiesSupervisor")
               .SendAsync("ReceiveNotification", new
               {
                   Title = "",
                   Message = ""
               });
                await _notificationService.SendNotificationToRoleAsync(
                    "مقارنة عرض اسعار جديدة",
                    $"يوجد مقارنة عرض اسعار جديدة رقم {model?.quoteCode} جاهزة للإعتماد",
                    (int)RoleNumber.ActivitiesSupervisor
              );

                return RedirectToAction(nameof(Index)); // After Add New
            }
            else
            {
                await _service.UpdateAsync(entity);
                await _hubContext.Clients.Groups("ActivitiesSupervisor")
                               .SendAsync("ReceiveNotification", new
                               {
                                   Title = "",
                                   Message = ""
                               });
                await _notificationService.SendNotificationToRoleAsync(
                    "مقارنة عرض اسعار حدثت",
                    $"يوجد مقارنة عرض اسعار حدثت رقم {model?.quoteCode} جاهزة للإعتماد",
                    (int)RoleNumber.ActivitiesSupervisor
              );
            }

            return RedirectToAction(nameof(AddEdit), new { model.Id }); // After Edit
        }

   
        //[YesGet]
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id.HasValue && id.Value != 0)
        //    {
        //        var item = await _service.GetByIdAsync(id.Value);
        //        if (item == null) return NotFound();
        //        var vm = _mapper.Map<quoteVM>(item);
        //        //if (vm != null && vm.UserId != null)
        //        //{
        //        //    ApplicationUser? Applicant = await _userManager.FindByIdAsync(vm.UserId);
        //        //    vm.TrainerName = Applicant?.FullNameAr;
        //        //}
        //        //// Fetch trainer name from signed user (if already signed)
        //        //if (vm.SignatureUser != null && !string.IsNullOrEmpty(vm.SignatureUser.UserId))
        //        //{
        //        //    var trainerUser = await _userManager.FindByIdAsync(vm.SignatureUser.UserId);
        //        //    vm.TrainerSignedName = trainerUser?.FullNameAr ?? trainerUser?.FullNameEn ?? trainerUser?.Email ?? "";
        //        //}
        //        //// Fetch manager full name from ApplicationUser using SignatureManager.UserId
        //        //if (vm.SignatureManager != null && !string.IsNullOrEmpty(vm.SignatureManager.UserId))
        //        //{
        //        //    var mgrUser = await _userManager.FindByIdAsync(vm.SignatureManager.UserId);
        //        //    vm.ManagerUserName = mgrUser?.FullNameAr ?? mgrUser?.FullNameEn ?? mgrUser?.Email ?? "";
        //        //}
        //        return View(vm);
        //    }
        //    else
        //    {
        //        return NotFound();
        //    }
        //}

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> Print(string? searchTerm, DateOnly? dateFrom, DateOnly? dateTo)
        {
            var addPermission = ValidatePermission("quote", "Add");
            var editPermission = ValidatePermission("quote", "Edit");
            var AddOREditPermission = addPermission || editPermission;

            var allquotes = await _service.GetAllAsync(AddOREditPermission);

            // ✅ Date filtering — replace `c.YourDateProperty` with your actual property
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                allquotes = allquotes.Where(c => (!string.IsNullOrEmpty(c.quoteCode) && c.quoteCode.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                || (!string.IsNullOrEmpty(c.OrderText) && c.OrderText.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                );
            }
            if (dateFrom.HasValue)
            {
                allquotes = allquotes.Where(c => c.Date >= dateFrom.Value);
            }
            if (dateTo.HasValue)
            {
                allquotes = allquotes.Where(c => c.Date <= dateTo.Value);
            }

            var allquoteVM = _mapper.Map<IEnumerable<quoteVM>>(allquotes);

            ViewBag.dateFrom = dateFrom?.ToString("yyyy-MM-dd");
            ViewBag.dateTo = dateTo?.ToString("yyyy-MM-dd");

            return View(allquoteVM);
        }

        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> PrintDetails(int? id)
        {
            if (id.HasValue && id.Value != 0)
            {
                var item = await _service.GetByIdAsync(id.Value);
                if (item == null) return NotFound();
                var vm = _mapper.Map<quoteVM>(item);
                var suppliers = await _service.GetAllSuppliersAsync();
                //vm.suppliers = suppliers;
                return View(vm);
            }
            else
            {
                return NotFound();
            }
        }
        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> createExcelReport_Download(string? searchTerm, DateOnly? dateFrom, DateOnly? dateTo)
        {
            var addPermission = ValidatePermission("quote", "Add");
            var editPermission = ValidatePermission("quote", "Edit");
            var AddOREditPermission = addPermission || editPermission;

            var allquotes = await _service.GetAllAsync(AddOREditPermission);

            // ✅ Date filtering — replace `c.YourDateProperty` with your actual property
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                allquotes = allquotes.Where(c => (!string.IsNullOrEmpty(c.quoteCode) && c.quoteCode.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                || (!string.IsNullOrEmpty(c.OrderText) && c.OrderText.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                );
            }
            if (dateFrom.HasValue)
            {
                allquotes = allquotes.Where(c => c.Date >= dateFrom.Value);
            }
            if (dateTo.HasValue)
            {
                allquotes = allquotes.Where(c => c.Date <= dateTo.Value);
            }

            // ---- End Get Data As Print

            var boolStatus = false;
            byte[]? fileBytes = null;
            var pathNewFile = "";
            try
            {
                var lang = SessionHelper.GetCurrentLanguage();
                var allData_list = allquotes;
                var ListTitles = new List<string>
                {
                    "رقم العرض","تاريخ العرض","المطلوب"
                };
                if (allData_list != null || allData_list?.Count() > 0)
                {
                    var excelDataDTO = allData_list.Select(single => new ExcelDataDTO
                    {
                        t1 = single.quoteCode,
                        t2 = (single.Date.HasValue ? (lang == "ar" ? single.Date.Value.ToString("d")?.Replace("/","-") : single.Date.Value.ToString("d")?.Replace("/","-")) : ""),
                        t3 = single.OrderText,
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
                    var fileExcelName = Resource1.quotesList;
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
        public async Task<IActionResult> Details(int id)
        {
            // استخدام Include مباشرة لتحميل كل البيانات المرتبطة
            var quote = await _unitOfWork.quotes.GetQueryable()
                .Include(qi => qi.SignatureSuperVisor).Include(qi => qi.SignatureAccountant).Include(qi => qi.SignatureUserSecetary).Include(qi => qi.SignatureManager)
                .Include(q => q.quotesItems)
                    .ThenInclude(qi => qi.ItemSuppliers)
                        .ThenInclude(its => its.Supplier)
                .FirstOrDefaultAsync(q => q.Id == id);

            if (quote == null)
                return NotFound();

            var qouteVM = new quoteVM
            {
                SignatureSuperVisor = quote.SignatureSuperVisor,
                SignatureAccountant = quote.SignatureAccountant,
                SignatureUserSecetary = quote.SignatureUserSecetary,
                SignatureManager = quote.SignatureManager,

                SignatureSuperVisorId = quote.SignatureSuperVisorId,
                SignatureAccountantId = quote.SignatureAccountantId,
                SignatureUserSecetaryId = quote.SignatureUserSecetaryId,
                SignatureManagerId = quote.SignatureManagerId,
            };
            var viewModel = new quoteitemsVM
            {
                Id = quote.Id,
                quoteCode = quote.quoteCode,
                order = quote.OrderText,
                Date = quote.Date,
                textarea = quote.TextArea,
                IsConfirmed = quote.quoteIsConfirmed,
                Items = new List<SupplierAssignmentItemVM>(),
                SuppliersList = (await _unitOfWork.Suppliers.GetAllAsync(x => x.SupplierCategoryId == 2)) // Companies only
                    .Select(s => new SelectListItem
                    {
                        Value = s.Id.ToString(),
                        Text = s.SupplierNameAr
                    })
                    .ToList()

                    ,
                quote = qouteVM,
            };

            // تجهيز المواد مع الموردين المرتبطين
            foreach (var quoteItem in quote.quotesItems)
            {
                if (quoteItem.ItemSuppliers != null && quoteItem.ItemSuppliers.Any())
                {
                    foreach (var itemSupplier in quoteItem.ItemSuppliers)
                    {
                        viewModel.Items.Add(new SupplierAssignmentItemVM
                        {
                            ItemId = quoteItem.quoteItemId,
                            ItemName = quoteItem.ItemName,
                            Quantity = quoteItem.Quantity,
                            SupplierId = itemSupplier.SupplierId,
                            SupplierName = itemSupplier.Supplier?.SupplierNameAr,
                            SinglePrice = itemSupplier.SinglePrice,
                            ItemSupplierIsConfirmed = itemSupplier.ItemSupplierIsConfirmed
                        });
                    }
                }
                else
                {
                    // مواد بدون مورد
                    viewModel.Items.Add(new SupplierAssignmentItemVM
                    {
                        ItemId = quoteItem.quoteItemId,
                        ItemName = quoteItem.ItemName,
                        Quantity = quoteItem.Quantity,
                        SupplierId = null,
                        SupplierName = null,
                        SinglePrice = null,
                        ItemSupplierIsConfirmed = false
                    });
                }
            }

            return View(viewModel);
        }
        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> Details_ComparePrices(int id)
        {
            // استخدام Include مباشرة لتحميل كل البيانات المرتبطة
            var quote = await _unitOfWork.quotes.GetQueryable()
                .Include(qi => qi.SignatureSuperVisor).Include(qi => qi.SignatureAccountant).Include(qi => qi.SignatureUserSecetary).Include(qi => qi.SignatureManager)
                .Include(q => q.quotesItems)
                    .ThenInclude(qi => qi.ItemSuppliers)
                        .ThenInclude(its => its.Supplier)
                .FirstOrDefaultAsync(q => q.Id == id);

            if (quote == null)
                return NotFound();

            var hasAddEditPermission = ValidatePermission("quote", "Add") || ValidatePermission("quote", "Edit");

            if ((quote.quoteIsConfirmed&& !(ValidateRoleNumber() == RoleNumber.Manager)) || !hasAddEditPermission)
            {
                return RedirectToAction("Details", new { id });
            }

            if (!(ValidateRoleNumber() == RoleNumber.Manager) && (quote?.SignatureSuperVisorId > 0 && !string.IsNullOrWhiteSpace(quote?.SignatureSuperVisor?.ImagePath)))
            {
                return RedirectToAction("Details", new { id });
            }
            else if (ValidateRoleNumber() == RoleNumber.Manager && (quote?.SignatureManagerId > 0 && !string.IsNullOrWhiteSpace(quote?.SignatureManager?.ImagePath)))
            {
                return RedirectToAction("Details", new { id });
            }

            var qouteVM = new quoteVM
                {
                    SignatureSuperVisor = quote.SignatureSuperVisor,
                    SignatureAccountant = quote.SignatureAccountant,
                    SignatureUserSecetary = quote.SignatureUserSecetary,
                    SignatureManager = quote.SignatureManager,

                    SignatureSuperVisorId = quote.SignatureSuperVisorId,
                    SignatureAccountantId = quote.SignatureAccountantId,
                    SignatureUserSecetaryId = quote.SignatureUserSecetaryId,
                    SignatureManagerId = quote.SignatureManagerId,
                };
            var viewModel = new quoteitemsVM
            {
                Id = quote.Id,
                quoteCode = quote.quoteCode,
                order = quote.OrderText,
                Date = quote.Date,
                textarea=quote.TextArea,
                IsConfirmed = quote.quoteIsConfirmed,
                Items = new List<SupplierAssignmentItemVM>(),
                SuppliersList = (await _unitOfWork.Suppliers.GetAllAsync(x => x.SupplierCategoryId == 2)) // Companies only
                    .Select(s => new SelectListItem
                    {
                        Value = s.Id.ToString(),
                        Text = s.SupplierNameAr
                    })
                    .ToList()

                    ,
                quote = qouteVM,
            };

            // تجهيز المواد مع الموردين المرتبطين
            foreach (var quoteItem in quote.quotesItems)
            {
                if (quoteItem.ItemSuppliers != null && quoteItem.ItemSuppliers.Any())
                {
                    foreach (var itemSupplier in quoteItem.ItemSuppliers)
                    {
                        viewModel.Items.Add(new SupplierAssignmentItemVM
                        {
                            ItemId = quoteItem.quoteItemId,
                            ItemName = quoteItem.ItemName,
                            Quantity = quoteItem.Quantity,
                            SupplierId = itemSupplier.SupplierId,
                            SupplierName = itemSupplier.Supplier?.SupplierNameAr,
                            SinglePrice = itemSupplier.SinglePrice,
                            ItemSupplierIsConfirmed = itemSupplier.ItemSupplierIsConfirmed
                        });
                    }
                }
                else
                {
                    // مواد بدون مورد
                    viewModel.Items.Add(new SupplierAssignmentItemVM
                    {
                        ItemId = quoteItem.quoteItemId,
                        ItemName = quoteItem.ItemName,
                        Quantity = quoteItem.Quantity,
                        SupplierId = null,
                        SupplierName = null,
                        SinglePrice = null,
                        ItemSupplierIsConfirmed = false
                    });
                }
            }

            return View(viewModel);
        }
       
        [HttpPost]
        [IgnoreAction]
        public async Task<IActionResult> ConfirmSupplierAssignment([FromBody] ConfirmSupplierAssignmentModel model)
        {
            try
            {
                // جلب الـ quote مع جميع البيانات المرتبطة
                var quote = await _unitOfWork.quotes.GetQueryable()
                    .Include(q => q.quotesItems)
                        .ThenInclude(qi => qi.ItemSuppliers)
                    .FirstOrDefaultAsync(q => q.Id == model.QuoteId);

                if (quote == null)
                    return Json(new { success = false, message = "الطلب غير موجود" });

                // تحديث أو إضافة ItemSupplier
                foreach (var item in model.Items)
                {
                    var quoteItem = quote.quotesItems.FirstOrDefault(qi => qi.quoteItemId == item.ItemId);
                    if (quoteItem == null) continue;

                    // البحث عن ItemSupplier موجود
                    var existingItemSupplier = quoteItem.ItemSuppliers
                        .FirstOrDefault(its => its.SupplierId == item.SupplierId);

                    if (existingItemSupplier != null)
                    {
                        // تحديث الموجود
                        existingItemSupplier.SinglePrice = item.SinglePrice;
                        existingItemSupplier.ItemSupplierIsConfirmed = true;
                        _unitOfWork.ItemSuppliers.Update(existingItemSupplier);
                    }
                    else
                    {
                        // إضافة جديد
                        var newItemSupplier = new ItemSupplier
                        {
                            quotesItemId = quoteItem.quoteItemId,
                            SupplierId = item.SupplierId,
                            SinglePrice = item.SinglePrice,
                            ItemSupplierIsConfirmed = true
                        };
                        await _unitOfWork.ItemSuppliers.AddAsync(newItemSupplier);
                    }
                }

                // حساب المجموع الكلي
                decimal orderTotal = 0;
                foreach (var quoteItem in quote.quotesItems)
                {
                    // إعادة تحميل ItemSuppliers مع Suppliers للتأكد
                    var itemWithSuppliers = await _unitOfWork.quotesItems.GetQueryable()
                        .Include(qi => qi.ItemSuppliers)
                            .ThenInclude(its => its.Supplier)
                        .FirstOrDefaultAsync(qi => qi.quoteItemId == quoteItem.quoteItemId);

                    if (itemWithSuppliers != null)
                    {
                        foreach (var itemSupplier in itemWithSuppliers.ItemSuppliers.Where(its => its.ItemSupplierIsConfirmed))
                        {
                            orderTotal += (itemSupplier.SinglePrice ?? 0) * (quoteItem.Quantity ?? 0);
                        }
                    }
                }
                quote.OrderTotal = orderTotal;

                // التحقق من تأكيد كل الموردين
                var allItemSuppliers = quote.quotesItems
                    .SelectMany(qi => qi.ItemSuppliers)
                    .ToList();

                if (allItemSuppliers.Any() && allItemSuppliers.All(its => its.ItemSupplierIsConfirmed))
                {
                    quote.quoteIsConfirmed = true;
                }

                _unitOfWork.quotes.Update(quote);
                await _unitOfWork.CompleteAsync();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // ──────── AJAX: Confirm Single Supplier ────────
        [HttpPost]
        [IgnoreAction]
        public async Task<IActionResult> ConfirmSingleSupplier([FromBody] ConfirmSingleSupplierModel model)
        {
            try
            {
                var quote = await _unitOfWork.quotes.GetQueryable()
                    .Include(q => q.quotesItems)
                        .ThenInclude(qi => qi.ItemSuppliers)
                    .FirstOrDefaultAsync(q => q.Id == model.QuoteId);

                if (quote == null)
                    return Json(new { success = false, message = "الطلب غير موجود" });

                foreach (var quoteItem in quote.quotesItems)
                {
                    // Find the price from the incoming model items
                    var incomingItem = model.Items?.FirstOrDefault(i => i.ItemId == quoteItem.quoteItemId);
                    var priceToSave = incomingItem?.SinglePrice ?? 0;

                    // Check if supplier already linked to this item
                    var existing = quoteItem.ItemSuppliers
                        .FirstOrDefault(its => its.SupplierId == model.SupplierId);

                    if (existing != null)
                    {
                        existing.SinglePrice = priceToSave;
                        existing.ItemSupplierIsConfirmed = true;
                        _unitOfWork.ItemSuppliers.Update(existing);
                    }
                    else
                    {
                        var newItemSupplier = new ItemSupplier
                        {
                            quotesItemId = quoteItem.quoteItemId,
                            SupplierId = model.SupplierId,
                            SinglePrice = priceToSave,
                            ItemSupplierIsConfirmed = true
                        };
                        await _unitOfWork.ItemSuppliers.AddAsync(newItemSupplier);
                    }
                }

                await _unitOfWork.CompleteAsync();

                // Return updated items for this supplier
                var updatedQuote = await _unitOfWork.quotes.GetQueryable()
                    .Include(q => q.quotesItems)
                        .ThenInclude(qi => qi.ItemSuppliers)
                            .ThenInclude(its => its.Supplier)
                    .FirstOrDefaultAsync(q => q.Id == model.QuoteId);

                var supplierItems = updatedQuote.quotesItems
                    .SelectMany(qi => qi.ItemSuppliers.Where(its => its.SupplierId == model.SupplierId)
                        .Select(its => new
                        {
                            ItemId = qi.quoteItemId,
                            ItemName = qi.ItemName,
                            Quantity = qi.Quantity,
                            SupplierId = its.SupplierId,
                            SupplierName = its.Supplier?.SupplierNameAr,
                            SinglePrice = its.SinglePrice,
                            ItemSupplierIsConfirmed = its.ItemSupplierIsConfirmed
                        }))
                    .ToList();

                return Json(new { success = true, items = supplierItems });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // ──────── AJAX: Update Text Area ────────
        [HttpPost]
        [IgnoreAction]
        public async Task<IActionResult> UpdateTextArea([FromBody] UpdateTextAreaModel model)
        {
            try
            {
                var quote = await _unitOfWork.quotes.GetByIdAsync(model.QuoteId);
                if (quote == null)
                    return Json(new { success = false, message = "الطلب غير موجود" });

                quote.TextArea = model.TextArea;
                _unitOfWork.quotes.Update(quote);
                await _unitOfWork.CompleteAsync();

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // ──────── AJAX: Delete All Items for a Supplier ────────
        [HttpPost]
        [IgnoreAction]
        public async Task<IActionResult> DeleteSupplierItems([FromBody] DeleteSupplierModel model)
        {
            try
            {
                var quote = await _unitOfWork.quotes.GetQueryable()
                    .Include(q => q.quotesItems)
                        .ThenInclude(qi => qi.ItemSuppliers)
                    .FirstOrDefaultAsync(q => q.Id == model.QuoteId);

                if (quote == null)
                    return Json(new { success = false, message = "الطلب غير موجود" });

                foreach (var quoteItem in quote.quotesItems)
                {
                    var toRemove = quoteItem.ItemSuppliers
                        .Where(its => its.SupplierId == model.SupplierId)
                        .ToList();

                    foreach (var item in toRemove)
                    {
                        _unitOfWork.ItemSuppliers.Delete(item);
                    }
                }

                await _unitOfWork.CompleteAsync();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // ──────── AJAX: Delete Single ItemSupplier ────────
        [HttpPost]
        [IgnoreAction]
        public async Task<IActionResult> DeleteSingleItem([FromBody] DeleteSingleItemModel model)
        {
            try
            {
                var quote = await _unitOfWork.quotes.GetQueryable()
                    .Include(q => q.quotesItems)
                        .ThenInclude(qi => qi.ItemSuppliers)
                    .FirstOrDefaultAsync(q => q.Id == model.QuoteId);

                if (quote == null)
                    return Json(new { success = false, message = "الطلب غير موجود" });

                var quoteItem = quote.quotesItems.FirstOrDefault(qi => qi.quoteItemId == model.ItemId);
                if (quoteItem == null)
                    return Json(new { success = false, message = "العنصر غير موجود" });

                var itemSupplier = quoteItem.ItemSuppliers
                    .FirstOrDefault(its => its.SupplierId == model.SupplierId);

                if (itemSupplier != null)
                {
                    _unitOfWork.ItemSuppliers.Delete(itemSupplier);
                    await _unitOfWork.CompleteAsync();
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // ──────── AJAX: Update Item Price ────────
        [HttpPost]
        [IgnoreAction]
        public async Task<IActionResult> UpdateItemPrice([FromBody] UpdateItemPriceModel model)
        {
            try
            {
                var quote = await _unitOfWork.quotes.GetQueryable()
                    .Include(q => q.quotesItems)
                        .ThenInclude(qi => qi.ItemSuppliers)
                    .FirstOrDefaultAsync(q => q.Id == model.QuoteId);

                if (quote == null)
                    return Json(new { success = false, message = "الطلب غير موجود" });

                var quoteItem = quote.quotesItems.FirstOrDefault(qi => qi.quoteItemId == model.ItemId);
                if (quoteItem == null)
                    return Json(new { success = false, message = "العنصر غير موجود" });

                var itemSupplier = quoteItem.ItemSuppliers
                    .FirstOrDefault(its => its.SupplierId == model.SupplierId);

                if (itemSupplier != null)
                {
                    itemSupplier.SinglePrice = model.NewPrice;
                    _unitOfWork.ItemSuppliers.Update(itemSupplier);
                    await _unitOfWork.CompleteAsync();
                }

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        // ──────── AJAX: Final Confirm Quote ────────
        [HttpPost]
        [IgnoreAction]
        public async Task<IActionResult> FinalConfirmQuote([FromBody] int quoteId)
        {
            try
            {
                var quote = await _unitOfWork.quotes.GetByIdAsync(quoteId);
                if (quote == null)
                    return Json(new { success = false, message = "الطلب غير موجود" });

                quote.quoteIsConfirmed = true;
                _unitOfWork.quotes.Update(quote);
                await _unitOfWork.CompleteAsync();
                if (ValidateRoleNumber() != RoleNumber.ActivitiesSupervisor)
                {
                    await _hubContext.Clients.Groups("ActivitiesSupervisor")
                 .SendAsync("ReceiveNotification", new
                 {
                     Title = "",
                     Message = ""
                 });
                    await _notificationService.SendNotificationToRoleAsync(
                        "مقارنة عرض اسعار جديدة",
                        $"يوجد مقارنة عرض اسعار جديدة رقم {quote?.quoteCode} جاهزة للإعتماد",
                        (int)RoleNumber.ActivitiesSupervisor
                  );
                }
                return Json(new { success = true, url = Url.Action("Details_ComparePrices", new { id = quoteId }) });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
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

            var report = await _unitOfWork.quotes.GetByColumnAsync(
                    e => e.quoteCode != null && e.Id == (int)request.Id);

            var result = await _service.ValidateOtpAsync((int)request.Id, request.Code, request.Role ?? "Tr");
            if (result.success==true)
            {
                var gropName = "";
                int roleNumber = 0;
                if (request.Role == "Supervisor")
                {
                    gropName = "Accountant";
                    roleNumber = (int)RoleNumber.Accountant;
                }
                else if (request.Role == "Accountant")
                {
                    // 0 , ""
                    //gropName = "Secetary";
                    //roleNumber = (int)RoleNumber.NormalUser;
                    await _hubContext.Clients.Groups("Secetary")
                    .SendAsync("ReceiveNotification", new
                    {
                        Title = "",
                        Message = ""
                    });
                    await _notificationService.SendNotificationToPermissionAsync(
                      "مقارنة عرض اسعار جديدة",
                      $"يوجد مقارنة عرض اسعار جديدة رقم {report?.quoteCode} جاهزة للإعتماد",
                      "quote.SecrtaryPermissionSign"
                  );
                }
                else if (request.Role == "Secetary")
                {
                    gropName = "Manager";
                    roleNumber = (int)RoleNumber.Manager;
                }


                if (!string.IsNullOrWhiteSpace(gropName) && roleNumber>0)
                {
                    await _hubContext.Clients.Groups(gropName)
                   .SendAsync("ReceiveNotification", new
                   {
                       Title = "",
                       Message = ""
                   });
                    await _notificationService.SendNotificationToRoleAsync(
                      "مقارنة عرض اسعار جديدة",
                      $"يوجد مقارنة عرض اسعار جديدة رقم {report?.quoteCode} جاهزة للإعتماد",
                      roleNumber
                  );
                }
            }
            

            return Json(new { success = result.success, message = result.message });
        }


        [YesGet]
        public async Task<IActionResult> SecrtaryPermissionSign()
        {
            return Ok();    
        }
        #endregion

    }
}