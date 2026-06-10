using Application.Helpers;
using Application.Interfaces.Admin;
using Application.Services.Admin;
using AutoMapper;
using Domain.DTOs;
using Domain.Entities.Waste;
using Infrastructure.Repositories.InterfacesDB;
using KhaledTeamRecycling.Areas.Admin.ViewModels.OrderSellToFactory;
using KhaledTeamRecycling.Attributes;
using KhaledTeamRecycling.Helpers;
using KhaledTeamRecycling.Middelware;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Principal;

namespace KhaledTeamRecycling.Areas.Admin.Controllers
{
    [AdminAuthorize]
    [Area("Admin")]
    public class OrderSellToFactorysController : Controller
    {
        private readonly IOrderSellToFactoryService _orderSellToFactoryService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly PermissionScanner _PermissionScanner;

        public OrderSellToFactorysController(IOrderSellToFactoryService orderSellToFactoryService,PermissionScanner PermissionScanner, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _orderSellToFactoryService = orderSellToFactoryService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _PermissionScanner = PermissionScanner;
        }

        [YesGet]
        public async Task<IActionResult> Index(string? searchTerm, int? mainWasteId, int? subWasteId, int page = 1, int pageSize = 50)
        {
            var query = _unitOfWork.OrderSellToFactorys.Table
                .Include(x => x.SubWaste!)
                .ThenInclude(x => x.MainWaste!)
                .Include(x => x.Status!)
                .AsQueryable();

            var loggedInUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var loggedInUser = !string.IsNullOrEmpty(loggedInUserId) ? await _unitOfWork.Users.GetByIdAsync(loggedInUserId) : null;
            var isFactoryUser = loggedInUser != null &&( loggedInUser.FKUserType == 3); // مصنع فقط

            if (isFactoryUser)
            {
                query = query.Where(x => x.FKUserId == loggedInUserId);
            }

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
            var allUserHasFactorysOnly = await _unitOfWork.Users.GetAllAsync(x => x.FKUserType == 3);

            var vm = new OrderSellToFactoryVM
            {
                IsFactoryUser = isFactoryUser,
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
                StatusesList = SelectListHelper.BindSelectList(allStatuses.ToList(), null).ToList(),
                UsersList = SelectListHelper.BindSelectList(allUserHasFactorysOnly.ToList(), null, "Id", "FullNameAr", "FullNameEn").ToList()
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

            var vm = new OrderSellToFactoryVM();
            var allMainWastes = await _unitOfWork.MainWastes.GetAllAsync();
            var allSubWastes = await _unitOfWork.SubWastes.GetAllAsync();
            var allStatuses = await _unitOfWork.Statuses.GetAllAsync();
            var allUserHasFactorysOnly = await _unitOfWork.Users.GetAllAsync(x=>x.FKUserType == 3);

            var loggedInUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var loggedInUser = !string.IsNullOrEmpty(loggedInUserId) ? await _unitOfWork.Users.GetByIdAsync(loggedInUserId) : null;
            var isFactoryUser = loggedInUser != null &&( loggedInUser.FKUserType == 3); // مصنع فقط

            vm.IsFactoryUser = isFactoryUser;
            if (isFactoryUser)
            {
                vm.FKUserId = loggedInUserId;
                if (!id.HasValue || id.Value == 0)
                {
                    vm.Address = loggedInUser?.Address;
                }
            }

            var DoneStatus = await _unitOfWork.Statuses.GetByIdAsync(x => x.ShortChar == "D");
            if(vm.StatusId == DoneStatus?.Id && vm.StatusId >0) vm.isDisabled=true;

            vm.MainWastesList = SelectListHelper.BindSelectList(allMainWastes.ToList(), vm.FKMainWasteId).ToList();
            vm.SubWastesList = new List<SelectListItem>();
            vm.StatusesList = SelectListHelper.BindSelectList(allStatuses.ToList(), vm.StatusId).ToList();
            vm.UsersList = SelectListHelper.BindSelectList(allUserHasFactorysOnly.ToList(), null, "Id", "FullNameAr", "FullNameEn").ToList();

            if (!id.HasValue || id.Value == 0)
            {
                vm.OrderDate = AppDubaiTime.Now;

                if (isFactoryUser && !string.IsNullOrEmpty(loggedInUserId))
                {
                    var userPointsTable = _unitOfWork.Context.Set<Domain.Entities.UserPoints>();
                    var userPoints = await userPointsTable.FirstOrDefaultAsync(x => x.FKUserId == loggedInUserId);
                    if (userPoints != null && userPoints.Points.HasValue && userPoints.Points.Value > 0)
                    {
                        vm.MaxDiscountRatioAllowed = Math.Min(30, (userPoints.Points.Value / 50));
                    }
                }

                return View(vm);
            }

            var entity = await _orderSellToFactoryService.GetByIdAsync(id.Value);
            if (entity == null)
            {
                return NotFound();
            }
            var currentStatusObj = await _unitOfWork.Statuses.GetByIdAsync(entity.StatusId);
            if (currentStatusObj != null && (currentStatusObj.ShortChar == "C" || currentStatusObj.ShortChar == "S"))
            {
                TempData["Error"] = "Cannot edit this order because it is in a restricted status.";
                return RedirectToAction(nameof(Index));
            }

            vm = _mapper.Map<OrderSellToFactoryVM>(entity);
            vm.IsFactoryUser = isFactoryUser;
            if (isFactoryUser)
            {
                vm.FKUserId = loggedInUserId;
            }
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
            vm.UsersList = SelectListHelper.BindSelectList(allUserHasFactorysOnly.ToList(), null, "Id", "FullNameAr", "FullNameEn").ToList();

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEdit(OrderSellToFactoryVM model)
        {
            var loggedInUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var loggedInUser = !string.IsNullOrEmpty(loggedInUserId) ? await _unitOfWork.Users.GetByIdAsync(loggedInUserId) : null;
            var isFactoryUser = loggedInUser != null && loggedInUser.FKUserType == 1;

            var FkUserId = "";

            if (isFactoryUser)
            {
                model.FKUserId = loggedInUserId;
                model.IsFactoryUser = true;
            }

            if (!ModelState.IsValid)
            {
                var allMainWastes = await _unitOfWork.MainWastes.GetAllAsync();
                var allSubWastes = await _unitOfWork.SubWastes.GetAllAsync();
                var allStatuses = await _unitOfWork.Statuses.GetAllAsync();
                var allUserHasFactorysOnly = await _unitOfWork.Users.GetAllAsync(x => x.FKUserType == 3);

                model.MainWastesList = SelectListHelper.BindSelectList(allMainWastes.ToList(), model.FKMainWasteId).ToList();
                model.SubWastesList = SelectListHelper.BindSelectList(allSubWastes.Where(x => x.FKMainWasteId == model.FKMainWasteId).ToList(), model.FKSubWasteId).ToList();
                model.StatusesList = SelectListHelper.BindSelectList(allStatuses.ToList(), model.StatusId).ToList();
                model.UsersList = SelectListHelper.BindSelectList(allUserHasFactorysOnly.ToList(), null, "Id", "FullNameAr", "FullNameEn").ToList();

                return View(model);
            }

            var entity = _mapper.Map<Domain.Entities.Waste.OrderSellToFactory>(model);

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

            // Points discount logic: only for new orders by Client
            bool pointsDiscountApplied = false;
            if (model.Id == 0 && isFactoryUser && model.UsePointsDiscount && !string.IsNullOrEmpty(loggedInUserId))
            {
                var userPointsTable = _unitOfWork.Context.Set<Domain.Entities.UserPoints>();
                var userPoints = await userPointsTable.FirstOrDefaultAsync(x => x.FKUserId == loggedInUserId);
                if (userPoints != null && userPoints.Points.HasValue && userPoints.Points.Value > 0)
                {
                    int maxDiscount = Math.Min(30, userPoints.Points.Value / 50);
                    if (maxDiscount > 0)
                    {
                        entity.DiscountRatio = maxDiscount;
                        pointsDiscountApplied = true;

                        // Reset points and TotalsReNew
                        userPoints.Points = 0;
                        userPoints.TotalsReNew = 0;
                        userPointsTable.Update(userPoints);
                    }
                }
            }

            if (!pointsDiscountApplied)
            {
                // Clients cannot set DiscountRatio manually
                entity.DiscountRatio = isFactoryUser ? null : model.DiscountRatio;
            }

            // Apply discount
            if (entity.DiscountRatio.HasValue && entity.DiscountRatio.Value > 0)
            {
                entity.DiscountValue = total * (entity.DiscountRatio.Value / 100);
                total -= entity.DiscountValue.Value;
            }
            else if (!isFactoryUser && model.DiscountValue.HasValue && model.DiscountValue.Value > 0)
            {
                total -= model.DiscountValue.Value;
            }

            entity.Total = total;

            if (model.Id == 0)
            {
                await _orderSellToFactoryService.AddAsync(entity);
                if (pointsDiscountApplied)
                    await _unitOfWork.CompleteAsync();
                return RedirectToAction(nameof(AddEdit), new { id = entity.Id });
            }

            FkUserId = entity.FKUserId;
            await _orderSellToFactoryService.UpdateAsync(entity);


            var financial = await _unitOfWork.Financials.Table.FirstOrDefaultAsync(x => x.TableType == "OrderSellToFactory" && x.ItsId == model.Id);
            if (financial != null)
            {
                financial.Total = entity.Total;
                var FKUserType = await _PermissionScanner.GetFKUserType(FkUserId);
                financial.FKUserType = FKUserType;
                financial.FKUserId = FkUserId;
                _unitOfWork.Financials.Update(financial);
                await _unitOfWork.CompleteAsync();
            }

            return RedirectToAction(nameof(AddEdit), new { id = model.Id });
        }


        [YesGet]
        public async Task<IActionResult> UpdateStatus(int? id)
        {

            var vm = new OrderSellToFactoryVM();
            var allMainWastes = await _unitOfWork.MainWastes.GetAllAsync();
            var allSubWastes = await _unitOfWork.SubWastes.GetAllAsync();
            var allStatuses = await _unitOfWork.Statuses.GetAllAsync();
            var allUserHasFactorysOnly = await _unitOfWork.Users.GetAllAsync(x => x.FKUserType == 3);

            var loggedInUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var loggedInUser = !string.IsNullOrEmpty(loggedInUserId) ? await _unitOfWork.Users.GetByIdAsync(loggedInUserId) : null;
            var isFactoryUser = loggedInUser != null && (loggedInUser.FKUserType == 3); // مصنع فقط

            vm.IsFactoryUser = isFactoryUser;
            if (isFactoryUser)
            {
                vm.FKUserId = loggedInUserId;
                if (!id.HasValue || id.Value == 0)
                {
                    vm.Address = loggedInUser?.Address;
                }
            }

            var DoneStatus = await _unitOfWork.Statuses.GetByIdAsync(x => x.ShortChar == "D");
            if (vm.StatusId == DoneStatus?.Id && vm.StatusId > 0) vm.isDisabled = true;

            vm.MainWastesList = SelectListHelper.BindSelectList(allMainWastes.ToList(), vm.FKMainWasteId).ToList();
            vm.SubWastesList = new List<SelectListItem>();
            vm.StatusesList = SelectListHelper.BindSelectList(allStatuses.ToList(), vm.StatusId).ToList();
            vm.UsersList = SelectListHelper.BindSelectList(allUserHasFactorysOnly.ToList(), null, "Id", "FullNameAr", "FullNameEn").ToList();

            if (!id.HasValue || id.Value == 0)
            {
                vm.OrderDate = AppDubaiTime.Now;
                return View(vm);
            }

            var entity = await _orderSellToFactoryService.GetByIdAsync(id.Value);
            if (entity == null)
            {
                return NotFound();
            }
            var currentStatusObj = await _unitOfWork.Statuses.GetByIdAsync(entity.StatusId);
            if (currentStatusObj != null && (currentStatusObj.ShortChar == "C" || currentStatusObj.ShortChar == "S"))
            {
                TempData["Error"] = "Cannot update status of this order because it is in a restricted status.";
                return RedirectToAction(nameof(Index));
            }

            vm = _mapper.Map<OrderSellToFactoryVM>(entity);
            vm.IsFactoryUser = isFactoryUser;
            if (isFactoryUser)
            {
                vm.FKUserId = loggedInUserId;
            }
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
            var statusesToBind = allStatuses.ToList();
            if (currentStatusObj != null && currentStatusObj.ShortChar == "D")
            {
                statusesToBind = statusesToBind.Where(s => s.ShortChar == "S" || s.ShortChar == "C" || s.Id == currentStatusObj.Id).ToList();
            }
            vm.StatusesList = SelectListHelper.BindSelectList(statusesToBind, vm.StatusId).ToList();
            vm.UsersList = SelectListHelper.BindSelectList(allUserHasFactorysOnly.ToList(), null, "Id", "FullNameAr", "FullNameEn").ToList();

            return View(vm);
        }

        [IgnoreAction]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(OrderSellToFactoryVM model)
        {
            var loggedInUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var loggedInUser = !string.IsNullOrEmpty(loggedInUserId) ? await _unitOfWork.Users.GetByIdAsync(loggedInUserId) : null;
            var isFactoryUser = loggedInUser != null && (loggedInUser.FKUserType == 3); // مصنع فقط

            var FkUserId = "";

            if (isFactoryUser)
            {
                model.FKUserId = loggedInUserId;
                model.IsFactoryUser = true;
            }

            if (!ModelState.IsValid)
            {
                var oldEntityForView = await _unitOfWork.OrderSellToFactorys.Table
                    .Include(x => x.Status)
                    .Include(x => x.SubWaste)
                    .FirstOrDefaultAsync(x => x.Id == model.Id);
                if (oldEntityForView != null)
                {
                    await PopulateUpdateStatusViewModelAsync(model, oldEntityForView);
                }

                return View(model);
            }

            var oldEntity = await _unitOfWork.OrderSellToFactorys.Table
                .Include(x => x.Status)
                .Include(x => x.SubWaste)
                .FirstOrDefaultAsync(x => x.Id == model.Id);
            if (oldEntity == null)
            {
                return NotFound();
            }
            FkUserId = oldEntity.FKUserId;

            string oldStatusChar = oldEntity.Status?.ShortChar ?? string.Empty;

            var newStatus = await _unitOfWork.Statuses.Table.FirstOrDefaultAsync(x => x.Id == model.StatusId);

            StatusTransitionValidationResult? transitionValidation = null;
            var requiresValidation = OrderSellToFactoryStatusValidator.RequiresBalanceAndRoomValidation(newStatus?.ShortChar, oldStatusChar);
            if (requiresValidation)
            {
                transitionValidation = await OrderSellToFactoryStatusValidator.ValidateTransitionToDoneAsync(_unitOfWork, oldEntity);
                if (!transitionValidation.Success)
                {
                    ModelState.AddModelError(string.Empty, transitionValidation.ErrorMessage!);
                    model.StatusId = oldEntity.StatusId;
                    await PopulateUpdateStatusViewModelAsync(model, oldEntity);
                    return View(model);
                }
            }

            oldEntity.StatusId = model.StatusId;
            await _orderSellToFactoryService.UpdateAsync(oldEntity);

            if (newStatus != null)
            {
                var financial = await _unitOfWork.Financials.Table.FirstOrDefaultAsync(x => x.TableType == "OrderSellToFactory" && x.ItsId == model.Id);

                if (oldStatusChar == "P" && newStatus.ShortChar == "D" && financial == null)
                {
                    var FKUserType = await _PermissionScanner.GetFKUserType(FkUserId);
                    await _unitOfWork.Financials.AddAsync(new Domain.Entities.Financial
                    {
                        TableType = "OrderSellToFactory",
                        ItsId = model.Id,
                        TypeTransaction = '-',
                        StatusId = model.StatusId,
                        Total = oldEntity.Total,
                        FKUserType = FKUserType,
                        FKUserId = FkUserId,
                    });
                    await _unitOfWork.CompleteAsync();
                }
                else if (financial != null)
                {
                    financial.StatusId = model.StatusId;
                    financial.Total = oldEntity.Total;
                    if (oldStatusChar == "P" && newStatus.ShortChar == "D")
                    {
                        financial.TypeTransaction = '-';
                    }
                    _unitOfWork.Financials.Update(financial);
                }

                // UserPoints logic: Add points when status becomes "S"
                if (oldStatusChar != "S" && newStatus.ShortChar == "S")
                {
                    var userId = oldEntity.FKUserId;
                    if (!string.IsNullOrEmpty(userId))
                    {
                        var userPointsTable = _unitOfWork.Context.Set<Domain.Entities.UserPoints>();
                        var userPoints = await userPointsTable.FirstOrDefaultAsync(x => x.FKUserId == userId);
                        
                        if (userPoints == null)
                        {
                            userPoints = new Domain.Entities.UserPoints
                            {
                                FKUserId = userId,
                                Totals = 0,
                                TotalsReNew = 0,
                                Points = 0
                            };
                            await userPointsTable.AddAsync(userPoints);
                        }

                        decimal currentTotal = (decimal)(oldEntity.Total ?? 0);
                        userPoints.Totals = (userPoints.Totals ?? 0) + currentTotal;
                        userPoints.TotalsReNew = (userPoints.TotalsReNew ?? 0) + currentTotal;

                        // Calculate points: Every 1000 = 20 points
                        userPoints.Points = (int)Math.Floor(((userPoints.TotalsReNew ?? 0) / 1000m) * 20m);

                        userPointsTable.Update(userPoints);
                    }
                }

                if (transitionValidation?.Success == true
                    && transitionValidation.SelectedRoomId.HasValue
                    && OrderSellToFactoryStatusValidator.RequiresRoomDeduction(newStatus.ShortChar, oldStatusChar))
                {
                    var room = await _unitOfWork.RoomInventories.GetByIdAsync(transitionValidation.SelectedRoomId.Value);
                    if (room != null)
                    {
                        room.MaxKilo = (room.MaxKilo ?? 0) - transitionValidation.RequiredKilos;
                        _unitOfWork.RoomInventories.Update(room);
                    }
                }

                await _unitOfWork.CompleteAsync();
            }

            if (newStatus != null && (newStatus.ShortChar == "S" || newStatus.ShortChar == "C"))
            {
                TempData["Success"] = "Status updated successfully.";
                return RedirectToAction(nameof(Index));
            }

            return RedirectToAction(nameof(UpdateStatus), new { id = model.Id });
        }


        [YesGet]
        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            var entity = await _orderSellToFactoryService.GetByIdAsync(id);
            if (entity == null)
            {
                return NotFound();
            }

            var vm = _mapper.Map<OrderSellToFactoryVM>(entity);

            var loggedInUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var loggedInUser = !string.IsNullOrEmpty(loggedInUserId) ? await _unitOfWork.Users.GetByIdAsync(loggedInUserId) : null;
            var isFactoryUser = loggedInUser != null &&( loggedInUser.FKUserType == 3); // مصنع فقط

            vm.IsFactoryUser = isFactoryUser;
            if (isFactoryUser)
            {
                vm.FKUserId = loggedInUserId;
            }

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
                if (entity.Kilo.Value < 1000)
                {
                    vm.IsKilosSelected = true;
                    vm.KilosValue = entity.Kilo.Value;
                }
                else
                {
                    vm.IsTonSelected = true;
                    vm.TonValue = entity.Kilo.Value / 1000;
                }
            }

            var allMainWastes = await _unitOfWork.MainWastes.GetAllAsync();
            var allSubWastes = await _unitOfWork.SubWastes.GetAllAsync();
            var allStatuses = await _unitOfWork.Statuses.GetAllAsync();
            var allUserHasFactorysOnly = await _unitOfWork.Users.GetAllAsync(x => x.FKUserType == 3);

            vm.MainWastesList = SelectListHelper.BindSelectList(allMainWastes.ToList(), vm.FKMainWasteId).ToList();
            vm.SubWastesList = SelectListHelper.BindSelectList(allSubWastes.Where(x => x.FKMainWasteId == vm.FKMainWasteId).ToList(), vm.FKSubWasteId).ToList();
            vm.StatusesList = SelectListHelper.BindSelectList(allStatuses.ToList(), vm.StatusId).ToList();
            vm.UsersList = SelectListHelper.BindSelectList(allUserHasFactorysOnly.ToList(), null, "Id", "FullNameAr", "FullNameEn").ToList();

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                return NotFound();
            }

