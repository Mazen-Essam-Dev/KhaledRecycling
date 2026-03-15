using Application.Helpers;
using Application.Interfaces.Admin;
using Application.Interfaces.Admin.ExpenseAndReceipt;
using Application.Services.Admin;
using AutoMapper;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Vml.Office;
using DocumentFormat.OpenXml.Wordprocessing;
using Domain.DTOs;
using Domain.Entities;
using Domain.Entities.ExpenseAndReceipt;
using Domain.Enums;
using Domain.Resources;
using FougeraClub.Areas.Admin.ViewModels;
using FougeraClub.Areas.Admin.ViewModels.Cars;
using FougeraClub.Areas.Admin.ViewModels.ExpenseAndReceipts;
using FougeraClub.Areas.Admin.ViewModels.SMS;
using FougeraClub.Attributes;
using FougeraClub.Helpers;
using FougeraClub.Middelware;
using Infrastructure.Repositories.InterfacesDB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.CodeAnalysis.Elfie.Model.Tree;
using System;

namespace FougeraClub.Areas.Admin.Controllers
{
    [AdminAuthorize]
    [Area("Admin")]
    public class ExpenseAndReceiptController : Controller
    {
        private readonly IExpenseService _expenseService;
        private readonly IReceiptService _receiptService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHubContext<Hub.NotificationHub> _hubContext;
        private readonly INotificationService _notificationService;

        public ExpenseAndReceiptController(IExpenseService expenseService, IReceiptService receiptService, IMapper mapper, IUnitOfWork unitOfWork, IHubContext<Hub.NotificationHub> hubContext, INotificationService notificationService)
        {
            _expenseService = expenseService;
            _receiptService = receiptService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _hubContext = hubContext;
            _notificationService = notificationService;
        }

