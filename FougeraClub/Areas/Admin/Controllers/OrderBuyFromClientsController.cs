using Application.Helpers;
using Application.Interfaces.Admin;
using AutoMapper;
using Domain.DTOs;
using Domain.Entities.Waste;
using KhaledTeamRecycling.Areas.Admin.ViewModels.OrderBuyFromClient;
using KhaledTeamRecycling.Attributes;
using KhaledTeamRecycling.Helpers;
using KhaledTeamRecycling.Middelware;
using Infrastructure.Repositories.InterfacesDB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Application.Services.Admin;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace KhaledTeamRecycling.Areas.Admin.Controllers
{
    [AdminAuthorize]
    [Area("Admin")]
    public class OrderBuyFromClientsController : Controller
    {
        private readonly IOrderBuyFromClientService _orderBuyFromClientService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public OrderBuyFromClientsController(IOrderBuyFromClientService orderBuyFromClientService, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _orderBuyFromClientService = orderBuyFromClientService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [YesGet]
        public async Task<IActionResult> Index(string? searchTerm, int? mainWasteId, int? subWasteId, int page = 1, int pageSize = 50)
        {
            var query = _unitOfWork.OrderBuyFromClients.Table
                .Include(x => x.SubWaste)
                .ThenInclude(x => x.MainWaste)
                .Include(x => x.Status)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(x =>
                    (x.SubWaste != null && x.SubWaste.NameAr != null && x.SubWaste.NameAr.Contains(searchTerm)) ||
                    (x.SubWaste != null && x.SubWaste.NameEn != null && x.SubWaste.NameEn.Contains(searchTerm)));
            }

            if (mainWasteId.HasValue && mainWasteId.Value > 0)
            {
                query = query.Where(x => x.SubWaste != null && x.SubWaste.FKMainWasteId == mainWasteId.Value);
            }

            if (subWasteId.HasValue && subWasteId.Value > 0)
            {
                query = query.Where(x => x.FKSubWasteId == subWasteId.Value);
            }

            var totalRecords = await query.CountAsync();
            var items = await query
                .OrderByDescending(x => x.OrderDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var allMainWastes = await _unitOfWork.MainWastes.GetAllAsync();
            var allSubWastes = await _unitOfWork.SubWastes.GetAllAsync();
            var allStatuses = await _unitOfWork.Statuses.GetAllAsync();

            List<SelectListItem> subWastesList;
            if (mainWasteId.HasValue && mainWasteId.Value > 0)
            {
                subWastesList = SelectListHelper.BindSelectList(allSubWastes.Where(x => x.FKMainWasteId == mainWasteId.Value).ToList(), subWasteId).ToList();
            }
            else
            {
                subWastesList = new List<SelectListItem>();
            }

            var vm = new OrderBuyFromClientVM
            {
                Items = items,
                SearchString = searchTerm,
                MainWasteFilterId = mainWasteId,
                SubWasteFilterId = subWasteId,
                CurrentPage = page,
                PageSize = pageSize,
                TotalCount = totalRecords,
                TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize),
                HasNextPage = totalRecords > pageSize * page,
                HasPreviousPage = page > 1,
                MainWastesList = SelectListHelper.BindSelectList(allMainWastes.ToList(), mainWasteId).ToList(),
                SubWastesList = subWastesList,
                StatusesList = SelectListHelper.BindSelectList(allStatuses.ToList(), null).ToList()
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
            var vm = new OrderBuyFromClientVM();
            var allMainWastes = await _unitOfWork.MainWastes.GetAllAsync();
            var allSubWastes = await _unitOfWork.SubWastes.GetAllAsync();
            var allStatuses = await _unitOfWork.Statuses.GetAllAsync();
            var allUserHasIndividualsOnly = await _unitOfWork.Users.GetAllAsync(x=>x.FKUserType==1);

            vm.MainWastesList = SelectListHelper.BindSelectList(allMainWastes.ToList(), vm.FKMainWasteId).ToList();
            vm.SubWastesList = new List<SelectListItem>();
            vm.StatusesList = SelectListHelper.BindSelectList(allStatuses.ToList(), vm.StatusId).ToList();
            vm.UsersList = SelectListHelper.BindSelectList(allUserHasIndividualsOnly.ToList(), null, "Id", "FullNameAr", "FullNameEn").ToList();

            if (!id.HasValue || id.Value == 0)
            {
                vm.OrderDate = AppDubaiTime.Now;
                return View(vm);
            }

            var entity = await _orderBuyFromClientService.GetByIdAsync(id.Value);
            if (entity == null)
            {
                return NotFound();
            }

            vm = _mapper.Map<OrderBuyFromClientVM>(entity);
            if (entity.SubWaste != null)
            {
                vm.FKMainWasteId = entity.SubWaste.FKMainWasteId;
                vm.BuyPriceUnit = entity.SubWaste.BuyPriceUnit;
                vm.BuyPriceKilo = entity.SubWaste.BuyPriceKilo;
                vm.BuyPriceTon = entity.SubWaste.BuyPriceTon;
            }

            // Determine which checkboxes should be checked based on stored values
            if (entity.CountUnits.HasValue && entity.CountUnits.Value > 0)
            {
                vm.IsUnitsSelected = true;
                vm.UnitsValue = entity.CountUnits.Value;
            }

            if (entity.Kilo.HasValue && entity.Kilo.Value > 0)
            {
                // Check if kilo value is less than 1000, treat as kilos
                if (entity.Kilo.Value < 1000)
                {
                    vm.IsKilosSelected = true;
                    vm.KilosValue = entity.Kilo.Value;
                }
                else
                {
                    // If kilo value is 1000 or more, treat as tons
                    vm.IsTonSelected = true;
                    vm.TonValue = entity.Kilo.Value / 1000;
                }
            }

            vm.MainWastesList = SelectListHelper.BindSelectList(allMainWastes.ToList(), vm.FKMainWasteId).ToList();
            vm.SubWastesList = SelectListHelper.BindSelectList(allSubWastes.Where(x => x.FKMainWasteId == vm.FKMainWasteId).ToList(), vm.FKSubWasteId).ToList();
            vm.StatusesList = SelectListHelper.BindSelectList(allStatuses.ToList(), vm.StatusId).ToList();
            vm.UsersList = SelectListHelper.BindSelectList(allUserHasIndividualsOnly.ToList(), null, "Id", "FullNameAr", "FullNameEn").ToList();

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEdit(OrderBuyFromClientVM model)
        {
            if (!ModelState.IsValid)
            {
                var allMainWastes = await _unitOfWork.MainWastes.GetAllAsync();
                var allSubWastes = await _unitOfWork.SubWastes.GetAllAsync();
                var allStatuses = await _unitOfWork.Statuses.GetAllAsync();
                var allUserHasIndividualsOnly = await _unitOfWork.Users.GetAllAsync(x=>x.FKUserType==1);

                model.MainWastesList = SelectListHelper.BindSelectList(allMainWastes.ToList(), model.FKMainWasteId).ToList();
                model.SubWastesList = SelectListHelper.BindSelectList(allSubWastes.Where(x => x.FKMainWasteId == model.FKMainWasteId).ToList(), model.FKSubWasteId).ToList();
                model.StatusesList = SelectListHelper.BindSelectList(allStatuses.ToList(), model.StatusId).ToList();
                model.UsersList = SelectListHelper.BindSelectList(allUserHasIndividualsOnly.ToList(), null, "Id", "FullNameAr", "FullNameEn").ToList();

                return View(model);
            }

            var entity = _mapper.Map<Domain.Entities.Waste.OrderBuyFromClient>(model);

            // Calculate total based on selected checkboxes
            if (model.IsUnitsSelected && model.UnitsValue.HasValue && model.BuyPriceUnit.HasValue)
            {
                entity.CountUnits = (int?)model.UnitsValue.Value;
            }
            if (model.IsKilosSelected && model.KilosValue.HasValue && model.BuyPriceKilo.HasValue)
            {
                entity.Kilo = model.KilosValue.Value;
            }
            if (model.IsTonSelected && model.TonValue.HasValue && model.BuyPriceTon.HasValue)
            {
                // Convert ton to kilo (1 ton = 1000 kilo)
                entity.Kilo = (entity.Kilo ?? 0) + (model.TonValue.Value * 1000);
            }

            // Calculate total
            double total = 0;
            if (model.IsUnitsSelected && model.UnitsValue.HasValue && model.BuyPriceUnit.HasValue)
            {
                total += model.UnitsValue.Value * model.BuyPriceUnit.Value;
            }
            if (model.IsKilosSelected && model.KilosValue.HasValue && model.BuyPriceKilo.HasValue)
            {
                total += model.KilosValue.Value * model.BuyPriceKilo.Value;
            }
            if (model.IsTonSelected && model.TonValue.HasValue && model.BuyPriceTon.HasValue)
            {
                total += model.TonValue.Value * model.BuyPriceTon.Value;
            }

            // Apply discount
            if (model.DiscountRatio.HasValue && model.DiscountRatio.Value > 0)
            {
                entity.DiscountValue = total * (model.DiscountRatio.Value / 100);
                total -= entity.DiscountValue.Value;
            }
            else if (model.DiscountValue.HasValue && model.DiscountValue.Value > 0)
            {
                total -= model.DiscountValue.Value;
            }

            entity.Total = total;

            if (model.Id == 0)
            {
                await _orderBuyFromClientService.AddAsync(entity);
                return RedirectToAction(nameof(Index));
            }

            await _orderBuyFromClientService.UpdateAsync(entity);
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

            var hasRelated = await _orderBuyFromClientService.HasRelatedObjectsInDb(id);
            if (hasRelated)
            {
                TempData["Error"] = "Cannot delete this item because it has related records.";
                return RedirectToAction(nameof(Index));
            }

            await _orderBuyFromClientService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [IgnoreAction]
        [HttpGet]
        public async Task<IActionResult> GetSubWastesByMainWaste(int mainWasteId)
        {
            var subWastes = await _unitOfWork.SubWastes.Table
                .Where(x => x.FKMainWasteId == mainWasteId)
                .Select(x => new { x.Id, x.NameAr, x.NameEn, x.BuyPriceUnit, x.BuyPriceKilo, x.BuyPriceTon })
                .ToListAsync();

            return Json(subWastes);
        }

        [IgnoreAction]
        [HttpGet]
        public async Task<IActionResult> GetSubWastePrices(int subWasteId)
        {
            var subWaste = await _unitOfWork.SubWastes.GetByIdAsync(subWasteId);
            if (subWaste == null)
            {
                return Json(new { success = false });
            }

            return Json(new
            {
                success = true,
                buyPriceUnit = subWaste.BuyPriceUnit,
                buyPriceKilo = subWaste.BuyPriceKilo,
                buyPriceTon = subWaste.BuyPriceTon
            });
        }

        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> Print(string? searchTerm, int? mainWasteId, int? subWasteId)
        {
            var items = await _orderBuyFromClientService.GetAllAsync(searchTerm, mainWasteId, subWasteId);
            var allMainWastes = await _unitOfWork.MainWastes.GetAllAsync();
            var allSubWastes = await _unitOfWork.SubWastes.GetAllAsync();
            var allStatuses = await _unitOfWork.Statuses.GetAllAsync();

            var vm = new OrderBuyFromClientVM
            {
                Items = items,
                SearchString = searchTerm,
                MainWasteFilterId = mainWasteId,
                SubWasteFilterId = subWasteId,
                TotalCount = items.Count(),
                MainWastesList = SelectListHelper.BindSelectList(allMainWastes.ToList(), mainWasteId).ToList(),
                SubWastesList = SelectListHelper.BindSelectList(allSubWastes.ToList(), subWasteId).ToList(),
                StatusesList = SelectListHelper.BindSelectList(allStatuses.ToList(), null).ToList()
            };

            return View(vm);
        }

        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> createExcelReport_Download(string? searchTerm, int? mainWasteId, int? subWasteId)
        {
            var items = await _orderBuyFromClientService.GetAllAsync(searchTerm, mainWasteId, subWasteId);
            var list = items.ToList();

            if (!list.Any())
            {
                return RedirectToAction(nameof(Index), new { searchTerm, mainWasteId, subWasteId });
            }

            var titles = new List<string> { "Sub Waste", "Main Waste", "Order Date", "Units", "Kilo", "Discount", "Total", "Status" };
            var excelData = list.Select(x => new ExcelDataDTO
            {
                t1 = SessionHelper.GetCurrentLanguage() == "ar" ? x.SubWaste?.NameAr ?? string.Empty : x.SubWaste?.NameEn ?? string.Empty,
                t2 = SessionHelper.GetCurrentLanguage() == "ar" ? x.SubWaste?.MainWaste?.NameAr ?? string.Empty : x.SubWaste?.MainWaste?.NameEn ?? string.Empty,
                t3 = x.OrderDate?.ToString("yyyy-MM-dd") ?? string.Empty,
                t4 = x.CountUnits?.ToString() ?? string.Empty,
                t5 = x.Kilo?.ToString() ?? string.Empty,
                t6 = x.DiscountValue?.ToString() ?? string.Empty,
                t7 = x.Total?.ToString() ?? string.Empty,
                t8 = SessionHelper.GetCurrentLanguage() == "ar" ? x.Status?.NameAr ?? string.Empty : x.Status?.NameEn ?? string.Empty
            }).ToList();

            var (ok, bytes) = ExcelStaticReport.ExcelReportArEn_(excelData, titles, 0, "en");
            if (!ok || bytes == null || bytes.Length == 0)
            {
                return RedirectToAction(nameof(Index), new { searchTerm, mainWasteId, subWasteId });
            }

            return File(
                bytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"OrderBuyFromClients_{AppDubaiTime.Now:yyyyMMdd_HHmmss}.xlsx");
        }
    }
}
