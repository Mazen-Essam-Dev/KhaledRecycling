using Application.Helpers;
using Application.Interfaces.Admin;
using Application.Services.Admin;
using AutoMapper;
using Domain.DTOs;
using Domain.Entities;
using Domain.Resources;
using FougeraClub.Areas.Admin.ViewModels.Account;
using FougeraClub.Areas.Admin.ViewModels.AnnualSchedule;
using FougeraClub.Attributes;
using FougeraClub.Helpers;
using FougeraClub.Middelware;
using Infrastructure.Repositories.InterfacesDB;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace FougeraClub.Areas.Admin.Controllers
{
    [AdminAuthorize]
    [Area("Admin")]
    
    public class AnnualScheduleController : Controller
    {
        private readonly IAnnualScheduleService _annualScheduleService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AnnualScheduleController(IAnnualScheduleService annualScheduleService, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _annualScheduleService = annualScheduleService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        [YesGet]
        public async Task<IActionResult> Index(string? searchTerm, string? selectedYear, int? selectedCategory, int page = 1, int pageSize = 50)
        {
            var allSchedules = await _annualScheduleService.GetAllAsync();
            var allYears = await _unitOfWork.AnnualSchedules.Table.Select(x => x.year).Distinct().Where(x => x != null).OrderByDescending(x => x).ToListAsync();
            ViewBag.allYears = allYears;
            ViewBag.selectedYear = selectedYear;

            // Get categories for filter dropdown
            var categories = await _unitOfWork.AnnualScheduleCategories.GetAllAsync();
            ViewBag.Categories = categories.Select(c => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Value = c.Id.ToString(),
                Text = SessionHelper.GetCurrentLanguage() == "ar" ? c.NameAr : c.NameEn
            }).ToList();
            ViewBag.selectedCategory = selectedCategory;

            var scheduleVMs = _mapper.Map<List<AnnualScheduleVM>>(allSchedules).AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                scheduleVMs = scheduleVMs.Where(s =>
                    (!string.IsNullOrEmpty(s.Item) && s.Item.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(s.Statement) && s.Statement.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                );
            }

            if (!string.IsNullOrWhiteSpace(selectedYear))
            {
                scheduleVMs = scheduleVMs.Where(s =>
                    (!string.IsNullOrEmpty(s.year) && s.year.Contains(selectedYear, StringComparison.OrdinalIgnoreCase))
                );
            }

            if (selectedCategory.HasValue)
            {
                scheduleVMs = scheduleVMs.Where(s => s.AnnualScheduleCategoryId == selectedCategory.Value);
            }

            ViewBag.TotalCount = scheduleVMs.Count();
            var paginated = PaginatedList<AnnualScheduleVM>.Create(scheduleVMs.OrderByDescending(m => m.Id), page, pageSize, searchTerm);

            // Calling from Ajax Return PartialView
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_ListPartial", paginated);
            }

            return View(paginated);
        }

        public async Task<IActionResult> AddEdit(int? id)
        {
            var vm = new AnnualScheduleVM();
            
            // Get categories for dropdown
            var categories = await _unitOfWork.AnnualScheduleCategories.GetAllAsync();
            ViewBag.Categories = categories.Select(c => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Value = c.Id.ToString(),
                Text = SessionHelper.GetCurrentLanguage() == "ar" ? c.NameAr : c.NameEn
            }).ToList();
            
            if (id == null || id == 0)
                return View(vm);

            var schedule = await _annualScheduleService.GetByIdAsync(id.Value);
            if (schedule == null) return NotFound();

            if (SessionHelper.GetCurrentLanguage() == "ar")
                ViewBag.Daytext = schedule.Day?.ToString("dddd", new CultureInfo("ar-EG"));
            else
                ViewBag.Daytext = schedule.Day?.ToString("dddd", new CultureInfo("en-US"));

            vm = _mapper.Map<AnnualScheduleVM>(schedule);
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> AddEdit(AnnualScheduleVM model)
        {
            // Get categories for dropdown in case of validation error
            var categories = await _unitOfWork.AnnualScheduleCategories.GetAllAsync();
            ViewBag.Categories = categories.Select(c => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Value = c.Id.ToString(),
                Text = SessionHelper.GetCurrentLanguage() == "ar" ? c.NameAr : c.NameEn
            }).ToList();
            
            if (SessionHelper.GetCurrentLanguage() == "ar")
                ViewBag.Daytext = model.Day?.ToString("dddd", new CultureInfo("ar-EG"));
            else
                ViewBag.Daytext = model.Day?.ToString("dddd", new CultureInfo("en-US"));

            if (!ModelState.IsValid)
                return View(model);

            var entity = _mapper.Map<AnnualSchedule>(model);
            entity.year = entity.Day?.ToString("yyyy");

            if (model.Id == 0)
            {
                model.Id = await _annualScheduleService.AddAsync(entity);
                return RedirectToAction(nameof(Index)); // After Add New

            }
            else
                await _annualScheduleService.UpdateAsync(entity);

            return RedirectToAction(nameof(AddEdit), new { id = model.Id }); // After Edit 
        }

        [IgnoreAction]
        [HttpGet]
        [YesGet]
        public async Task<IActionResult> Details(int id)
        {
            if (id == 0) return NotFound();

            var schedule = await _unitOfWork.AnnualSchedules.GetByIdAsync(id);

            if (schedule == null) return NotFound();

            var vm = _mapper.Map<AnnualScheduleVM>(schedule);

            if(SessionHelper.GetCurrentLanguage()=="ar")
                ViewBag.Daytext = vm.Day?.ToString("dddd", new CultureInfo("ar-EG"));
            else
                ViewBag.Daytext = vm.Day?.ToString("dddd", new CultureInfo("en-US"));

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _annualScheduleService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> Print(string? searchTerm, string? selectedYear, int? selectedCategory, int page = 1, int pageSize = 10)
        {
            var allSchedules = await _annualScheduleService.GetAllAsync();
            var allYears = await _unitOfWork.AnnualSchedules.Table.Select(x => x.year).Distinct().ToListAsync();
            ViewBag.allYears = allYears;
            ViewBag.selectedYear = selectedYear;

            var scheduleVMs = _mapper.Map<List<AnnualScheduleVM>>(allSchedules).AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                scheduleVMs = scheduleVMs.Where(s =>
                    (!string.IsNullOrEmpty(s.Item) && s.Item.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(s.Statement) && s.Statement.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                );
            }

            if (!string.IsNullOrWhiteSpace(selectedYear))
            {
                scheduleVMs = scheduleVMs.Where(s =>
                    (!string.IsNullOrEmpty(s.year) && s.year.Contains(selectedYear, StringComparison.OrdinalIgnoreCase))
                );
            }

            if (selectedCategory.HasValue)
            {
                scheduleVMs = scheduleVMs.Where(s => s.AnnualScheduleCategoryId == selectedCategory.Value);
            }

            ViewBag.TotalCount = scheduleVMs.Count();

            return View(scheduleVMs.OrderByDescending(m => m.Id));
        }
        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> createExcelReport_Download(string? searchTerm, string? selectedYear, int? selectedCategory, int page = 1, int pageSize = 10)
        {
            var allSchedules = await _annualScheduleService.GetAllAsync();
            var allYears = await _unitOfWork.AnnualSchedules.Table.Select(x => x.year).Distinct().ToListAsync();
            ViewBag.allYears = allYears;
            ViewBag.selectedYear = selectedYear;

            var scheduleVMs = _mapper.Map<List<AnnualScheduleVM>>(allSchedules).AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                scheduleVMs = scheduleVMs.Where(s =>
                    (!string.IsNullOrEmpty(s.Item) && s.Item.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(s.Statement) && s.Statement.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                );
            }

            if (!string.IsNullOrWhiteSpace(selectedYear))
            {
                scheduleVMs = scheduleVMs.Where(s =>
                    (!string.IsNullOrEmpty(s.year) && s.year.Contains(selectedYear, StringComparison.OrdinalIgnoreCase))
                );
            }

            if (selectedCategory.HasValue)
            {
                scheduleVMs = scheduleVMs.Where(s => s.AnnualScheduleCategoryId == selectedCategory.Value);
            }

            ViewBag.TotalCount = scheduleVMs.Count();
            // ---- End Get Data As Print

            var boolStatus = false;
            byte[]? fileBytes = null;
            var pathNewFile = "";
            try
            {

                var lang = SessionHelper.GetCurrentLanguage();
                //var allActivitys = await _ActivityService.GetAllAsync();
                var allData_list = scheduleVMs.OrderByDescending(m => m.Id);
                var ListTitles = new List<string>
        {
            Resource1.Item,Resource1.Particular/*,Resource1.ExchangeStatement*/,Resource1.Previous,
            Resource1.Current,Resource1.Record,Resource1.Status1,"التصنيف",
            //Resource1.InventoryYear,
        };
                if (allData_list != null || allData_list?.Count() > 0)
                {
                    var excelDataDTO = allData_list.Select(single => new ExcelDataDTO
                    {
                        t1 = single.Item,
                        t2 = single.Statement,
                        t3 = single.Previous,
                        t4 = single.Current,
                        t5 = single.Record,
                        t6 = single.Status,
                        t7 = single.CategoryName,
                        //t8 = single.year,
                    }).ToList();

                    var strTitle=$"{Resource1.AnnualTextarea1} {Resource1.AnnualTextarea2}" ;

                    if (lang == "ar")
                    {
                        (boolStatus, fileBytes) = ExcelStaticReport.ExcelReportArEn_(excelDataDTO, ListTitles, 0, "ar", strTitle);
                    }
                    else
                    {
                        (boolStatus, fileBytes) = ExcelStaticReport.ExcelReportArEn_(excelDataDTO, ListTitles, 0, "en", strTitle);
                    }
                }

                FileContentResult? Excelfile = null;
                if (fileBytes != null && fileBytes.Length > 0 && boolStatus == true)
                {
                    var fileExcelName = Resource1.InventoryList;
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