        public async Task<IActionResult> AddEdit(int? id, ItemType itemType)
        {
            var vm = new ExpenseAndReceiptVM();

            if (itemType == ItemType.Expenses)
                ViewBag.ItemType = ItemType.Expenses;
            else if (itemType == ItemType.Receipts)
                ViewBag.ItemType = ItemType.Receipts;
            else
                ViewBag.ItemType = null;

            var ExpensesGates = await _unitOfWork.ExpensesGates.GetAllAsync();
            ExpensesGates = ExpensesGates.OrderBy(x => x.Id);
            var ExpensesSources = await _unitOfWork.ExpensesSources.GetAllAsync();
            ExpensesSources = ExpensesSources.OrderBy(x => x.Id);


            if (id == null || id == 0)
            {
                //bind dropdown lists
                vm.ItemTypesList = RadioButtonHelper.GetRadioList<ItemType>((int)itemType);
                var allSuppliers = await _unitOfWork.Suppliers.GetAllAsync();
                ViewBag.AllSuppliers = allSuppliers.ToList();
                var Suppliers = allSuppliers.ToList();
                // If creating a Receipt (مقبوضات) show only suppliers with category 2
                if (itemType == ItemType.Receipts)
                {
                    Suppliers = Suppliers.Where(s => s.SupplierCategoryId == 2).ToList();
                }
                vm.SuppliersList = SelectListHelper.BindSelectList(Suppliers, vm.SupplierId, "Id", "SupplierNameAr", "SupplierNameEn").ToList();

                // ExpensesSourcesList needed for both Expenses and Receipts
                vm.ExpensesSourcesList = SelectListHelper.BindSelectList(ExpensesSources.ToList(), vm.ExpensesSourceId, "Id", "NameAr", "NameEn").ToList();

                if (itemType != ItemType.Receipts)
                {
                    var BudgetItems = await _unitOfWork.BudgetItems.GetAllAsync();
                    vm.BudgetItemsList = SelectListHelper.BindSelectList(BudgetItems.ToList(), vm.BudgetItemId, "Id", "ItemTitle", "ItemTitle").ToList();
                    vm.ExpensesGateList = SelectListHelper.BindSelectList(ExpensesGates.ToList(), vm.ExpensesGateId, "Id", "NameAr", "NameEn").ToList();
                }
                ViewBag.SelectedType = vm.ItemTypeId.ToString();

                vm.Attachment_OldPath = vm.AttachmentPath;

                return View(vm);
            }

            if (itemType == ItemType.Expenses)
            {
                var expense = await _expenseService.GetByIdAsync(id.Value);
                if (expense == null) return NotFound();
                vm = _mapper.Map<ExpenseAndReceiptVM>(expense);
                vm.idForReciptSign = vm.Id;
            }
            else
            {
                var receipt = await _receiptService.GetByIdAsync(id.Value);
                if (receipt == null) return NotFound();
                vm = _mapper.Map<ExpenseAndReceiptVM>(receipt);
            }

            //bind dropdown lists
            var suppliers = await _unitOfWork.Suppliers.GetAllAsync();
            ViewBag.AllSuppliers = suppliers.ToList();
            vm.ItemTypesList = RadioButtonHelper.GetRadioList<ItemType>((int)itemType);
            // If viewing/editing a Receipt, limit suppliers to category 2
            if (vm.ItemType == ItemType.Receipts)
            {
                suppliers = suppliers.Where(s => s.SupplierCategoryId == 2).ToList();
            }
            vm.SuppliersList = SelectListHelper.BindSelectList(suppliers.ToList(), vm.SupplierId, "Id", "SupplierNameAr", "SupplierNameEn").ToList();


            // ExpensesSourcesList needed for both Expenses and Receipts
            vm.ExpensesSourcesList = SelectListHelper.BindSelectList(ExpensesSources.ToList(), vm.ExpensesSourceId, "Id", "NameAr", "NameEn").ToList();

            if (vm.ItemType != ItemType.Receipts)
            {
                var budgetItems = await _unitOfWork.BudgetItems.GetAllAsync();
                vm.BudgetItemsList = SelectListHelper.BindSelectList(budgetItems.ToList(), vm.BudgetItemId, "Id", "ItemTitle", "ItemTitle").ToList();
                vm.ExpensesGateList = SelectListHelper.BindSelectList(ExpensesGates.ToList(), vm.ExpensesGateId, "Id", "NameAr", "NameEn").ToList();
            }
            ViewBag.SelectedType = vm.ItemTypeId.ToString();
            vm.Attachment_OldPath = vm.AttachmentPath;
            var allRecivingReciptsHasSignned = await _unitOfWork.ReceivingReceipts.GetAllAsync(x => x.ExpenseId == vm.idForReciptSign && x.ManagerSignitureId != null, c => c.ManagerSignature!) ?? new List<ReceivingReceipt>();
            var allIdsOfRecivingReciptsHasSignned = allRecivingReciptsHasSignned.Where(y => y.ManagerSignature?.ImagePath?.Length > 1).Select(x => x.ExpenseId ?? -1).ToList();

            if (allIdsOfRecivingReciptsHasSignned.Contains(vm.idForReciptSign))
            {
                vm.isSigned = true;
            }
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEdit(ExpenseAndReceiptVM model)
        {
            ViewBag.SelectedType = model.ItemTypeId.ToString();
            ModelState.Remove("BudgetItemId");
            ModelState.Remove("ExpensesGateId");
            ModelState.Remove("ExpensesSourceId");
            ModelState.Remove("SupplierId");
            
            if (model.ItemTypeId == 1) // Expenses
            {
                if (model.BudgetItemId == null) ModelState.AddModelError("BudgetItemId", Resource1.Required);
                if (model.ExpensesGateId == null) ModelState.AddModelError("ExpensesGateId", Resource1.Required);
                if (model.ExpensesSourceId == null) ModelState.AddModelError("ExpensesSourceId", Resource1.Required);
                if (model.SupplierId == null) ModelState.AddModelError("SupplierId", Resource1.Required);
            }
            else if (model.ItemTypeId == 2) // Receipts
            {
                // For Receipts (مقبوضات) we require the Supplier (الجهة) and not the Expenses Source (المصدر)
                if (model.ExpensesSourceId == null) ModelState.AddModelError("ExpensesSourceId", Resource1.Required);
                if (model.SupplierId == null) ModelState.AddModelError("SupplierId", Resource1.Required);
            }

            // Notes (البيان) is required for both Expenses and Receipts
            if (string.IsNullOrWhiteSpace(model.Notes))
            {
                ModelState.AddModelError("Notes", Resource1.Required);
            }

            var FolderEntityWillSaveIn = (model.ItemTypeId == 1) ? "Expenses" : "Receipts";
            #region Validate Is File is PDF And MG // Validate Images
            // Local function to validate the uploaded image size & type
            async Task<string?> ValidateImageAsync(string? TempPath, IFormFile? fileOrignal, string? filePath, string key)
            {

                IFormFile? Image_File_Temp = !string.IsNullOrEmpty(TempPath) ? FileHelper.ConvertToIFormFile(TempPath) : fileOrignal;
                string? NewImage_path = !string.IsNullOrEmpty(TempPath) ? TempPath : filePath;
                var result_Text = await FileHelper.CheckFileIsPdf_5Mg_Async(Image_File_Temp);
                if (result_Text != "OK" && result_Text != "null")
                {
                    ModelState.AddModelError(key, result_Text);
                    // Delete old actual image from Final folder if exists
                    FileHelper.DeleteImageFile(NewImage_path);
                }
                return result_Text;
            }

            var Attachment_Text = await ValidateImageAsync(model.Attachment_TempFilePath, model.Attachment, model.AttachmentPath, "Attachment");

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
                var suppliers = await _unitOfWork.Suppliers.GetAllAsync();
                ViewBag.AllSuppliers = suppliers.ToList();
                model.ItemTypesList = RadioButtonHelper.GetRadioList<ItemType>((int)model.ItemType);
                // If validation failed for a Receipt, show only suppliers in category 2
                if (model.ItemType == ItemType.Receipts)
                {
                    suppliers = suppliers.Where(s => s.SupplierCategoryId == 2).ToList();
                }
                model.SuppliersList = SelectListHelper.BindSelectList(suppliers.ToList(), model.SupplierId, "Id", "SupplierNameAr", "SupplierNameEn").ToList();

                // ExpensesSourcesList needed for both Expenses and Receipts
                var ExpensesGates = await _unitOfWork.ExpensesGates.GetAllAsync();
                ExpensesGates = ExpensesGates.OrderBy(x => x.Id);
                var ExpensesSources = await _unitOfWork.ExpensesSources.GetAllAsync();
                ExpensesSources = ExpensesSources.OrderBy(x => x.Id);


                model.ExpensesSourcesList = SelectListHelper.BindSelectList(ExpensesSources.ToList(), model.ExpensesSourceId, "Id", "NameAr", "NameEn").ToList();

                //if (model.ItemType != ItemType.Receipts)
                //{
                    var budgetItems = await _unitOfWork.BudgetItems.GetAllAsync();
                    model.BudgetItemsList = SelectListHelper.BindSelectList(budgetItems.ToList(), model.BudgetItemId, "Id", "ItemTitle", "ItemTitle").ToList();
                    model.ExpensesGateList = SelectListHelper.BindSelectList(ExpensesGates.ToList(), model.ExpensesGateId, "Id", "NameAr", "NameEn").ToList();
                //}

                TempData.Keep(); // for safety if re-rendered
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


            if (model.Id == 0)
            {
                if (model.Date.HasValue)
                {
                    var month = model.Date.HasValue ? model.Date.Value.Month : 1;
                    var year = model.Date.HasValue ? model.Date.Value.Year : 1000;
                    var CheckthisYearMonth = await _unitOfWork.ExpenseAndReceiptAndOther.GetAllAsync(x => x.Date.Year == year && x.Date.Month == month && x.ExpensesSourceId == model.ExpensesSourceId);

                    if (model.ExpensesSourceId == (int)ExpensesSourceEnum.MiscellaneousExpenses)
                    {
                        if (CheckthisYearMonth.Count() == 0)
                        {
                            await _hubContext.Clients.Groups("Accountant")
                                .SendAsync("ReceiveNotification", new
                                {
                                    Title = "",
                                    Message = ""
                                });
                            await _notificationService.SendNotificationToRoleAsync(
                                  "تقرير للمصروفات النثرية جديد",
                                  $"يوجد تقرير للمصروفات النثرية تاريخ {month + " - " + year} جديد جاهز للإعتماد",
                                  (int)RoleNumber.Accountant
                              );
                        }
                    }
                    else if (model.ExpensesSourceId == (int)ExpensesSourceEnum.Bank)
                    {
                        if (CheckthisYearMonth.Count() == 0)
                        {
                            await _hubContext.Clients.Groups("Accountant")
                              .SendAsync("ReceiveNotification", new
                              {
                                  Title = "",
                                  Message = ""
                              });
                            await _notificationService.SendNotificationToRoleAsync(
                                  "تقرير للمصروفات والمقبوضات جديد",
                                  $"يوجد تقرير للمصروفات والمقبوضات تاريخ {month + " - " + year} جديد جاهز للإعتماد",
                                  (int)RoleNumber.Accountant
                              );
                        }
                    }
                        //var existReport = await _unitOfWork.ExpensesAndReciptReportSigns.GetAllAsync(x => x.Month == model.Date.Value.Month && x.Year == model.Date.Value.Year);
                        //if (existReport.Count() == 0 && model.ExpensesSourceId == (int)ExpensesSourceEnum.MiscellaneousExpenses)
                        //{
                        //    await _hubContext.Clients.Groups("Accountant")
                        //        .SendAsync("ReceiveNotification", new
                        //        {
                        //            Title = "",
                        //            Message = ""
                        //        });
                        //    await _notificationService.SendNotificationToRoleAsync(
                        //      "تقرير للمصروفات النثرية جديد",
                        //      $"يوجد تقرير للمصروفات النثرية تاريخ {model?.Date?.Month + " - " + model?.Date?.Year} جديد جاهزة للإعتماد",
                        //          (int)RoleNumber.Accountant
                        //      );

                        //    var ExpensesAndReciptReportSign = new ExpensesAndReciptReportSign
                        //    {
                        //        Month = model.Date.Value.Month,
                        //        Year = model.Date.Value.Year,
                        //        ReportTypeId = (int)ReportTypeEnum.ExpensesReport,
                        //    };
                        //    await _unitOfWork.ExpensesAndReciptReportSigns.AddAsync(ExpensesAndReciptReportSign);
                        //    await _unitOfWork.CompleteAsync();
                        //}
                        //else if (existReport.Count() == 0 && model.ExpensesSourceId == (int)ExpensesSourceEnum.Bank)
                        //{
                        //    await _hubContext.Clients.Groups("Accountant")
                        //        .SendAsync("ReceiveNotification", new
                        //        {
                        //            Title = "",
                        //            Message = ""
                        //        });
                        //    await _notificationService.SendNotificationToRoleAsync(
                        //          "تقرير للمصروفات والمقبوضات جديد",
                        //          $"يوجد تقرير للمصروفات والمقبوضات تاريخ {model?.Date?.Month + " - " + model?.Date?.Year} جديد جاهز للإعتماد",
                        //          (int)RoleNumber.Accountant
                        //      );
                        //    var ExpensesAndReciptReportSign = new ExpensesAndReciptReportSign
                        //    {
                        //        Month = model.Date.Value.Month,
                        //        Year = model.Date.Value.Year,
                        //        ReportTypeId = (int)ReportTypeEnum.ExpensesAndReceiptsReport,
                        //    };
                        //    await _unitOfWork.ExpensesAndReciptReportSigns.AddAsync(ExpensesAndReciptReportSign);
                        //    await _unitOfWork.CompleteAsync();
                        //}

                    }
            }

            if (model.ItemType == ItemType.Expenses)
            {
                var entity = _mapper.Map<ExpenseAndReceiptAndOther>(model);
                if (model.Id == 0)
                {
                    model.Id = await _expenseService.AddAsync(entity);
                    return RedirectToAction(nameof(Expenses)); // After Add New
                }
                else
                {
                    await _expenseService.UpdateAsync(entity);
                    return RedirectToAction(nameof(AddEdit), new { id = model.Id, itemType = ItemType.Expenses });
                }
            }
            else
            {
                var entity = _mapper.Map<ExpenseAndReceiptAndOther>(model);
                if (model.Id == 0)
                {                    
                    model.Id = await _receiptService.AddAsync(entity);
                    return RedirectToAction(nameof(Receipts)); // After Add New
                }
                else
                {
                    await _receiptService.UpdateAsync(entity);
                    return RedirectToAction(nameof(AddEdit), new { id = model.Id, itemType = ItemType.Receipts });
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id, ItemType itemType)
        {
            if (itemType == ItemType.Expenses)
            {
                await _expenseService.DeleteAsync(id);
                return RedirectToAction(nameof(Expenses));
            }
            else
            {
                await _receiptService.DeleteAsync(id);
                return RedirectToAction(nameof(Receipts));
            }
        }

        // Return suppliers as JSON, optionally filtered by SupplierCategoryId
        [HttpGet]
        [IgnoreAction]
        public async Task<IActionResult> GetSuppliersByCategory(int? categoryId)
        {
            var suppliers = await _unitOfWork.Suppliers.GetAllAsync();
            if (categoryId.HasValue)
            {
                suppliers = suppliers.Where(s => s.SupplierCategoryId == categoryId.Value).ToList();
            }

            var lang = SessionHelper.GetCurrentLanguage();
            var list = suppliers.Select(s => new
            {
                id = s.Id,
                text = lang == "ar" ? (s.SupplierNameAr ?? string.Empty) : (s.SupplierNameEn ?? string.Empty)
            }).ToList();

            return Json(list);
        }

        #region eXCEL aND pRINT _Receipts
        [YesGet]
        public async Task<IActionResult> Receipts(string? searchTerm, int? selectedSupplier, int? selectedSource, DateOnly? dateFrom, DateOnly? dateTo, int page = 1, int pageSize = 50)
        {
            var allReceipts = await _receiptService.GetAllAsync();
            var receiptVMs = _mapper.Map<List<ExpenseAndReceiptVM>>(allReceipts).AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                receiptVMs = receiptVMs.Where(s =>
                    (!string.IsNullOrEmpty(s.Notes) && s.Notes.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                );
            }
            if (selectedSupplier.HasValue)
                receiptVMs = receiptVMs.Where(e => e.SupplierId != null && e.SupplierId == selectedSupplier);

            if (selectedSource.HasValue)
                receiptVMs = receiptVMs.Where(e => e.ExpensesSourceId != null && e.ExpensesSourceId == selectedSource);

            // ✅ Date filtering — replace `c.YourDateProperty` with your actual property
            if (dateFrom.HasValue)
            {
                receiptVMs = receiptVMs.Where(c => c.Date >= dateFrom.Value);
            }
            if (dateTo.HasValue)
            {
                receiptVMs = receiptVMs.Where(c => c.Date <= dateTo.Value);
            }

            var paginated = PaginatedList<ExpenseAndReceiptVM>.Create(receiptVMs, page, pageSize, "");

            var totalAmount = receiptVMs.Sum(e => e.Amount??0);
            if (paginated.Items.Count > 0)
                paginated.Items[0].TotalAmount = totalAmount;

            #region fill filters
            var suppliers = await _unitOfWork.Suppliers.GetAllAsync();
            // Only show suppliers with category id == 2 for Receipts filter
            suppliers = suppliers.Where(s => s.SupplierCategoryId == 2).ToList();
            var expensesSources = await _unitOfWork.ExpensesSources.GetAllAsync();
            expensesSources = expensesSources.OrderBy(x => x.Id);
            ViewBag.SuppliersList = SelectListHelper.BindSelectList(suppliers.ToList(), selectedSupplier, "Id", "SupplierNameAr", "SupplierNameEn").ToList();
            ViewBag.ExpensesSourcesList = SelectListHelper.BindSelectList(expensesSources.ToList(), selectedSource, "Id", "NameAr", "NameEn").ToList();

            ViewBag.dateFrom = dateFrom?.ToString("yyyy-MM-dd");
            ViewBag.dateTo = dateTo?.ToString("yyyy-MM-dd");
            #endregion

            // Calling from Ajax Return PartialView
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_ReceiptsListPartial", paginated);
            }

            ViewBag.SelectedSource = selectedSource;
            return View(paginated);
        }
        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> Print_Receipts(string? searchTerm, int? selectedSupplier, int? selectedSource, DateOnly? dateFrom, DateOnly? dateTo)
        {
            var allReceipts = await _receiptService.GetAllAsync();
            var receiptVMs = _mapper.Map<List<ExpenseAndReceiptVM>>(allReceipts).AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                receiptVMs = receiptVMs.Where(s =>
                    (!string.IsNullOrEmpty(s.Notes) && s.Notes.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                );
            }
            if (selectedSupplier.HasValue)
                receiptVMs = receiptVMs.Where(e => e.SupplierId != null && e.SupplierId == selectedSupplier);
            if (selectedSource.HasValue)
                receiptVMs = receiptVMs.Where(e => e.ExpensesSourceId != null && e.ExpensesSourceId == selectedSource);

            // ✅ Date filtering — replace `c.YourDateProperty` with your actual property
            if (dateFrom.HasValue)
            {
                receiptVMs = receiptVMs.Where(c => c.Date >= dateFrom.Value);
            }
            if (dateTo.HasValue)
            {
                receiptVMs = receiptVMs.Where(c => c.Date <= dateTo.Value);
            }

            var totalAmount = receiptVMs.Sum(e => e.Amount??0);

            ViewBag.totalAmount = totalAmount;
            ViewBag.SelectedSource = selectedSource;
            return View(receiptVMs);
        }
        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> createExcelReport_Download_Receipts(string? searchTerm, int? selectedSupplier, int? selectedSource, DateOnly? dateFrom, DateOnly? dateTo)
        {
            var allReceipts = await _receiptService.GetAllAsync();
            var receiptVMs = _mapper.Map<List<ExpenseAndReceiptVM>>(allReceipts).AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                receiptVMs = receiptVMs.Where(s =>
                    (!string.IsNullOrEmpty(s.Notes) && s.Notes.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                );
            }
            if (selectedSupplier.HasValue)
                receiptVMs = receiptVMs.Where(e => e.SupplierId != null && e.SupplierId == selectedSupplier);
            if (selectedSource.HasValue)
                receiptVMs = receiptVMs.Where(e => e.ExpensesSourceId != null && e.ExpensesSourceId == selectedSource);

            // ✅ Date filtering — replace `c.YourDateProperty` with your actual property
            if (dateFrom.HasValue)
            {
                receiptVMs = receiptVMs.Where(c => c.Date >= dateFrom.Value);
            }
            if (dateTo.HasValue)
            {
                receiptVMs = receiptVMs.Where(c => c.Date <= dateTo.Value);
            }

            var totalAmount = receiptVMs.Sum(e => e.Amount??0);

            ViewBag.totalAmount = totalAmount;

            #region fill filters
            var suppliers = await _unitOfWork.Suppliers.GetAllAsync();
            // Only include category 2 suppliers for Receipts
            suppliers = suppliers.Where(s => s.SupplierCategoryId == 2).ToList();
            ViewBag.SuppliersList = SelectListHelper.BindSelectList(suppliers.ToList(), selectedSupplier, "Id", "SupplierNameAr", "SupplierNameEn").ToList();

            ViewBag.dateFrom = dateFrom?.ToString("yyyy-MM-dd");
            ViewBag.dateTo = dateTo?.ToString("yyyy-MM-dd");
            ViewBag.totalAmount = totalAmount;
            #endregion

            // ---- end get data as print

            var boolstatus = false;
            byte[]? filebytes = null;
            try
            {
                var lang = SessionHelper.GetCurrentLanguage();
                var alldata_list = receiptVMs;
                var listtitles = new List<string>
                {
                    Resource2.ItemNumber,Resource2.IncomingAmount,Resource1.Description,Resource1.Destenation_Supplierss,Resource2.Date
                };
                if (alldata_list != null || alldata_list?.Count() > 0)
                {
                    var excelDataDTO = alldata_list.Select(single => new ExcelDataDTO
                    {
                        //t1 = lang == "ar" ? single.suppliernamear : single.suppliernameen,
                        t1 = single.ItemNumber,
                        t2 = single.Amount,
                        t3 = single.Notes,
                        t4 = (single.SupplierId.HasValue && suppliers.Where(x => x.Id == single.SupplierId.Value).FirstOrDefault()!=null) ? (lang=="ar"? suppliers.Where(x => x.Id == single.SupplierId.Value).FirstOrDefault().SupplierNameAr : suppliers.Where(x => x.Id == single.SupplierId.Value).FirstOrDefault().SupplierNameEn) : " ",
                        t5 = single.Date.HasValue ? single.Date.Value.ToString("d").Replace("/", "-") : "",
                    }).ToList();

                    var LastRaw = new ExcelDataDTO
                    {
                        t1 = Resource2.Total,
                        t2 = totalAmount,
                    };

                    excelDataDTO.Add(LastRaw);

                    if (lang == "ar")
                    {
                        (boolstatus, filebytes) = ExcelStaticReport.ExcelReportArEn_(excelDataDTO, listtitles, 0, "ar");
                    }
                    else
                    {
                        (boolstatus, filebytes) = ExcelStaticReport.ExcelReportArEn_(excelDataDTO, listtitles, 0, "en");
                    }
                }

                FileContentResult? excelfile = null;
                if (filebytes != null && filebytes.Length > 0 && boolstatus == true)
                {
                    var fileexcelname = Resource2.ReceiptsList;
                    excelfile = File(filebytes,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        $"{fileexcelname}_{AppDubaiTime.Now:yyyymmdd_hhmmss}.xlsx");
                }
                return excelfile;

            }
            catch (Exception)
            {
                return RedirectToAction("index");
            }
        }

        #endregion

        #region eXCEL aND pRINT _Expenses
        [YesGet]
        public async Task<IActionResult> Expenses(int? selectedBudgetItem, int? selectedExpenseGate, int? selectedExpenseSource, int? selectedSupplier, DateOnly? dateFrom, DateOnly? dateTo, int page = 1, int pageSize = 50)
        {
            var allRecivingReciptsHasSignned = await _unitOfWork.ReceivingReceipts.GetAllAsync(x=> x.ExpenseId != null && x.ManagerSignitureId!= null, c => c.ManagerSignature!) ?? new List<ReceivingReceipt>();
            var allIdsOfRecivingReciptsHasSignned = allRecivingReciptsHasSignned.Where(y=>y.ManagerSignature?.ImagePath?.Length >1).Select(x=>x.ExpenseId??-1).ToList();
            var allExpenses = await _expenseService.GetAllAsync();
            var expenseVMs = _mapper.Map<List<ExpenseAndReceiptVM>>(allExpenses).AsQueryable();

            if (selectedBudgetItem.HasValue)
                expenseVMs = expenseVMs.Where(e => e.BudgetItem != null && e.BudgetItem.Id == selectedBudgetItem);

            if (selectedExpenseGate.HasValue)
                expenseVMs = expenseVMs.Where(e => e.ExpensesGateId == selectedExpenseGate);
            
            if (selectedExpenseSource.HasValue)
                expenseVMs = expenseVMs.Where(e => e.ExpensesSourceId != null && e.ExpensesSourceId == selectedExpenseSource);
            if (selectedSupplier.HasValue)
                expenseVMs = expenseVMs.Where(e => e.Supplier != null && e.Supplier.Id == selectedSupplier);

            // ✅ Date filtering — replace `c.YourDateProperty` with your actual property
            if (dateFrom.HasValue)
            {
                expenseVMs = expenseVMs.Where(c => c.Date >= dateFrom.Value);
            }
            if (dateTo.HasValue)
            {
                expenseVMs = expenseVMs.Where(c => c.Date <= dateTo.Value);
            }

            var paginated = PaginatedList<ExpenseAndReceiptVM>.Create(expenseVMs, page, pageSize, "");

            var totalAmount = expenseVMs.Sum(e => e.Amount);
            var totalVat = expenseVMs.Sum(e => e.Vat);
            var totalWithVat = ((totalAmount ?? 0) + (totalVat ?? 0));

            if (paginated.Items.Count > 0)
            {
                paginated.Items[0].TotalAmount = totalAmount ?? 0;
                paginated.Items[0].TotalVAT = totalVat ?? 0;
                paginated.Items[0].TotalAmountWithVAT = ((totalAmount??0) + (totalVat??0));
            }

            #region fill filter dropdowns
            var budgetItems = await _unitOfWork.BudgetItems.GetAllAsync();
            var suppliers = await _unitOfWork.Suppliers.GetAllAsync();
            var ExpensesGates = await _unitOfWork.ExpensesGates.GetAllAsync();
            ExpensesGates = ExpensesGates.OrderBy(x => x.Id);
            var ExpensesSources = await _unitOfWork.ExpensesSources.GetAllAsync();
            ExpensesSources = ExpensesSources.OrderBy(x => x.Id);
            ViewBag.BudgetItemsList = SelectListHelper.BindSelectList(budgetItems.ToList(), selectedBudgetItem, "Id", "ItemTitle", "ItemTitle").ToList();
            ViewBag.ExpensesGateList = SelectListHelper.BindSelectList(ExpensesGates.ToList(), selectedExpenseGate, "Id", "NameAr", "NameEn").ToList();
            ViewBag.SuppliersList = SelectListHelper.BindSelectList(suppliers.ToList(), selectedSupplier, "Id", "SupplierNameAr", "SupplierNameEn").ToList();
            ViewBag.ExpensesSourcesList = SelectListHelper.BindSelectList(ExpensesSources.ToList(), selectedExpenseSource /* selected value */, "Id", "NameAr", "NameEn").ToList();
            #endregion

            ViewBag.dateFrom = dateFrom?.ToString("yyyy-MM-dd");
            ViewBag.dateTo = dateTo?.ToString("yyyy-MM-dd");
            ViewBag.allIdsOfRecivingReciptsHasSignned = allIdsOfRecivingReciptsHasSignned;

            // Calling from Ajax Return PartialView
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_ExpensesListPartial", paginated);
            }

            return View(paginated);
        }
        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> Print_Expenses(int? selectedBudgetItem, int? selectedExpenseGate, int? selectedExpenseSource, int? selectedSupplier, DateOnly? dateFrom, DateOnly? dateTo)
        {
            var allRecivingReciptsHasSignned = await _unitOfWork.ReceivingReceipts.GetAllAsync(x => x.ExpenseId != null && x.ManagerSignitureId != null, c => c.ManagerSignature!) ?? new List<ReceivingReceipt>();
            var allIdsOfRecivingReciptsHasSignned = allRecivingReciptsHasSignned.Where(y => y.ManagerSignature?.ImagePath?.Length > 1).Select(x => x.ExpenseId ?? -1).ToList();
            var allExpenses = await _expenseService.GetAllAsync();
            var expenseVMs = _mapper.Map<List<ExpenseAndReceiptVM>>(allExpenses).AsQueryable();

            if (selectedBudgetItem.HasValue)
                expenseVMs = expenseVMs.Where(e => e.BudgetItem != null && e.BudgetItem.Id == selectedBudgetItem);

            if (selectedExpenseGate.HasValue)
                expenseVMs = expenseVMs.Where(e => e.ExpensesGateId == selectedExpenseGate);

            if (selectedSupplier.HasValue)
                expenseVMs = expenseVMs.Where(e => e.Supplier != null && e.Supplier.Id == selectedSupplier);

            if (selectedExpenseSource.HasValue)
                expenseVMs = expenseVMs.Where(e => e.ExpensesSourceId != null && e.ExpensesSourceId == selectedExpenseSource);

            // ✅ Date filtering — replace `c.YourDateProperty` with your actual property
            if (dateFrom.HasValue)
            {
                expenseVMs = expenseVMs.Where(c => c.Date >= dateFrom.Value);
            }
            if (dateTo.HasValue)
            {
                expenseVMs = expenseVMs.Where(c => c.Date <= dateTo.Value);
            }

            var totalAmount = expenseVMs.Sum(e => e.Amount);
            var totalVat = expenseVMs.Sum(e => e.Vat);
            var totalWithVat = ((totalAmount ?? 0) + (totalVat ?? 0));


            ViewBag.totalAmount = totalAmount;
            ViewBag.totalVat = totalVat;
            ViewBag.totalWithVat = totalWithVat;
            return View(expenseVMs);
        }
        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> createExcelReport_Download_Expenses(int? selectedBudgetItem, int? selectedExpenseGate, int? selectedExpenseSource, int? selectedSupplier, DateOnly? dateFrom, DateOnly? dateTo)
        {
            var allRecivingReciptsHasSignned = await _unitOfWork.ReceivingReceipts.GetAllAsync(x => x.ExpenseId != null && x.ManagerSignitureId != null, c => c.ManagerSignature!) ?? new List<ReceivingReceipt>();
            var allIdsOfRecivingReciptsHasSignned = allRecivingReciptsHasSignned.Where(y => y.ManagerSignature?.ImagePath?.Length > 1).Select(x => x.ExpenseId ?? -1).ToList();
            var allExpenses = await _expenseService.GetAllAsync();
            var expenseVMs = _mapper.Map<List<ExpenseAndReceiptVM>>(allExpenses).AsQueryable();

            if (selectedBudgetItem.HasValue)
                expenseVMs = expenseVMs.Where(e => e.BudgetItem != null && e.BudgetItem.Id == selectedBudgetItem);

            if (selectedExpenseGate.HasValue)
                expenseVMs = expenseVMs.Where(e => e.ExpensesGateId == selectedExpenseGate);

            if (selectedSupplier.HasValue)
                expenseVMs = expenseVMs.Where(e => e.Supplier != null && e.Supplier.Id == selectedSupplier);

            if (selectedExpenseSource.HasValue)
                expenseVMs = expenseVMs.Where(e => e.ExpensesSourceId != null && e.ExpensesSourceId == selectedExpenseSource);

            // ✅ Date filtering — replace `c.YourDateProperty` with your actual property
            if (dateFrom.HasValue)
            {
                expenseVMs = expenseVMs.Where(c => c.Date >= dateFrom.Value);
            }
            if (dateTo.HasValue)
            {
                expenseVMs = expenseVMs.Where(c => c.Date <= dateTo.Value);
            }

            var totalAmount = expenseVMs.Sum(e => e.Amount);
            var totalVat = expenseVMs.Sum(e => e.Vat);
            var totalWithVat = ((totalAmount ?? 0) + (totalVat ?? 0));


            #region fill filter dropdowns
            var budgetItems = await _unitOfWork.BudgetItems.GetAllAsync();
            var suppliers = await _unitOfWork.Suppliers.GetAllAsync();

            var ExpensesGates = await _unitOfWork.ExpensesGates.GetAllAsync();
            ExpensesGates = ExpensesGates.OrderBy(x => x.Id);
            var ExpensesSources = await _unitOfWork.ExpensesSources.GetAllAsync();
            ExpensesSources = ExpensesSources.OrderBy(x => x.Id);


            ViewBag.BudgetItemsList = SelectListHelper.BindSelectList(budgetItems.ToList(), selectedBudgetItem, "Id", "ItemTitle", "ItemTitle").ToList();
            ViewBag.ExpensesGateList = SelectListHelper.BindSelectList(ExpensesGates.ToList(), selectedExpenseGate, "Id", "NameAr", "NameEn").ToList();
            ViewBag.SuppliersList = SelectListHelper.BindSelectList(suppliers.ToList(), selectedSupplier, "Id", "SupplierNameAr", "SupplierNameEn").ToList();
            #endregion

            ViewBag.dateFrom = dateFrom?.ToString("yyyy-MM-dd");
            ViewBag.dateTo = dateTo?.ToString("yyyy-MM-dd");

            // ---- end get data as print

            var boolstatus = false;
            byte[]? filebytes = null;
            var pathnewfile = "";
            try
            {
                var lang = SessionHelper.GetCurrentLanguage();
                var alldata_list = expenseVMs;
                var listtitles = new List<string>
                {
                    Resource2.ItemNumber,Resource2.SpentAmount,"VAT 5%",Resource2.TotalAmount,Resource2.ExpenseNotes,Resource2.Item,
                    Resource2.Date,
                };
                if (alldata_list != null || alldata_list?.Count() > 0)
                {
                    var excelDataDTO = alldata_list.Select(single => new ExcelDataDTO
                    {
                        //t1 = lang == "ar" ? single.suppliernamear : single.suppliernameen,
                        t1 = single.ItemNumber,
                        t2 = single.Amount,
                        t3 = single.Vat,
                        t4 = ((single.Amount??0) + (single.Vat??0)),
                        t5 = single.Notes,
                        t6 = single.BudgetItem != null ? single.BudgetItem.ItemTitle : " ",
                        t7 = single.Date.HasValue ? single.Date.Value.ToString("d").Replace("/","-") : "",
                    }).ToList();

                    var LastRaw = new ExcelDataDTO
                    {
                        t1 = Resource2.Total,
                        t2 = totalAmount,
                        t3 = totalVat,
                        t4 = totalWithVat,
                    };

                    excelDataDTO.Add(LastRaw);

                    if (lang == "ar")
                    {
                        (boolstatus, filebytes) = ExcelStaticReport.ExcelReportArEn_(excelDataDTO, listtitles, 0, "ar");
                    }
                    else
                    {
                        (boolstatus, filebytes) = ExcelStaticReport.ExcelReportArEn_(excelDataDTO, listtitles, 0, "en");
                    }
                }

                FileContentResult? excelfile = null;
                if (filebytes != null && filebytes.Length > 0 && boolstatus == true)
                {
                    var fileexcelname = Resource2.ExpensesList;
                    excelfile = File(filebytes,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        $"{fileexcelname}_{AppDubaiTime.Now:yyyymmdd_hhmmss}.xlsx");
                }
                return excelfile;

            }
            catch (Exception ex)
            {
                return RedirectToAction("index");
            }
        }

        #endregion

        #region eXCEL aND pRINT _ExpensesReport
        [YesGet]
        public async Task<IActionResult> ExpensesReport(int? selectedYear, int? selectedMonth, int page = 1, int pageSize = 50)
        {
            var lang = SessionHelper.GetCurrentLanguage();
            var expensesReport = await _expenseService.GetAllExpensesReportAsync(lang, selectedYear ?? 0, selectedMonth ?? 0);
            var expensesReportVM = _mapper.Map<ExpensesReportVM>(expensesReport);

            var years = await _expenseService.GetAllYearsInDb();
            var months = Enumerable.Range(1, 12).Select(m => new SelectListItem
            {
                Value = m.ToString(),
                Text = new DateTime(1, m, 1).ToString("MMMM")
            }).ToList();

            ViewBag.Years = years.ToList();
            ViewBag.Months = months;

            ViewBag.SelectedYear = selectedYear;
            ViewBag.SelectedMonth = selectedMonth;

            var paginated = PaginatedList<ExpensesReportElementVM>.Create(expensesReportVM.Expenses ?? [], page, pageSize, "");

            expensesReportVM.PaginatedExpenses = paginated;
            expensesReportVM.Expenses = null;
            expensesReportVM.IsAbleToOpen = expensesReportVM.PaginatedExpenses != null && expensesReportVM.PaginatedExpenses.Items.Any() ? true : false;

            // Calling from Ajax Return PartialView
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_ExpensesReportListPartial", expensesReportVM);
            }

            return View(expensesReportVM);
        }
        [YesGet]
        public async Task<IActionResult> OpenDetails_ExpensesReport(int? selectedYear, int? selectedMonth)
        {
            var lang = SessionHelper.GetCurrentLanguage();
            var expensesReport = await _expenseService.GetAllExpensesReportAsync(lang, selectedYear ?? 0, selectedMonth ?? 0);
            var expensesReportVM = _mapper.Map<ExpensesReportVM>(expensesReport);
            expensesReportVM.IsAbleToOpen = expensesReportVM.Expenses != null && expensesReportVM.Expenses.Any() ? true : false;
            expensesReportVM.year = expensesReport.Year;
            expensesReportVM.month = expensesReport.Month;
            if (!expensesReportVM.IsAbleToOpen)
            {
                return RedirectToAction("ExpensesReport", new { selectedYear, selectedMonth });
            }
            return View(expensesReportVM);
        }


