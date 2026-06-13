using Application.Interfaces.Admin;
using Application.Helpers;
using AutoMapper;
using Domain.DTOs;
using Domain.Entities.Product;
using KhaledTeamRecycling.Areas.Admin.ViewModels.MainProduct;
using KhaledTeamRecycling.Attributes;
using KhaledTeamRecycling.Middelware;
using Infrastructure.Repositories.InterfacesDB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Application.Services.Admin;

namespace KhaledTeamRecycling.Areas.Admin.Controllers
{
    [AdminAuthorize]
    [Area("Admin")]
    public class MainProductsController : Controller
    {
        private readonly IMainProductService _mainProductService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public MainProductsController(IMainProductService mainProductService, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _mainProductService = mainProductService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [YesGet]
        public async Task<IActionResult> Index(string? searchTerm, int page = 1, int pageSize = 50)
        {
            var query = _unitOfWork.MainProducts.Table.AsQueryable();
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(x =>
                    (x.NameAr != null && x.NameAr.Contains(searchTerm)) ||
                    (x.NameEn != null && x.NameEn.Contains(searchTerm)));
            }

            var totalRecords = await query.CountAsync();
            var items = await query
                .OrderBy(x => x.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var vm = new MainProductVM
            {
                Items = items,
                SearchString = searchTerm,
                CurrentPage = page,
                PageSize = pageSize,
                TotalCount = totalRecords,
                TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize),
                HasNextPage = totalRecords > pageSize * page,
                HasPreviousPage = page > 1
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
            if (!id.HasValue || id.Value == 0)
            {
                return View(new MainProductVM());
            }

            var entity = await _mainProductService.GetByIdAsync(id.Value);
            if (entity == null)
            {
                return NotFound();
            }

            var vm = _mapper.Map<MainProductVM>(entity);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEdit(MainProductVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var entity = _mapper.Map<MainProduct>(model);

            if (model.Id == 0)
            {
                await _mainProductService.AddAsync(entity);
                return RedirectToAction(nameof(Index));
            }

            await _mainProductService.UpdateAsync(entity);
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

            var hasRelated = await _mainProductService.HasRelatedObjectsInDb(id);
            if (hasRelated)
            {
                TempData["Error"] = "Cannot delete this item because it has related SubProduct records.";
                return RedirectToAction(nameof(Index));
            }

            await _mainProductService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> Print(string? searchTerm)
        {
            var query = _unitOfWork.MainProducts.Table.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(x =>
                    (x.NameAr != null && x.NameAr.Contains(searchTerm)) ||
                    (x.NameEn != null && x.NameEn.Contains(searchTerm)));
            }

            var items = await query.OrderBy(x => x.Id).ToListAsync();
            var vm = new MainProductVM
            {
                Items = items,
                SearchString = searchTerm,
                TotalCount = items.Count()
            };

            return View(vm);
        }

        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> createExcelReport_Download(string? searchTerm)
        {
            var query = _unitOfWork.MainProducts.Table.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(x =>
                    (x.NameAr != null && x.NameAr.Contains(searchTerm)) ||
                    (x.NameEn != null && x.NameEn.Contains(searchTerm)));
            }

            var list = await query.OrderBy(x => x.Id).ToListAsync();

            if (!list.Any())
            {
                return RedirectToAction(nameof(Index), new { searchTerm });
            }

            var titles = new List<string> { "Name (AR)", "Name (EN)" };
            var excelData = list.Select(x => new ExcelDataDTO
            {
                t1 = x.NameAr ?? string.Empty,
                t2 = x.NameEn ?? string.Empty
            }).ToList();

            var (ok, bytes) = ExcelStaticReport.ExcelReportArEn_(excelData, titles, 0, "en");
            if (!ok || bytes == null || bytes.Length == 0)
            {
                return RedirectToAction(nameof(Index), new { searchTerm });
            }

            return File(
                bytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"MainProducts_{AppDubaiTime.Now:yyyyMMdd_HHmmss}.xlsx");
        }
    }
}
