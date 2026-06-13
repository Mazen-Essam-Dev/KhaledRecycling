using Application.Helpers;
using Application.Interfaces.Admin;
using Application.Services.Admin;
using AutoMapper;
using Domain.DTOs;
using KhaledTeamRecycling.Areas.Admin.ViewModels.RoomInventory;
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
    public class RoomInventoriesController : Controller
    {
        private readonly IRoomInventoryService _roomInventoryService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public RoomInventoriesController(IRoomInventoryService roomInventoryService, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _roomInventoryService = roomInventoryService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [YesGet]
        public async Task<IActionResult> Index(string? searchTerm, int? inventoryId, int? subWasteId, int page = 1, int pageSize = 50)
        {
            var query = _unitOfWork.RoomInventories.Table
                .Include(x => x.Inventory)
                .Include(x => x.SubWaste!)
                    .ThenInclude(x => x.MainWaste)
                .OrderByDescending(x => x.FkInventory)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(x =>
                    (x.GenCode != null && x.GenCode.Contains(searchTerm)) ||
                    (x.Description != null && x.Description.Contains(searchTerm)));
            }

            if (inventoryId.HasValue && inventoryId.Value > 0)
            {
                query = query.Where(x => x.FkInventory == inventoryId.Value);
            }

            if (subWasteId.HasValue && subWasteId.Value > 0)
            {
                query = query.Where(x => x.FKSubWaste == subWasteId.Value);
            }

            var totalRecords = await query.CountAsync();
            var items = await query
                //.OrderBy(x => x.Id)
                .OrderBy(x => x.FkInventory)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var allInventories = await _unitOfWork.Inventories.GetAllAsync();
            var allSubWastes = await _unitOfWork.SubWastes.GetAllAsync();

            var vm = new RoomInventoryVM
            {
                Items = items,
                SearchString = searchTerm,
                InventoryFilterId = inventoryId,
                SubWasteFilterId = subWasteId,
                CurrentPage = page,
                PageSize = pageSize,
                TotalCount = totalRecords,
                TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize),
                HasNextPage = totalRecords > pageSize * page,
                HasPreviousPage = page > 1,
                InventoriesList = SelectListHelper.BindSelectList(allInventories.ToList(), inventoryId, "Id", "Name", "Name").ToList(),
                SubWastesList = SelectListHelper.BindSelectList(allSubWastes.ToList(), subWasteId).ToList()
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
            var vm = new RoomInventoryVM();
            await BindLists(vm);

            if (!id.HasValue || id.Value == 0)
            {
                return View(vm);
            }

            var entity = await _roomInventoryService.GetByIdAsync(id.Value);
            if (entity == null)
            {
                return NotFound();
            }

            vm = _mapper.Map<RoomInventoryVM>(entity);
            await BindLists(vm);
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEdit(RoomInventoryVM model)
        {
            if (!ModelState.IsValid)
            {
                await BindLists(model);
                return View(model);
            }

            if (model.Id == 0)
            {
                var entity = _mapper.Map<Domain.Entities.Inventory.RoomInventory>(model);
                entity.FilledKilo ??= 0;
                entity.ReservedKilo ??= 0;
                await _roomInventoryService.AddAsync(entity);
                return RedirectToAction(nameof(Index));
            }

            var existing = await _unitOfWork.RoomInventories.GetByIdAsync(model.Id);
            if (existing == null)
            {
                return NotFound();
            }

            var minAllowed = (existing.FilledKilo ?? 0) + (existing.ReservedKilo ?? 0);
            if ((model.MaxKilo ?? 0) < minAllowed)
            {
                ModelState.AddModelError(nameof(RoomInventoryVM.MaxKilo), $"لا يمكن أن تكون السعة أقل من {minAllowed:0.##} (الممتلئ + المحجوز)");
                await BindLists(model);
                return View(model);
            }

            existing.GenCode = model.GenCode;
            existing.FkInventory = model.FkInventory;
            existing.FKSubWaste = model.FKSubWaste;
            existing.MaxKilo = model.MaxKilo;
            existing.Description = model.Description;

            _unitOfWork.RoomInventories.Update(existing);
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

            var hasRelated = await _roomInventoryService.HasRelatedObjectsInDb(id);
            if (hasRelated)
            {
                TempData["Error"] = "Cannot delete this item because it has related records.";
                return RedirectToAction(nameof(Index));
            }

            await _roomInventoryService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> Print(string? searchTerm, int? inventoryId, int? subWasteId)
        {
            var items = await _roomInventoryService.GetAllAsync(searchTerm, inventoryId, subWasteId);
            var allInventories = await _unitOfWork.Inventories.GetAllAsync();
            var allSubWastes = await _unitOfWork.SubWastes.GetAllAsync();

            var vm = new RoomInventoryVM
            {
                Items = items,
                SearchString = searchTerm,
                InventoryFilterId = inventoryId,
                SubWasteFilterId = subWasteId,
                TotalCount = items.Count(),
                InventoriesList = SelectListHelper.BindSelectList(allInventories.ToList(), inventoryId, "Id", "Name", "Name").ToList(),
                SubWastesList = SelectListHelper.BindSelectList(allSubWastes.ToList(), subWasteId).ToList()
            };

            return View(vm);
        }

        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> createExcelReport_Download(string? searchTerm, int? inventoryId, int? subWasteId)
        {
            var items = await _roomInventoryService.GetAllAsync(searchTerm, inventoryId, subWasteId);
            var list = items.ToList();

            if (!list.Any())
            {
                return RedirectToAction(nameof(Index), new { searchTerm, inventoryId, subWasteId });
            }

            var titles = new List<string> { "Code", "Inventory", "Sub Waste", "Max Kilo", "Description" };
            var excelData = list.Select(x => new ExcelDataDTO
            {
                t1 = x.GenCode ?? string.Empty,
                t2 = x.Inventory?.Name ?? string.Empty,
                t3 = DisplayHelper.FormatMainSubName(
                    x.SubWaste?.MainWaste?.NameAr,
                    x.SubWaste?.MainWaste?.NameEn,
                    x.SubWaste?.NameAr,
                    x.SubWaste?.NameEn),
                t4 = x.MaxKilo?.ToString() ?? string.Empty,
                t5 = x.Description ?? string.Empty
            }).ToList();

            var (ok, bytes) = ExcelStaticReport.ExcelReportArEn_(excelData, titles, 0, "en");
            if (!ok || bytes == null || bytes.Length == 0)
            {
                return RedirectToAction(nameof(Index), new { searchTerm, inventoryId, subWasteId });
            }

            return File(
                bytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"RoomInventories_{AppDubaiTime.Now:yyyyMMdd_HHmmss}.xlsx");
        }

        private async Task BindLists(RoomInventoryVM vm)
        {
            var allInventories = await _unitOfWork.Inventories.GetAllAsync();
            var allSubWastes = await _unitOfWork.SubWastes.Table
                .Include(x => x.MainWaste)
                .ToListAsync();
            vm.InventoriesList = SelectListHelper.BindSelectList(allInventories.ToList(), vm.FkInventory, "Id", "Name", "Name").ToList();
            vm.SubWastesList = SelectListHelper.BindMainSubSelectList(
                allSubWastes,
                vm.FKSubWaste,
                x => x.MainWaste?.NameAr,
                x => x.MainWaste?.NameEn,
                x => x.NameAr,
                x => x.NameEn,
                x => x.Id).ToList();
        }
    }
}