            var hasRelated = await _orderSellToFactoryService.HasRelatedObjectsInDb(id);
            if (hasRelated)
            {
                TempData["Error"] = "Cannot delete this item because it has related records.";
                return RedirectToAction(nameof(Index));
            }

            await _orderSellToFactoryService.DeleteAsync(id);
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
        [HttpGet]
        public async Task<IActionResult> GetUserAddress(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return Json(new { success = false });
            }
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
            {
                return Json(new { success = false });
            }
            return Json(new { success = true, address = user.Address });
        }

        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> Print(string? searchTerm, int? mainWasteId, int? subWasteId)
        {
            var items = await _orderSellToFactoryService.GetAllAsync(searchTerm, mainWasteId, subWasteId);

            var loggedInUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var loggedInUser = !string.IsNullOrEmpty(loggedInUserId) ? await _unitOfWork.Users.GetByIdAsync(loggedInUserId) : null;
            var isFactoryUser = loggedInUser != null &&( loggedInUser.FKUserType == 3); // مصنع فقط

            if (isFactoryUser)
            {
                items = items.Where(x => x.FKUserId == loggedInUserId).ToList();
            }

            var allMainWastes = await _unitOfWork.MainWastes.GetAllAsync();
            var allSubWastes = await _unitOfWork.SubWastes.GetAllAsync();
            var allStatuses = await _unitOfWork.Statuses.GetAllAsync();
            var allUserHasFactorysOnly = await _unitOfWork.Users.GetAllAsync(x => x.FKUserType == 3);

            var vm = new OrderSellToFactoryVM
            {
                Items = items,
                SearchString = searchTerm,
                MainWasteFilterId = mainWasteId,
                SubWasteFilterId = subWasteId,
                TotalCount = items.Count(),
                MainWastesList = SelectListHelper.BindSelectList(allMainWastes.ToList(), mainWasteId).ToList(),
                SubWastesList = SelectListHelper.BindSelectList(allSubWastes.ToList(), subWasteId).ToList(),
                StatusesList = SelectListHelper.BindSelectList(allStatuses.ToList(), null).ToList(),
                UsersList = SelectListHelper.BindSelectList(allUserHasFactorysOnly.ToList(), null, "Id", "FullNameAr", "FullNameEn").ToList()
            };

            return View(vm);
        }

        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> createExcelReport_Download(string? searchTerm, int? mainWasteId, int? subWasteId)
        {
            var items = await _orderSellToFactoryService.GetAllAsync(searchTerm, mainWasteId, subWasteId);

            var loggedInUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var loggedInUser = !string.IsNullOrEmpty(loggedInUserId) ? await _unitOfWork.Users.GetByIdAsync(loggedInUserId) : null;
            var isFactoryUser = loggedInUser != null &&( loggedInUser.FKUserType == 3); // مصنع فقط

            if (isFactoryUser)
            {
                items = items.Where(x => x.FKUserId == loggedInUserId).ToList();
            }

            var list = items.ToList();
            var allUserHasFactorysOnly = await _unitOfWork.Users.GetAllAsync(x => x.FKUserType == 3);
            var usersList = SelectListHelper.BindSelectList(allUserHasFactorysOnly.ToList(), null, "Id", "FullNameAr", "FullNameEn").ToList();

            if (!list.Any())
            {
                return RedirectToAction(nameof(Index), new { searchTerm, mainWasteId, subWasteId });
            }

            var titles = new List<string> { "Factory", "Sub Waste", "Main Waste", "Order Date", "Units", "Kilo", "Discount", "Total", "Status" };
            var excelData = list.Select(x => new ExcelDataDTO
            {
                t1 = usersList.FirstOrDefault(u => u.Value == x.FKUserId)?.Text ?? string.Empty,
                t2 = SessionHelper.GetCurrentLanguage() == "ar" ? x.SubWaste?.NameAr ?? string.Empty : x.SubWaste?.NameEn ?? string.Empty,
                t3 = SessionHelper.GetCurrentLanguage() == "ar" ? x.SubWaste?.MainWaste?.NameAr ?? string.Empty : x.SubWaste?.MainWaste?.NameEn ?? string.Empty,
                t4 = x.OrderDate?.ToString("yyyy-MM-dd") ?? string.Empty,
                t5 = x.CountUnits?.ToString() ?? string.Empty,
                t6 = x.Kilo?.ToString("F2") ?? string.Empty,
                t7 = x.DiscountValue?.ToString("F2") ?? string.Empty,
                t8 = x.Total?.ToString("F2") ?? string.Empty,
                t9 = SessionHelper.GetCurrentLanguage() == "ar" ? x.Status?.NameAr ?? string.Empty : x.Status?.NameEn ?? string.Empty
            }).ToList();

            var (ok, bytes) = ExcelStaticReport.ExcelReportArEn_(excelData, titles, 0, "ar");
            if (!ok || bytes == null || bytes.Length == 0)
            {
                return RedirectToAction(nameof(Index), new { searchTerm, mainWasteId, subWasteId });
            }

            return File(
                bytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"OrderSellToFactorys_{AppDubaiTime.Now:yyyyMMdd_HHmmss}.xlsx");
        }

