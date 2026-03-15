using Application.Helpers;
using Application.Interfaces.Admin;
using Application.Services.Admin;
using AutoMapper;
using Domain.DTOs;
using Domain.Entities.BudgetItem;
using Domain.Resources;
using FougeraClub.Areas.Admin.ViewModels.BudgetItem;
using FougeraClub.Attributes;
using FougeraClub.Helpers;
using FougeraClub.Middelware;
using Microsoft.AspNetCore.Mvc;

namespace FougeraClub.Areas.Admin.Controllers
{
    [AdminAuthorize]
    [Area("Admin")]
    public class BudgetItemController : Controller
    {
        private readonly IBudgetItemService _budgetItemService;
        private readonly IMapper _mapper;

        public BudgetItemController(IBudgetItemService budgetItemService, IMapper mapper)
        {
            _budgetItemService = budgetItemService;
            _mapper = mapper;
        }
        [YesGet]
        public async Task<IActionResult> Index(string? searchTerm, int page = 1, int pageSize = 50)
        {
            ViewBag.DeletionFailure = TempData["DeletionFailure"] ?? null;
            var allBudgetItems = await _budgetItemService.GetAllAsync();
            var budgetItemVMs = _mapper.Map<List<BudgetItemVM>>(allBudgetItems).AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                int searchNumberInt;
                bool isNumber = Int32.TryParse(searchTerm, out searchNumberInt);

                budgetItemVMs = budgetItemVMs.Where(s =>
                    (!string.IsNullOrEmpty(s.ItemTitle) && s.ItemTitle.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    (s.ItemNumber == searchNumberInt)
                    )
                );
            }

            var paginated = PaginatedList<BudgetItemVM>.Create(budgetItemVMs.OrderByDescending(m => m.Id), page, pageSize, searchTerm);

            // Calling from Ajax Return PartialView
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_ListPartial", paginated);
            }

            return View(paginated);
        }

        public async Task<IActionResult> AddEdit(int? id)
        {
            var vm = new BudgetItemVM();
            if (id != null && id != 0)
            {
                var budgetItem = await _budgetItemService.GetByIdAsync(id.Value);
                if (budgetItem == null) return NotFound();
                return View(_mapper.Map<BudgetItemVM>(budgetItem));
            }
            else
            {
                vm.ItemNumber = await _budgetItemService.GenerateNewCode();
            }
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEdit(BudgetItemVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var entity = _mapper.Map<BudgetItem>(model);

            if (model.Id == 0)
            {
                model.Id = await _budgetItemService.AddAsync(entity);
                return RedirectToAction(nameof(Index)); // After Add New
            }
            else
                await _budgetItemService.UpdateAsync(entity);

            return RedirectToAction(nameof(AddEdit), new{id= model.Id }); // After Edit 
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _budgetItemService.DeleteAsync(id);
            if (!result)
                TempData["DeletionFailure"] = "true";
            return RedirectToAction(nameof(Index));
        }
        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> Print(string? searchTerm, int page = 1, int pageSize = 50)
        {
            var allBudgetItems = await _budgetItemService.GetAllAsync();
            var budgetItemVMs = _mapper.Map<List<BudgetItemVM>>(allBudgetItems).AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                int searchNumberInt;
                bool isNumber = Int32.TryParse(searchTerm, out searchNumberInt);

                budgetItemVMs = budgetItemVMs.Where(s =>
                    (!string.IsNullOrEmpty(s.ItemTitle) && s.ItemTitle.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                    (s.ItemNumber == searchNumberInt)
                    )
                );
            }
            return View(budgetItemVMs.OrderByDescending(m => m.Id));
        }
        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> createExcelReport_Download(string? searchTerm, int page = 1, int pageSize = 50)
        {
            var allBudgetItems = await _budgetItemService.GetAllAsync();
            var budgetItemVMs = _mapper.Map<List<BudgetItemVM>>(allBudgetItems).AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                int searchNumberInt;
                bool isNumber = Int32.TryParse(searchTerm, out searchNumberInt);

                budgetItemVMs = budgetItemVMs.Where(s =>
                            (!string.IsNullOrEmpty(s.ItemTitle) && s.ItemTitle.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                            (s.ItemNumber == searchNumberInt)
                            )
                        );
            }

            // ---- End Get Data As Print

            var boolStatus = false;
            byte[]? fileBytes = null;
            var pathNewFile = "";
            try
            {
                var lang = SessionHelper.GetCurrentLanguage();
                var allData_list = budgetItemVMs.OrderByDescending(m => m.Id);
                var ListTitles = new List<string>
            {
                Resource2.BudgetItemNo,Resource2.ItemTitle,
            };
                if (allData_list != null || allData_list?.Count() > 0)
                {
                    var excelDataDTO = allData_list.Select(single => new ExcelDataDTO
                    {
                        t1 = single.ItemNumber,
                        t2 = single.ItemTitle,
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
                    var fileExcelName = Resource2.BudgetItemsList;
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
