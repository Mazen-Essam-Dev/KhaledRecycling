using Application.Helpers;
using Application.Interfaces.Admin;
using Application.Services.Admin;
using AutoMapper;
using Domain.DTOs;
using Domain.Entities;
using Domain.Resources;
using FougeraClub.Areas.Admin.ViewModels.Suppliers;
using FougeraClub.Attributes;
using FougeraClub.Helpers;
using FougeraClub.Middelware;
using Microsoft.AspNetCore.Mvc;

namespace FougeraClub.Areas.Admin.Controllers
{
    [AdminAuthorize]
    [Area("Admin")]
    public class SupplierController : Controller
    {
        private readonly ISupplierService _supplierService;
        private readonly IMapper _mapper;

        public SupplierController(ISupplierService supplierService, IMapper mapper)
        {
            _supplierService = supplierService;
            _mapper = mapper;
        }

        [YesGet]
        public async Task<IActionResult> Index(string? searchTerm, string? selectedType, int page = 1, int pageSize = 50)
        {
            var allSuppliers = await _supplierService.GetAllAsync();
            var supplierVMs = _mapper.Map<List<SupplierVM>>(allSuppliers).AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                supplierVMs = supplierVMs.Where(s =>
                    (!string.IsNullOrEmpty(s.SupplierNameAr) && s.SupplierNameAr.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(s.SupplierNameEn) && s.SupplierNameEn.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(s.Email) && s.Email.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(s.Phone) && s.Phone.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                );
            }
            if (!string.IsNullOrWhiteSpace(selectedType) && int.TryParse(selectedType, out int typeValue))
            {
                supplierVMs = supplierVMs.Where(s => s.SupplierCategoryId == typeValue);
            }
            ViewBag.TotalCount = supplierVMs.Count();

            var categories = await _supplierService.GetAllSupplierCategoryAsync();
            foreach(var cat in categories)
            {
                cat.NameAr = (!string.IsNullOrEmpty(cat.NameAr) && cat.NameAr.Contains("عام", StringComparison.OrdinalIgnoreCase)) ? "مورد" : cat.NameAr;
                cat.NameAr = (!string.IsNullOrEmpty(cat.NameAr) && cat.NameAr.Contains("خاص", StringComparison.OrdinalIgnoreCase)) ? "جهات" : cat.NameAr;
                cat.NameEn = (!string.IsNullOrEmpty(cat.NameEn) && cat.NameEn.Contains("General", StringComparison.OrdinalIgnoreCase)) ? "Supplier" : cat.NameEn;
                cat.NameEn = (!string.IsNullOrEmpty(cat.NameEn) && cat.NameEn.Contains("Special", StringComparison.OrdinalIgnoreCase)) ? "Institutions" : cat.NameEn;
            }
            ViewBag.TypeEnumList = SelectListHelper.BindSelectList(categories.OrderBy(c=>c.Id).ToList(), null, "Id", "NameAr", "NameEn").ToList();

            var paginated = PaginatedList<SupplierVM>.Create(supplierVMs, page, pageSize, searchTerm);

            var lang = SessionHelper.GetCurrentLanguage();
            foreach (var supplier in paginated.Items)
            {
                var category = categories.FirstOrDefault(c => c.Id == supplier.SupplierCategoryId);
                if (category != null)
                {
                    supplier.SupplierCategoryId = category.Id;
                    supplier.textCategory = lang == "ar" ? category.NameAr : category.NameEn;
                }
            }


            // Calling from Ajax Return PartialView
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_ListPartial", paginated);
            }