        [IgnoreAction]
        [HttpPost]
        public async Task<IActionResult> ValidateOtp_OpenDetails_ExpensesReport([FromBody] OtpValidationRequest request)
        {
            // request: { year, month, code, role }
            var result = await _expenseService.ValidateOtp_OpenDetails_ExpensesReportAsync(request.Year.Value, request.Month.Value, request.Code, request.Role, User);
            var report = await _unitOfWork.ExpensesAndReciptReportSigns.GetByColumnAsync(
                e => e.Year == request.Year && e.Month == request.Month && e.ReportTypeId == (int)ReportTypeEnum.ExpensesReport);

            //var report = await _expenseService.get
            if (result.success == true)
            {
                if (request.Role == "Acountant")
                {
                    await _hubContext.Clients.Groups("Manager")
                        .SendAsync("ReceiveNotification", new
                        {
                            Title = "",
                            Message = ""
                        });
                    await _notificationService.SendNotificationToRoleAsync(
                          "تقرير للمصروفات النثرية جديد",
                          $"يوجد تقرير للمصروفات النثرية تاريخ {report?.Month + " - " + report?.Year} جديد جاهزة للإعتماد",
                          (int)RoleNumber.Manager
                      );
                }
            }
            return Json(new { success = result.success, message = result.message });
        }

        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> Print_ExpensesReport(int? selectedYear, int? selectedMonth)
        {
            var lang = SessionHelper.GetCurrentLanguage();
            var expensesReport = await _expenseService.GetAllExpensesReportAsync(lang, selectedYear ?? 0, selectedMonth ?? 0);
            var expensesReportVM = _mapper.Map<ExpensesReportVM>(expensesReport);

            return View(expensesReportVM);
        }
        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> createExcelReport_Download_ExpensesReport(int? selectedYear, int? selectedMonth)
        {
            var lang = SessionHelper.GetCurrentLanguage();
            var expensesReport = await _expenseService.GetAllExpensesReportAsync(lang, selectedYear ?? 0, selectedMonth ?? 0);
            var expensesReportVM = _mapper.Map<ExpensesReportVM>(expensesReport);

            // ---- end get data as print

            var boolstatus = false;
            byte[]? filebytes = null;
            var pathnewfile = "";
            try
            {
                lang = SessionHelper.GetCurrentLanguage();
                var alldata_list = expensesReportVM.Expenses;
                var listtitles = new List<string>
                {
                    Resource2.ItemNumber,Resource2.Date,Resource1.Description,Resource2.CompanyName,Resource2.ExpensesType,Resource2.AmountWithoutVAT,
                    Resource2.VAT,Resource2.AmountWithVAT,Resource2.Balance
                };
                if (alldata_list != null || alldata_list?.Count() > 0)
                {
                    var excelDataDTO = alldata_list.Select(single => new ExcelDataDTO
                    {
                        //t1 = lang == "ar" ? single.suppliernamear : single.suppliernameen,
                        t1 = single.ItemNumber,
                        t2 = single.Date.ToString("d")?.Replace("/","-"),
                        t3 = single.Notes,
                        t4 = single.SupplierName,
                        t5 = single.BudgetItem,
                        t6 = single.Amount,
                        t7 = single.Vat,
                        t8 = single.AmountWithVat,
                        t9 = single.Balance,
                    }).ToList();

                    var FirstRaw = new ExcelDataDTO
                    {
                        t3 = Resource2.BeginningBalance,
                        t9 = expensesReportVM.BeginningBalance,
                    };
                    var LastRaw = new ExcelDataDTO
                    {
                        t3 = Resource2.EndingBalance,
                        t9 = expensesReportVM.EndingBalance,
                    };

                    excelDataDTO.Insert(0, FirstRaw);
                    excelDataDTO.Add(LastRaw);
                    if (lang == "ar")
                    {
                        (boolstatus, filebytes) = ExcelStaticReport.ExcelReportArEn_(excelDataDTO, listtitles, 0, "ar");
                    }
                    else
                    {
                        (boolstatus, filebytes) = ExcelStaticReport.ExcelReportArEn_(excelDataDTO, listtitles, 0, "en");
                    }
                }

                FileContentResult? excelfile = null;
                if (filebytes != null && filebytes.Length > 0 && boolstatus == true)
                {
                    var fileexcelname = Resource2.PettyCashReport;
                    excelfile = File(filebytes,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        $"{fileexcelname}_{AppDubaiTime.Now:yyyymmdd_hhmmss}.xlsx");
                }
                return excelfile;

            }
            catch (Exception ex)
            {
                return RedirectToAction("index");
            }
        }

