using Application.Helpers;
using Application.Interfaces.Admin;
using Application.Services.Admin;
using AutoMapper;
using Domain.DTOs;
using KhaledTeamRecycling.Areas.Admin.ViewModels.Gallery;
using KhaledTeamRecycling.Attributes;
using KhaledTeamRecycling.Middelware;
using Infrastructure.Repositories.InterfacesDB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KhaledTeamRecycling.Areas.Admin.Controllers
{
    [AdminAuthorize]
    [Area("Admin")]
    public class GalleriesController : Controller
    {
        private readonly IGalleryService _galleryService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GalleriesController(IGalleryService galleryService, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _galleryService = galleryService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [YesGet]
        public async Task<IActionResult> Index(string? searchTerm, int page = 1, int pageSize = 50)
        {
            var query = _unitOfWork.Galleries.Table.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(x =>
                    (x.Name != null && x.Name.Contains(searchTerm)) ||
                    (x.Location != null && x.Location.Contains(searchTerm)) ||
                    (x.Description != null && x.Description.Contains(searchTerm)));
            }

            var totalRecords = await query.CountAsync();
            var items = await query
                .OrderBy(x => x.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var vm = new GalleryVM
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
                return View(new GalleryVM());
            }

            var entity = await _galleryService.GetByIdAsync(id.Value);
            if (entity == null)
            {
                return NotFound();
            }

            var vm = _mapper.Map<GalleryVM>(entity);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEdit(GalleryVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var entity = _mapper.Map<Domain.Entities.Gallery.Gallery>(model);

            if (model.Id == 0)
            {
                await _galleryService.AddAsync(entity);
                return RedirectToAction(nameof(Index));
            }

            await _galleryService.UpdateAsync(entity);
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

            var hasRelated = await _galleryService.HasRelatedObjectsInDb(id);
            if (hasRelated)
            {
                TempData["Error"] = "Cannot delete this item because it has related room gallery records.";
                return RedirectToAction(nameof(Index));
            }

            await _galleryService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> Print(string? searchTerm)
        {
            var items = await _galleryService.GetAllAsync(searchTerm);
            var vm = new GalleryVM
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
            var items = await _galleryService.GetAllAsync(searchTerm);
            var list = items.ToList();

            if (!list.Any())
            {
                return RedirectToAction(nameof(Index), new { searchTerm });
            }

            var titles = new List<string> { "Name", "No Rooms", "Location", "Description" };
            var excelData = list.Select(x => new ExcelDataDTO
            {
                t1 = x.Name ?? string.Empty,
                t2 = x.NoRooms?.ToString() ?? string.Empty,
                t3 = x.Location ?? string.Empty,
                t4 = x.Description ?? string.Empty
            }).ToList();

            var (ok, bytes) = ExcelStaticReport.ExcelReportArEn_(excelData, titles, 0, "en");
            if (!ok || bytes == null || bytes.Length == 0)
            {
                return RedirectToAction(nameof(Index), new { searchTerm });
            }

            return File(
                bytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"Galleries_{AppDubaiTime.Now:yyyyMMdd_HHmmss}.xlsx");
        }
    }
}
