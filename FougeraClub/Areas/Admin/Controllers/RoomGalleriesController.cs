using Application.Helpers;
using Application.Interfaces.Admin;
using Application.Services.Admin;
using AutoMapper;
using Domain.DTOs;
using KhaledTeamRecycling.Areas.Admin.ViewModels.RoomGallery;
using KhaledTeamRecycling.Attributes;
using KhaledTeamRecycling.Helpers;
using KhaledTeamRecycling.Middelware;
using Infrastructure.Repositories.InterfacesDB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KhaledTeamRecycling.Areas.Admin.Controllers
{
    [AdminAuthorize]
    [Area("Admin")]
    public class RoomGalleriesController : Controller
    {
        private readonly IRoomGalleryService _roomGalleryService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public RoomGalleriesController(IRoomGalleryService roomGalleryService, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _roomGalleryService = roomGalleryService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [YesGet]
        public async Task<IActionResult> Index(string? searchTerm, int? galleryId, int? subProductId, int page = 1, int pageSize = 50)
        {
            var query = _unitOfWork.RoomGalleries.Table
                .Include(x => x.Gallery)
                .Include(x => x.SubProduct!)
                    .ThenInclude(x => x.MainProduct)
                .OrderByDescending(x => x.FkGallery)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(x =>
                    (x.GenCode != null && x.GenCode.Contains(searchTerm)) ||
                    (x.Description != null && x.Description.Contains(searchTerm)));
            }

            if (galleryId.HasValue && galleryId.Value > 0)
            {
                query = query.Where(x => x.FkGallery == galleryId.Value);
            }

            if (subProductId.HasValue && subProductId.Value > 0)
            {
                query = query.Where(x => x.FkSubProduct == subProductId.Value);
            }

            var totalRecords = await query.CountAsync();
            var items = await query
                //.OrderBy(x => x.Id)
                .OrderBy(x => x.FkGallery)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var allGalleries = await _unitOfWork.Galleries.GetAllAsync();
            var allSubProducts = await _unitOfWork.SubProducts.GetAllAsync();

            var vm = new RoomGalleryVM
            {
                Items = items,
                SearchString = searchTerm,
                GalleryFilterId = galleryId,
                SubProductFilterId = subProductId,
                CurrentPage = page,
                PageSize = pageSize,
                TotalCount = totalRecords,
                TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize),
                HasNextPage = totalRecords > pageSize * page,
                HasPreviousPage = page > 1,
                GalleriesList = SelectListHelper.BindSelectList(allGalleries.ToList(), galleryId, "Id", "Name", "Name").ToList(),
                SubProductsList = SelectListHelper.BindSelectList(allSubProducts.ToList(), subProductId).ToList()
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
            var vm = new RoomGalleryVM();
            await BindLists(vm);

            if (!id.HasValue || id.Value == 0)
            {
                return View(vm);
            }

            var entity = await _roomGalleryService.GetByIdAsync(id.Value);
            if (entity == null)
            {
                return NotFound();
            }

            vm = _mapper.Map<RoomGalleryVM>(entity);
            await BindLists(vm);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEdit(RoomGalleryVM model)
        {
            if (!ModelState.IsValid)
            {
                await BindLists(model);
                return View(model);
            }

            if (model.Id == 0)
            {
                var entity = _mapper.Map<Domain.Entities.Gallery.RoomGallery>(model);
                entity.FilledUnits ??= 0;
                entity.ReservedUnits ??= 0;
                await _roomGalleryService.AddAsync(entity);
                return RedirectToAction(nameof(Index));
            }

            var existing = await _unitOfWork.RoomGalleries.GetByIdAsync(model.Id);
            if (existing == null)
            {
                return NotFound();
            }

            var minAllowed = (existing.FilledUnits ?? 0) + (existing.ReservedUnits ?? 0);
            if ((model.MaxUnits ?? 0) < minAllowed)
            {
                ModelState.AddModelError(nameof(RoomGalleryVM.MaxUnits), $"لا يمكن أن تكون السعة أقل من {minAllowed} (الممتلئ + المحجوز)");
                await BindLists(model);
                return View(model);
            }

            existing.GenCode = model.GenCode;
            existing.FkGallery = model.FkGallery;
            existing.FkSubProduct = model.FkSubProduct;
            existing.MaxUnit = model.MaxUnits;
            existing.Description = model.Description;

            _unitOfWork.RoomGalleries.Update(existing);
            await _unitOfWork.CompleteAsync();

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

            var hasRelated = await _roomGalleryService.HasRelatedObjectsInDb(id);
            if (hasRelated)
            {
                TempData["Error"] = "Cannot delete this item because it has related records.";
                return RedirectToAction(nameof(Index));
            }

            await _roomGalleryService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> Print(string? searchTerm, int? galleryId, int? subProductId)
        {
            var query = _unitOfWork.RoomGalleries.Table
                .Include(x => x.Gallery)
                .Include(x => x.SubProduct!)
                    .ThenInclude(x => x.MainProduct)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(x =>
                    (x.GenCode != null && x.GenCode.Contains(searchTerm)) ||
                    (x.Description != null && x.Description.Contains(searchTerm)));
            }

            if (galleryId.HasValue && galleryId.Value > 0)
            {
                query = query.Where(x => x.FkGallery == galleryId.Value);
            }

            if (subProductId.HasValue && subProductId.Value > 0)
            {
                query = query.Where(x => x.FkSubProduct == subProductId.Value);
            }

            var items = await query.OrderBy(x => x.FkGallery).ToListAsync();
            var allGalleries = await _unitOfWork.Galleries.GetAllAsync();
            var allSubProducts = await _unitOfWork.SubProducts.GetAllAsync();

            var vm = new RoomGalleryVM
            {
                Items = items,
                SearchString = searchTerm,
                GalleryFilterId = galleryId,
                SubProductFilterId = subProductId,
                TotalCount = items.Count(),
                GalleriesList = SelectListHelper.BindSelectList(allGalleries.ToList(), galleryId, "Id", "Name", "Name").ToList(),
                SubProductsList = SelectListHelper.BindSelectList(allSubProducts.ToList(), subProductId).ToList()
            };

            return View(vm);
        }

        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> createExcelReport_Download(string? searchTerm, int? galleryId, int? subProductId)
        {
            var query = _unitOfWork.RoomGalleries.Table
                .Include(x => x.Gallery)
                .Include(x => x.SubProduct!)
                    .ThenInclude(x => x.MainProduct)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(x =>
                    (x.GenCode != null && x.GenCode.Contains(searchTerm)) ||
                    (x.Description != null && x.Description.Contains(searchTerm)));
            }

            if (galleryId.HasValue && galleryId.Value > 0)
            {
                query = query.Where(x => x.FkGallery == galleryId.Value);
            }

            if (subProductId.HasValue && subProductId.Value > 0)
            {
                query = query.Where(x => x.FkSubProduct == subProductId.Value);
            }

            var list = await query.OrderBy(x => x.FkGallery).ToListAsync();

            if (!list.Any())
            {
                return RedirectToAction(nameof(Index), new { searchTerm, galleryId, subProductId });
            }

            var titles = new List<string>
            {
                "Id",
                "Code",
                "Gallery",
                "Sub Product",
                "Max Units",
                "Filled Units",
                "Reserved Units",
                "Free Units",
                "Free (%)",
                "Description"
            };

            var excelData = list.Select(x =>
            {
                int maxUnit = x.MaxUnit ?? 0;
                int filled = x.FilledUnits ?? 0;
                int reserved = x.ReservedUnits ?? 0;

                int free = maxUnit - filled - reserved;
                double freePercentage = maxUnit > 0
                ? ((double)free / maxUnit) * 100
                : 100;

                return new ExcelDataDTO
                {
                    t1 = x.Id.ToString(),
                    t2 = x.GenCode ?? string.Empty,
                    t3 = x.Gallery?.Name ?? string.Empty,
                    t4 = DisplayHelper.FormatMainSubName(
                            x.SubProduct?.MainProduct?.NameAr,
                            x.SubProduct?.MainProduct?.NameEn,
                            x.SubProduct?.NameAr,
                            x.SubProduct?.NameEn),
                    t5 = maxUnit,
                    t6 = filled,
                    t7 = reserved,
                    t8 = free,
                    t9 = freePercentage.ToString("F2") + "%",
                    t10 = x.Description ?? string.Empty
                };
            }).ToList();

            var (ok, bytes) = ExcelStaticReport.ExcelReportArEn_(excelData, titles, 0, "en");
            if (!ok || bytes == null || bytes.Length == 0)
            {
                return RedirectToAction(nameof(Index), new { searchTerm, galleryId, subProductId });
            }

            return File(
                bytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"RoomGalleries_{AppDubaiTime.Now:yyyyMMdd_HHmmss}.xlsx");
        }

        private async Task BindLists(RoomGalleryVM vm)
        {
            var allGalleries = await _unitOfWork.Galleries.GetAllAsync();
            var allSubProducts = await _unitOfWork.SubProducts.Table
                .Include(x => x.MainProduct)
                .ToListAsync();
            vm.GalleriesList = SelectListHelper.BindSelectList(allGalleries.ToList(), vm.FkGallery, "Id", "Name", "Name").ToList();
            vm.SubProductsList = SelectListHelper.BindMainSubSelectList(
                allSubProducts,
                vm.FkSubProduct,
                x => x.MainProduct?.NameAr,
                x => x.MainProduct?.NameEn,
                x => x.NameAr,
                x => x.NameEn,
                x => x.Id).ToList();
        }
    }
}
