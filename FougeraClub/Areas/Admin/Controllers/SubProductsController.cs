using Application.Helpers;
using Application.Interfaces.Admin;
using AutoMapper;
using Domain.DTOs;
using Domain.Entities.Product;
using KhaledTeamRecycling.Areas.Admin.ViewModels.SubProduct;
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
    public class SubProductsController : Controller
    {
        private readonly ISubProductService _subProductService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public SubProductsController(ISubProductService subProductService, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _subProductService = subProductService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [YesGet]
        public async Task<IActionResult> Index(string? searchTerm, int? mainProductId, int page = 1, int pageSize = 50)
        {
            var query = _unitOfWork.SubProducts.Table.Include(x => x.MainProduct).AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(x =>
                    (x.NameAr != null && x.NameAr.Contains(searchTerm)) ||
                    (x.NameEn != null && x.NameEn.Contains(searchTerm)));
            }

            if (mainProductId.HasValue && mainProductId.Value > 0)
            {
                query = query.Where(x => x.FKMainProductId == mainProductId.Value);
            }

            var totalRecords = await query.CountAsync();
            var items = await query
                .OrderBy(x => x.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var allMainProducts = await _unitOfWork.MainProducts.GetAllAsync();

            var vm = new SubProductVM
            {
                Items = items,
                SearchString = searchTerm,
                MainProductFilterId = mainProductId,
                CurrentPage = page,
                PageSize = pageSize,
                TotalCount = totalRecords,
                TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize),
                HasNextPage = totalRecords > pageSize * page,
                HasPreviousPage = page > 1,
                MainProductsList = SelectListHelper.BindSelectList(allMainProducts.ToList(), mainProductId).ToList()
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
            var vm = new SubProductVM();
            var allMainProducts = await _unitOfWork.MainProducts.GetAllAsync();
            var allStatuses = await _unitOfWork.Statuses.GetAllAsync();
            vm.MainProductsList = SelectListHelper.BindSelectList(allMainProducts.ToList(), vm.FKMainProductId).ToList();
            vm.StatusesList = SelectListHelper.BindSelectList(allStatuses.ToList(), vm.StatusId).ToList();

            if (!id.HasValue || id.Value == 0)
            {
                return View(vm);
            }

            var entity = await _subProductService.GetByIdAsync(id.Value);
            if (entity == null)
            {
                return NotFound();
            }

            vm = _mapper.Map<SubProductVM>(entity);
            vm.MainProductsList = SelectListHelper.BindSelectList(allMainProducts.ToList(), vm.FKMainProductId).ToList();
            vm.StatusesList = SelectListHelper.BindSelectList(allStatuses.ToList(), vm.StatusId).ToList();
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEdit(SubProductVM model)
        {
            if (!ModelState.IsValid)
            {
                var allMainProducts = await _unitOfWork.MainProducts.GetAllAsync();
                var allStatuses = await _unitOfWork.Statuses.GetAllAsync();
                model.MainProductsList = SelectListHelper.BindSelectList(allMainProducts.ToList(), model.FKMainProductId).ToList();
                model.StatusesList = SelectListHelper.BindSelectList(allStatuses.ToList(), model.StatusId).ToList();
                return View(model);
            }

            var entity = _mapper.Map<SubProduct>(model);

            if (model.Id == 0)
            {
                await _subProductService.AddAsync(entity);
                return RedirectToAction(nameof(Index));
            }

            await _subProductService.UpdateAsync(entity);
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

            var hasRelated = await _subProductService.HasRelatedObjectsInDb(id);
            if (hasRelated)
            {
                TempData["Error"] = "Cannot delete this item because it has related records.";
                return RedirectToAction(nameof(Index));
            }

            await _subProductService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            var entity = await _subProductService.GetByIdAsync(id);
            if (entity == null)
            {
                return NotFound();
            }

            entity.StatusChar = entity.StatusChar == 'H' ? null : 'H';
            await _subProductService.UpdateAsync(entity);

            return RedirectToAction(nameof(Index));
        }

        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> Print(string? searchTerm, int? mainProductId)
        {
            var query = _unitOfWork.SubProducts.Table.Include(x => x.MainProduct).AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(x =>
                    (x.NameAr != null && x.NameAr.Contains(searchTerm)) ||
                    (x.NameEn != null && x.NameEn.Contains(searchTerm)));
            }

            if (mainProductId.HasValue && mainProductId.Value > 0)
            {
                query = query.Where(x => x.FKMainProductId == mainProductId.Value);
            }

            var items = await query.OrderBy(x => x.Id).ToListAsync();
            var allMainProducts = await _unitOfWork.MainProducts.GetAllAsync();

            var vm = new SubProductVM
            {
                Items = items,
                SearchString = searchTerm,
                MainProductFilterId = mainProductId,
                TotalCount = items.Count(),
                MainProductsList = SelectListHelper.BindSelectList(allMainProducts.ToList(), mainProductId).ToList()
            };

            return View(vm);
        }

        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> createExcelReport_Download(string? searchTerm, int? mainProductId)
        {
            var query = _unitOfWork.SubProducts.Table.Include(x => x.MainProduct).AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(x =>
                    (x.NameAr != null && x.NameAr.Contains(searchTerm)) ||
                    (x.NameEn != null && x.NameEn.Contains(searchTerm)));
            }

            if (mainProductId.HasValue && mainProductId.Value > 0)
            {
                query = query.Where(x => x.FKMainProductId == mainProductId.Value);
            }

            var list = await query.OrderBy(x => x.Id).ToListAsync();

            if (!list.Any())
            {
                return RedirectToAction(nameof(Index), new { searchTerm, mainProductId });
            }

            var titles = new List<string> { "Name (AR)", "Name (EN)", "Main Product" };
            var excelData = list.Select(x => new ExcelDataDTO
            {
                t1 = x.NameAr ?? string.Empty,
                t2 = x.NameEn ?? string.Empty,
                t3 = SessionHelper.GetCurrentLanguage() == "ar" ? x.MainProduct?.NameAr ?? string.Empty : x.MainProduct?.NameEn ?? string.Empty
            }).ToList();

            var (ok, bytes) = ExcelStaticReport.ExcelReportArEn_(excelData, titles, 0, "en");
            if (!ok || bytes == null || bytes.Length == 0)
            {
                return RedirectToAction(nameof(Index), new { searchTerm, mainProductId });
            }

            return File(
                bytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"SubProducts_{AppDubaiTime.Now:yyyyMMdd_HHmmss}.xlsx");
        }
    }
}