        [IgnoreAction]
        [HttpGet]
        public async Task<IActionResult> GetOrderAttachments(int id, string returnAction = "AddEdit")
        {
            var attachments = await _unitOfWork.OrderSellToFactoryAttachments.Table
                .Where(a => a.OrderSellToFactoryId == id)
                .ToListAsync();

            var vm = new OrderSellToFactoryAttachmentsVM
            {
                OrderSellToFactoryId = id,
                ReturnAction = returnAction,
                Attachments = attachments
            };

            return PartialView("_OrderAttachmentsModal", vm);
        }

        [IgnoreAction]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadOrderFiles(OrderSellToFactoryAttachmentsVM model)
        {
            var uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Files", "Orders");
            if (!Directory.Exists(uploadDir))
            {
                Directory.CreateDirectory(uploadDir);
            }

            // 1. Get existing attachments from DB
            var existingAttachments = await _unitOfWork.OrderSellToFactoryAttachments.Table
                .Where(a => a.OrderSellToFactoryId == model.OrderSellToFactoryId)
                .ToListAsync();

            // 2. Identify attachments that were removed in the UI
            var submittedPaths = model.Attachments?.Where(a => a.Path != null).Select(a => a.Path).ToList() ?? new List<string>();
            var attachmentsToRemove = existingAttachments.Where(ea => !submittedPaths.Contains(ea.Path)).ToList();

            // 3. Remove them from DB & File System
            foreach (var toRemove in attachmentsToRemove)
            {
                if (!string.IsNullOrEmpty(toRemove.Path))
                {
                    var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", toRemove.Path.TrimStart('/', '\\'));
                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                    }
                }
                _unitOfWork.OrderSellToFactoryAttachments.Delete(toRemove);
            }