        #endregion

        #region eXCEL aND pRINT _ExpensesAndReceiptsReport
        [YesGet]
        public async Task<IActionResult> ExpensesAndReceiptsReport(int? selectedYear, int? selectedMonth, int page = 1, int pageSize = 50)
        {
            var lang = SessionHelper.GetCurrentLanguage();
            var expenseAndReceiptReport = await _expenseService.GetAllExpenseAndReceiptReportAsync(lang, selectedYear ?? 0, selectedMonth ?? 0);
            var expenseAndReceiptReportVM = _mapper.Map<ExpensesAndReceiptsReportVM>(expenseAndReceiptReport);

            var years = await _expenseService.GetAllYearsOfExpenseAndReceiptReportInDb();
            var months = Enumerable.Range(1, 12).Select(m => new SelectListItem
            {
                Value = m.ToString(),
                Text = new DateTime(1, m, 1).ToString("MMMM")
            }).ToList();

            ViewBag.Years = years.ToList();
            ViewBag.Months = months;

            ViewBag.SelectedYear = selectedYear;
            ViewBag.SelectedMonth = selectedMonth;

            var paginated = PaginatedList<ExpensesAndReceiptsReportElementVM>.Create(expenseAndReceiptReportVM.ExpensesAndReceipts ?? [], page, pageSize, "");

            expenseAndReceiptReportVM.PaginatedExpensesAndReceipts = paginated;
            expenseAndReceiptReportVM.ExpensesAndReceipts = null;
            expenseAndReceiptReportVM.IsAbleToOpen = expenseAndReceiptReportVM.PaginatedExpensesAndReceipts != null && expenseAndReceiptReportVM.PaginatedExpensesAndReceipts.Items.Any() ? true : false;

            // Calling from Ajax Return PartialView
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_ExpensesAndReceiptsReportListPartial", expenseAndReceiptReportVM);
            }