            return View(paginated);
        }
        [NoLogging]
        public async Task<IActionResult> AddEdit(int? id)
        {
            var vm = new SupplierVM();
            if (id == null || id == 0)
                return View(vm);

            var supplier = await _supplierService.GetByIdAsync(id.Value);
            if (supplier == null) return NotFound();

            vm = _mapper.Map<SupplierVM>(supplier);

            if (vm.Phone != null && vm.Phone.StartsWith("971")) // Is Phone StartsWith 971 Remove it
                vm.Phone = vm.Phone.Substring(3);
            if (vm.Mobile != null && vm.Mobile.StartsWith("971")) // Is Mobile StartsWith 971 Remove it
                vm.Mobile = vm.Mobile.Substring(3);

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> AddEdit(SupplierVM model)
        {
            if (!ModelState.IsValid)
            {
                if (model.Phone != null && model.Phone.StartsWith("971")) // Is Phone StartsWith 971 Remove it
                    model.Phone = model.Phone.Substring(3);
                if (model.Mobile != null && model.Mobile.StartsWith("971")) // Is Mobile StartsWith 971 Remove it
                    model.Mobile = model.Mobile.Substring(3);
                return View(model);
            }

            // //Phone Dubai
            // //Phone Dubai
            model.Phone = await PhoneHelper.CheckAndDoPhoneStart971(model.Phone);
            // //Mobile Dubai
            model.Mobile = await PhoneHelper.CheckAndDoPhoneStart971(model.Mobile);

            var entity = _mapper.Map<Supplier>(model);

            if (model.Id == 0)
            {
                model.Id = await _supplierService.AddAsync(entity);
                return RedirectToAction(nameof(Index)); // After Add New
            }
            else
                await _supplierService.UpdateAsync(entity);

            return RedirectToAction(nameof(AddEdit), new { model.Id }); // After Edit
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _supplierService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> Print(string? searchTerm, string? selectedType)
        {
            var allSuppliers = await _supplierService.GetAllAsync();
            var supplierVMs = _mapper.Map<List<SupplierVM>>(allSuppliers).AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                supplierVMs = supplierVMs.Where(s =>
                    (!string.IsNullOrEmpty(s.SupplierNameAr) && s.SupplierNameAr.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(s.SupplierNameEn) && s.SupplierNameEn.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(s.Email) && s.Email.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(s.Phone) && s.Phone.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                );
            }
            if (!string.IsNullOrWhiteSpace(selectedType) && int.TryParse(selectedType, out int typeValue))
            {
                supplierVMs = supplierVMs.Where(s => s.SupplierCategoryId == typeValue);
            }
            var categories = await _supplierService.GetAllSupplierCategoryAsync();
            foreach (var cat in categories)
            {
                cat.NameAr = (!string.IsNullOrEmpty(cat.NameAr) && cat.NameAr.Contains("عام", StringComparison.OrdinalIgnoreCase)) ? "مورد" : cat.NameAr;
                cat.NameAr = (!string.IsNullOrEmpty(cat.NameAr) && cat.NameAr.Contains("خاص", StringComparison.OrdinalIgnoreCase)) ? "جهات" : cat.NameAr;
                cat.NameEn = (!string.IsNullOrEmpty(cat.NameEn) && cat.NameEn.Contains("General", StringComparison.OrdinalIgnoreCase)) ? "Supplier" : cat.NameEn;
                cat.NameEn = (!string.IsNullOrEmpty(cat.NameEn) && cat.NameEn.Contains("Special", StringComparison.OrdinalIgnoreCase)) ? "Institutions" : cat.NameEn;
            }
            var lang = SessionHelper.GetCurrentLanguage();
            foreach (var supplier in supplierVMs)
            {
                var category = categories.FirstOrDefault(c => c.Id == supplier.SupplierCategoryId);
                if (category != null)
                {
                    supplier.SupplierCategoryId = category.Id;
                    supplier.textCategory = lang == "ar" ? category.NameAr : category.NameEn;
                }
            }
            return View(supplierVMs);
        }


        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> createExcelReport_Download(string? searchTerm, string? selectedType)
        {
            var allSuppliers = await _supplierService.GetAllAsync();
            var supplierVMs = _mapper.Map<List<SupplierVM>>(allSuppliers).AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                supplierVMs = supplierVMs.Where(s =>
                    (!string.IsNullOrEmpty(s.SupplierNameAr) && s.SupplierNameAr.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(s.SupplierNameEn) && s.SupplierNameEn.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(s.Email) && s.Email.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(s.Phone) && s.Phone.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                );
            }
            if (!string.IsNullOrWhiteSpace(selectedType) && int.TryParse(selectedType, out int typeValue))
            {
                supplierVMs = supplierVMs.Where(s => s.SupplierCategoryId == typeValue);
            }
            var categories = await _supplierService.GetAllSupplierCategoryAsync();
            foreach (var cat in categories)
            {
                cat.NameAr = (!string.IsNullOrEmpty(cat.NameAr) && cat.NameAr.Contains("عام", StringComparison.OrdinalIgnoreCase)) ? "مورد" : cat.NameAr;
                cat.NameAr = (!string.IsNullOrEmpty(cat.NameAr) && cat.NameAr.Contains("خاص", StringComparison.OrdinalIgnoreCase)) ? "جهات" : cat.NameAr;
                cat.NameEn = (!string.IsNullOrEmpty(cat.NameEn) && cat.NameEn.Contains("General", StringComparison.OrdinalIgnoreCase)) ? "Supplier" : cat.NameEn;
                cat.NameEn = (!string.IsNullOrEmpty(cat.NameEn) && cat.NameEn.Contains("Special", StringComparison.OrdinalIgnoreCase)) ? "Institutions" : cat.NameEn;
            }
            var lang = SessionHelper.GetCurrentLanguage();
            foreach (var supplier in supplierVMs)
            {
                var category = categories.FirstOrDefault(c => c.Id == supplier.SupplierCategoryId);
                if (category != null)
                {
                    supplier.SupplierCategoryId = category.Id;
                    supplier.textCategory = lang == "ar" ? category.NameAr : category.NameEn;
                }
            }
            // ---- End Get Data As Print

            var boolStatus = false;
            byte[]? fileBytes = null;
            var pathNewFile = "";
            try
            {
                lang = SessionHelper.GetCurrentLanguage();
                var allData_list = supplierVMs;
                var ListTitles = new List<string>
                {
                    Resource1.SupplierName,Resource1.Email,Resource1.Mobile,Resource1.VATNumber,Resource1.Type,
                };

                if (allData_list != null || allData_list?.Count() > 0)
                {
                    var excelDataDTO = allData_list.Select(single => new ExcelDataDTO
                    {
                        t1 = lang == "ar" ? single.SupplierNameAr : single.SupplierNameEn,
                        t2 = single.Email,
                        t3 = single.Mobile,
                        t4 = single.VATNumber,
                        t5 = single.textCategory,
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
                    var fileExcelName = Resource1.SuppliersList;
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