            // 4. Save new files
            if (model.Attachments != null)
            {
                foreach (var attachment in model.Attachments)
                {
                    if (attachment.File != null && attachment.File.Length > 0)
                    {
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(attachment.File.FileName);
                        var filePath = Path.Combine(uploadDir, fileName);

                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await attachment.File.CopyToAsync(stream);
                        }

                        var newAttachment = new Domain.Entities.Waste.OrderSellToFactoryAttachment
                        {
                            Name = attachment.Name ?? attachment.File.FileName,
                            Path = $"Files/Orders/{fileName}",
                            OrderSellToFactoryId = model.OrderSellToFactoryId
                        };

                        await _unitOfWork.OrderSellToFactoryAttachments.AddAsync(newAttachment);
                    }
                }
            }

            await _unitOfWork.CompleteAsync();

            TempData["Success"] = Domain.Resources.Resource2.ToastDone;

            if (string.Equals(model.ReturnAction, nameof(UpdateStatus), StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction(nameof(UpdateStatus), new { id = model.OrderSellToFactoryId });
            }

            return RedirectToAction(nameof(AddEdit), new { id = model.OrderSellToFactoryId });
        }

        private async Task PopulateUpdateStatusViewModelAsync(OrderSellToFactoryVM model, OrderSellToFactory entity)
        {
            var allMainWastes = await _unitOfWork.MainWastes.GetAllAsync();
            var allSubWastes = await _unitOfWork.SubWastes.GetAllAsync();
            var allStatuses = await _unitOfWork.Statuses.GetAllAsync();
            var allUserHasFactorysOnly = await _unitOfWork.Users.GetAllAsync(x => x.FKUserType == 3);

            model.FKSubWasteId ??= entity.FKSubWasteId;
            model.Total ??= entity.Total;
            model.Address ??= entity.Address;
            model.DiscountRatio ??= entity.DiscountRatio;
            model.DiscountValue ??= entity.DiscountValue;

            if (entity.SubWaste != null)
            {
                model.FKMainWasteId = entity.SubWaste.FKMainWasteId;
                model.BuyPriceUnit = entity.SubWaste.BuyPriceUnit;
                model.BuyPriceKilo = entity.SubWaste.BuyPriceKilo;
                model.BuyPriceTon = entity.SubWaste.BuyPriceTon;
            }

            model.IsUnitsSelected = false;
            model.IsKilosSelected = false;
            model.IsTonSelected = false;
            model.UnitsValue = null;
            model.KilosValue = null;
            model.TonValue = null;

            if (entity.CountUnits.HasValue && entity.CountUnits.Value > 0)
            {
                model.IsUnitsSelected = true;
                model.UnitsValue = entity.CountUnits.Value;
            }

            if (entity.Kilo.HasValue && entity.Kilo.Value > 0)
            {
                if (entity.Kilo.Value < 1000)
                {
                    model.IsKilosSelected = true;
                    model.KilosValue = entity.Kilo.Value;
                }
                else
                {
                    model.IsTonSelected = true;
                    model.TonValue = entity.Kilo.Value / 1000;
                }
            }

            model.MainWastesList = SelectListHelper.BindSelectList(allMainWastes.ToList(), model.FKMainWasteId).ToList();
            model.SubWastesList = SelectListHelper.BindSelectList(allSubWastes.Where(x => x.FKMainWasteId == model.FKMainWasteId).ToList(), model.FKSubWasteId).ToList();

            var statusesToBind = allStatuses.ToList();
            if (entity.Status?.ShortChar == "D")
            {
                statusesToBind = statusesToBind.Where(s => s.ShortChar == "S" || s.ShortChar == "C" || s.Id == entity.StatusId).ToList();
            }

            model.StatusesList = SelectListHelper.BindSelectList(statusesToBind, model.StatusId).ToList();
            model.UsersList = SelectListHelper.BindSelectList(allUserHasFactorysOnly.ToList(), null, "Id", "FullNameAr", "FullNameEn").ToList();
        }
    }
}
