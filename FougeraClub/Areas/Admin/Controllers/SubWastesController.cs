using Application.Helpers;
using Application.Interfaces.Admin;
using AutoMapper;
using Domain.DTOs;
using Domain.Entities.Waste;
using KhaledTeamRecycling.Areas.Admin.ViewModels.SubWaste;
using KhaledTeamRecycling.Attributes;
using KhaledTeamRecycling.Helpers;
using KhaledTeamRecycling.Middelware;
using Infrastructure.Repositories.InterfacesDB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Application.Services.Admin;

namespace KhaledTeamRecycling.Areas.Admin.Controllers
{
    [AdminAuthorize]
    [Area("Admin")]
    public class SubWastesController : Controller
    {
        private readonly ISubWasteService _subWasteService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SubWastesController(ISubWasteService subWasteService, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _subWasteService = subWasteService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [YesGet]
        public async Task<IActionResult> Index(string? searchTerm, int? mainWasteId, int page = 1, int pageSize = 50)
        {
            var query = _unitOfWork.SubWastes.Table.Include(x => x.MainWaste).AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(x =>
                    (x.NameAr != null && x.NameAr.Contains(searchTerm)) ||
                    (x.NameEn != null && x.NameEn.Contains(searchTerm)));
            }

            if (mainWasteId.HasValue && mainWasteId.Value > 0)
            {
                query = query.Where(x => x.FKMainWasteId == mainWasteId.Value);
            }

            var totalRecords = await query.CountAsync();
            var items = await query
                .OrderBy(x => x.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var allMainWastes = await _unitOfWork.MainWastes.GetAllAsync();

            var vm = new SubWasteVM
            {
                Items = items,
                SearchString = searchTerm,
                MainWasteFilterId = mainWasteId,
                CurrentPage = page,
                PageSize = pageSize,
                TotalCount = totalRecords,
                TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize),
                HasNextPage = totalRecords > pageSize * page,
                HasPreviousPage = page > 1,
                MainWastesList = SelectListHelper.BindSelectList(allMainWastes.ToList(), mainWasteId).ToList()
            };

            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_ListPartial", vm);
            }

            return View(vm);
        }

        [YesGet]
        public async Task<IActionResult> AddEdit(int? id)
        {
            var vm = new SubWasteVM();
            var allMainWastes = await _unitOfWork.MainWastes.GetAllAsync();
            var allStatuses = await _unitOfWork.Statuses.GetAllAsync();
            vm.MainWastesList = SelectListHelper.BindSelectList(allMainWastes.ToList(), vm.FKMainWasteId).ToList();
            vm.StatusesList = SelectListHelper.BindSelectList(allStatuses.ToList(), vm.StatusId).ToList();

            if (!id.HasValue || id.Value == 0)
            {
                return View(vm);
            }

            var entity = await _subWasteService.GetByIdAsync(id.Value);
            if (entity == null)
            {
                return NotFound();
            }

            vm = _mapper.Map<SubWasteVM>(entity);
            vm.MainWastesList = SelectListHelper.BindSelectList(allMainWastes.ToList(), vm.FKMainWasteId).ToList();
            vm.StatusesList = SelectListHelper.BindSelectList(allStatuses.ToList(), vm.StatusId).ToList();
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEdit(SubWasteVM model)
        {
            if (!ModelState.IsValid)
            {
                var allMainWastes = await _unitOfWork.MainWastes.GetAllAsync();
                var allStatuses = await _unitOfWork.Statuses.GetAllAsync();
                model.MainWastesList = SelectListHelper.BindSelectList(allMainWastes.ToList(), model.FKMainWasteId).ToList();
                model.StatusesList = SelectListHelper.BindSelectList(allStatuses.ToList(), model.StatusId).ToList();
                return View(model);
            }

            var entity = _mapper.Map<SubWaste>(model);

            if (model.Id == 0)
            {
                await _subWasteService.AddAsync(entity);
                return RedirectToAction(nameof(Index));
            }

            await _subWasteService.UpdateAsync(entity);
            return RedirectToAction(nameof(AddEdit), new { id = model.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            var hasRelated = await _subWasteService.HasRelatedObjectsInDb(id);
            if (hasRelated)
            {
                TempData["Error"] = "Cannot delete this item because it has related records.";
                return RedirectToAction(nameof(Index));
            }

            await _subWasteService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> Print(string? searchTerm, int? mainWasteId)
        {
            var items = await _subWasteService.GetAllAsync(searchTerm, mainWasteId);
            var allMainWastes = await _unitOfWork.MainWastes.GetAllAsync();

            var vm = new SubWasteVM
            {
                Items = items,
                SearchString = searchTerm,
                MainWasteFilterId = mainWasteId,
                TotalCount = items.Count(),
                MainWastesList = SelectListHelper.BindSelectList(allMainWastes.ToList(), mainWasteId).ToList()
            };

            return View(vm);
        }

        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> createExcelReport_Download(string? searchTerm, int? mainWasteId)
        {
            var items = await _subWasteService.GetAllAsync(searchTerm, mainWasteId);
            var list = items.ToList();

            if (!list.Any())
            {
                return RedirectToAction(nameof(Index), new { searchTerm, mainWasteId });
            }

            var titles = new List<string> { "Name (AR)", "Name (EN)", "Main Waste" };
            var excelData = list.Select(x => new ExcelDataDTO
            {
                t1 = x.NameAr ?? string.Empty,
                t2 = x.NameEn ?? string.Empty,
                t3 = SessionHelper.GetCurrentLanguage() == "ar" ? x.MainWaste?.NameAr ?? string.Empty : x.MainWaste?.NameEn ?? string.Empty
            }).ToList();

            var (ok, bytes) = ExcelStaticReport.ExcelReportArEn_(excelData, titles, 0, "en");
            if (!ok || bytes == null || bytes.Length == 0)
            {
                return RedirectToAction(nameof(Index), new { searchTerm, mainWasteId });
            }

            return File(
                bytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"SubWastes_{AppDubaiTime.Now:yyyyMMdd_HHmmss}.xlsx");
        }
    }
}