            return View(expenseAndReceiptReportVM);
        }

        [YesGet]
        public async Task<IActionResult> OpenDetails_ExpensesAndReciptReport(int? selectedYear, int? selectedMonth)
        {
            var lang = SessionHelper.GetCurrentLanguage();
            var expenseAndReceiptReport = await _expenseService.GetAllExpenseAndReceiptReportAsync(lang, selectedYear ?? 0, selectedMonth ?? 0);
            var expenseAndReceiptReportVM = _mapper.Map<ExpensesAndReceiptsReportVM>(expenseAndReceiptReport);

            expenseAndReceiptReportVM.IsAbleToOpen = expenseAndReceiptReportVM.ExpensesAndReceipts != null && expenseAndReceiptReportVM.ExpensesAndReceipts.Any() ? true : false;
            expenseAndReceiptReportVM.year = expenseAndReceiptReport.Year;
            expenseAndReceiptReportVM.month = expenseAndReceiptReport.Month;
            if (!expenseAndReceiptReportVM.IsAbleToOpen)
            {
                return RedirectToAction("ExpensesAndReceiptsReport", new { selectedYear, selectedMonth });
            }
            return View(expenseAndReceiptReportVM);
        }


        [IgnoreAction]
        [HttpPost]
        public async Task<IActionResult> ValidateOtp_OpenDetails_ExpensesAndReciptReport([FromBody] OtpValidationRequest request)
        {
            // request: { year, month, code, role }
            var result = await _expenseService.ValidateOtp_OpenDetails_ExpensesAndReciptReportAsync(request.Year.Value, request.Month.Value, request.Code, request.Role, User);
            var report = await _unitOfWork.ExpensesAndReciptReportSigns.GetByColumnAsync(
                e => e.Year == request.Year && e.Month == request.Month && e.ReportTypeId == (int)ReportTypeEnum.ExpensesAndReceiptsReport);
            //var report = await _expenseService.get
            if (result.success == true)
            {
                if (request.Role == "Acountant")
                {
                    await _hubContext.Clients.Groups("Manager")
                        .SendAsync("ReceiveNotification", new
                        {
                            Title = "",
                            Message = ""
                        });
                    await _notificationService.SendNotificationToRoleAsync(
                          "تقرير للمصروفات والمقبوضات جديد",
                          $"يوجد تقرير للمصروفات والمقبوضات تاريخ {report?.Month + " - " + report?.Year} جديد جاهزة للإعتماد",
                          (int)RoleNumber.Manager
                      );
                }
            }
            return Json(new { success = result.success, message = result.message });
        }

        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> Print_ExpensesAndReceiptsReport(int? selectedYear, int? selectedMonth, int page = 1, int pageSize = 500)
        {
            var lang = SessionHelper.GetCurrentLanguage();
            var expenseAndReceiptReport = await _expenseService.GetAllExpenseAndReceiptReportAsync(lang, selectedYear ?? 0, selectedMonth ?? 0);
            var expenseAndReceiptReportVM = _mapper.Map<ExpensesAndReceiptsReportVM>(expenseAndReceiptReport);

            return View(expenseAndReceiptReportVM);
        }
        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> createExcelReport_Download_ExpensesAndReceiptsReport(int? selectedYear, int? selectedMonth, int page = 1, int pageSize = 500)
        {
            var lang = SessionHelper.GetCurrentLanguage();
            var expenseAndReceiptReport = await _expenseService.GetAllExpenseAndReceiptReportAsync(lang, selectedYear ?? 0, selectedMonth ?? 0);
            var expenseAndReceiptReportVM = _mapper.Map<ExpensesAndReceiptsReportVM>(expenseAndReceiptReport);

            // ---- end get data as print

            var boolstatus = false;
            byte[]? filebytes = null;
            var pathnewfile = "";
            try
            {
                lang = SessionHelper.GetCurrentLanguage();
                var alldata_list = expenseAndReceiptReportVM.ExpensesAndReceipts;
                var listtitles = new List<string>
                {
                    Resource2.Date,Resource1.Description,Resource1.DocumentAuthority,
                    Resource2.Deposit,Resource2.Withdrawal,Resource2.Balance,
                };
                if (alldata_list != null || alldata_list?.Count() > 0)
                {
                    var excelDataDTO = alldata_list.Select(single => new ExcelDataDTO
                    {
                        //t1 = lang == "ar" ? single.suppliernamear : single.suppliernameen,
                        t1 = single.Date,
                        t2 = single.Notes,
                        t3 = single.SupplierName,
                        t4 = single.Deposit,
                        t5 = single.Withdrawal,
                        t6 = single.Balance,
                    }).ToList();

                    var FirstRaw = new ExcelDataDTO
                    {
                        t2 = Resource2.BeginningBalance,
                        t6 = expenseAndReceiptReportVM.BeginningBalance,
                    };
                    var LastRaw = new ExcelDataDTO
                    {
                        t2 = Resource2.EndingBalance,
                        t6 = expenseAndReceiptReportVM.EndingBalance,
                    };

                    excelDataDTO.Insert(0, FirstRaw);
                    excelDataDTO.Add(LastRaw);
                    if (lang == "ar")
                    {
                        (boolstatus, filebytes) = ExcelStaticReport.ExcelReportArEn_(excelDataDTO, listtitles, 0, "ar");
                    }
                    else
                    {
                        (boolstatus, filebytes) = ExcelStaticReport.ExcelReportArEn_(excelDataDTO, listtitles, 0, "en");
                    }
                }

                FileContentResult? excelfile = null;
                if (filebytes != null && filebytes.Length > 0 && boolstatus == true)
                {
                    var fileexcelname = Resource2.ReceiptsAndPaymentsReport;
                    excelfile = File(filebytes,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        $"{fileexcelname}_{AppDubaiTime.Now:yyyymmdd_hhmmss}.xlsx");
                }
                return excelfile;

            }
            catch (Exception ex)
            {
                return RedirectToAction("index");
            }
        }
        #endregion


        #region Receipt

        public async Task<IActionResult> Receipt(int? expenseId, int? receiptId)
        {
            var vm = new ReceivingReceiptVM();

            // Determine the target id (either an expense or a receipt record id)
            var targetId = expenseId ?? receiptId;
            if (!targetId.HasValue)
                return BadRequest();

            // Try to find an existing ReceivingReceipt for the target id
            var receivingReceipt = await _expenseService.GetReceivingReceiptByIdAsync(targetId.Value);
            if (receivingReceipt == null)
            {
                // No existing receiving receipt — prefill from Expense or Receipt depending on which id was supplied
                if (expenseId.HasValue)
                {
                    var expense = await _expenseService.GetByIdAsync(expenseId.Value);
                    if (expense != null)
                    {
                        vm.ExpenseId = expenseId.Value;
                        vm.FinalAmount = (expense.Amount ?? 0) + (expense.Vat ?? 0);
                        vm.Date = expense.Date;
                        vm.Notes = expense.Notes;
                        vm.ItemNumber = expense.ItemNumber;
                        vm.itemType = ItemType.Expenses;
                    }
                }
                else if (receiptId.HasValue)
                {
                    var receipt = await _receiptService.GetByIdAsync(receiptId.Value);
                    if (receipt != null)
                    {
                        vm.ExpenseId = receiptId.Value; // use ExpenseId field to link to the receipt record
                        vm.FinalAmount = (receipt.Amount ?? 0);
                        vm.Date = receipt.Date;
                        vm.Notes = receipt.Notes;
                        vm.ItemNumber = receipt.ItemNumber;
                        vm.itemType = ItemType.Receipts;
                    }
                }

                // Auto-increment ItemNumber based on numeric parts of existing ReceivingReceipts
                var allReceipts = await _unitOfWork.ReceivingReceipts.GetAllAsync();
                int maxNumber = 0;
                if (allReceipts != null && allReceipts.Any())
                {
                    foreach (var r in allReceipts)
                    {
                        if (!string.IsNullOrWhiteSpace(r.ItemNumber) && int.TryParse(r.ItemNumber, out var num))
                        {
                            if (num > maxNumber) maxNumber = num;
                        }
                    }
                }
                if (string.IsNullOrWhiteSpace(vm.ItemNumber))
                    vm.ItemNumber = (maxNumber + 1).ToString();

                // Prefill the next serial code (auto-incremented on save) so the user sees it before saving
                try
                {
                    var nextSerial = vm.itemType == ItemType.Expenses
                        ? await _expenseService.GetLastSerialCode()
                        : await _receiptService.GetLastSerialCode();
                    vm.CodeSerial = nextSerial;
                }
                catch
                {
                    // ignore errors and leave CodeSerial empty
                }

                vm.isSavedFull = !(string.IsNullOrWhiteSpace(vm.Being) || string.IsNullOrWhiteSpace(vm.SumOfAmount) || string.IsNullOrWhiteSpace(vm.Payment) || string.IsNullOrWhiteSpace(vm.Received) ||
                                  string.IsNullOrWhiteSpace(vm.ItemNumber) || vm?.Date == null || !(vm.Amount > 0) || !(vm.FinalAmount > 0));

                return View(vm);
            }

            vm = _mapper.Map<ReceivingReceiptVM>(receivingReceipt);
            // If receiving receipt exists but FinalAmount is not set, populate from related expense/receipt
            if ((vm.FinalAmount == null || vm.FinalAmount == 0) && receivingReceipt.ExpenseId.HasValue)
            {
                // Try expense first
                var expense = await _expenseService.GetByIdAsync(receivingReceipt.ExpenseId.Value);
                if (expense != null)
                {
                    vm.FinalAmount = (expense.Amount ?? 0) + (expense.Vat ?? 0);
                    vm.itemType = ItemType.Expenses;
                }
                else
                {
                    var receipt = await _receiptService.GetByIdAsync(receivingReceipt.ExpenseId.Value);
                    if (receipt != null)
                    {
                        vm.FinalAmount = (receipt.Amount ?? 0);
                        vm.itemType = ItemType.Receipts;
                    }
                }
            }

            vm.isSavedFull = !(string.IsNullOrWhiteSpace(vm.Being) || string.IsNullOrWhiteSpace(vm.SumOfAmount) || string.IsNullOrWhiteSpace(vm.Payment) || string.IsNullOrWhiteSpace(vm.Received) ||
                              string.IsNullOrWhiteSpace(vm.ItemNumber) || vm?.Date == null || !(vm.Amount > 0) || !(vm.FinalAmount > 0));

            return View(vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Receipt(ReceivingReceiptVM model)
        {
            if (model == null) return NotFound();

            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var entity = _mapper.Map<ReceivingReceipt>(model);

            if (model.Id == 0)
            {
                var newSerilaCode = model.itemType==ItemType.Expenses ? await _expenseService.GetLastSerialCode() : await _receiptService.GetLastSerialCode();
                entity.CodeSerial = newSerilaCode;
                await _expenseService.AddAsync(entity);
                await _hubContext.Clients.Groups("Manager")
   .SendAsync("ReceiveNotification", new
   {
       Title = "",
       Message = ""
   });
                await _notificationService.SendNotificationToRoleAsync(

      "ايصال استلام جديد",
      $"يوجد ايصال استلام رقم {newSerilaCode} جديد جاهز للإعتماد",

      2
  );
                //return RedirectToAction(nameof(Expenses));
            }
            else
            {
                var exist = await _expenseService.GetReceivingReceiptByIdAsync(entity.Id);
                entity.CodeSerial = exist != null ? exist.CodeSerial : entity.CodeSerial;
                await _expenseService.UpdateAsync(entity);
                //return RedirectToAction(nameof(Receipt), new { expenseId = model.ExpenseId });
            }
            return RedirectToAction(nameof(Receipt), new { expenseId = model.ExpenseId });

        }
        #region sms approval
        [IgnoreAction]
        [NoLogging]
        [HttpPost]
        public async Task<IActionResult> SendOtp() // GetSignature
        {
            try
            {
                var statusResult =  await _expenseService.SendOtpAsync();
                return Json(new { success = statusResult });
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

            bool isValid = await _expenseService.ValidateOtpAsync((int)request.Id, request.Code);

            if (isValid)
                return Json(new { success = true });

            return Json(new { success = false, message = "Incorrect OTP code." });
        }

        #endregion

        #endregion





    }
}
