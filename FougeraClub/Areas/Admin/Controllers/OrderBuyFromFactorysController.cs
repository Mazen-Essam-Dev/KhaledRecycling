using Application.Helpers;
using Application.Interfaces.Admin;
using Application.Services.Admin;
using AutoMapper;
using Domain.DTOs;
using Domain.Entities.Product;
using Infrastructure.Repositories.InterfacesDB;
using KhaledTeamRecycling.Areas.Admin.ViewModels.OrderBuyFromFactory;
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
    public class OrderBuyFromFactorysController : Controller
    {
        private readonly IOrderBuyFromFactoryService _orderBuyFromFactoryService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly PermissionScanner _PermissionScanner;

        public OrderBuyFromFactorysController(IOrderBuyFromFactoryService orderBuyFromFactoryService,PermissionScanner PermissionScanner, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _orderBuyFromFactoryService = orderBuyFromFactoryService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _PermissionScanner = PermissionScanner;
        }

        [YesGet]
        public async Task<IActionResult> Index(string? searchTerm, int? mainProductId, int? subProductId, int page = 1, int pageSize = 50)
        {
            var query = _unitOfWork.OrderBuyFromFactorys.Table
                .Include(x => x.SubProduct!)
                .ThenInclude(x => x.MainProduct!)
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
                    (x.SubProduct != null && x.SubProduct.NameAr != null && x.SubProduct.NameAr.Contains(searchTerm)) ||
                    (x.SubProduct != null && x.SubProduct.NameEn != null && x.SubProduct.NameEn.Contains(searchTerm)));
            }

            if (mainProductId.HasValue && mainProductId.Value > 0)
            {
                query = query.Where(x => x.SubProduct != null && x.SubProduct.FKMainProductId == mainProductId.Value);
            }

            if (subProductId.HasValue && subProductId.Value > 0)
            {
                query = query.Where(x => x.FKSubProductId == subProductId.Value);
            }

            var totalRecords = await query.CountAsync();
            var items = await query
                .OrderByDescending(x => x.OrderDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var allMainProducts = await _unitOfWork.MainProducts.GetAllAsync();
            var allSubProducts = await _unitOfWork.SubProducts.GetAllAsync();
            var allStatuses = await _unitOfWork.Statuses.GetAllAsync();

            List<SelectListItem> subProductsList;
            if (mainProductId.HasValue && mainProductId.Value > 0)
            {
                subProductsList = SelectListHelper.BindSelectList(allSubProducts.Where(x => x.FKMainProductId == mainProductId.Value).ToList(), subProductId).ToList();
            }
            else
            {
                subProductsList = new List<SelectListItem>();
            }
            var allUserHasFactorysOnly = await _unitOfWork.Users.GetAllAsync(x => x.FKUserType == 3);

            var vm = new OrderBuyFromFactoryVM
            {
                IsFactoryUser = isFactoryUser,
                Items = items,
                SearchString = searchTerm,
                MainProductFilterId = mainProductId,
                SubProductFilterId = subProductId,
                CurrentPage = page,
                PageSize = pageSize,
                TotalCount = totalRecords,
                TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize),
                HasNextPage = totalRecords > pageSize * page,
                HasPreviousPage = page > 1,
                MainProductsList = SelectListHelper.BindSelectList(allMainProducts.ToList(), mainProductId).ToList(),
                SubProductsList = subProductsList,
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

            var vm = new OrderBuyFromFactoryVM();
            var allMainProducts = await _unitOfWork.MainProducts.GetAllAsync();
            var allSubProducts = await _unitOfWork.SubProducts.GetAllAsync();
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

            vm.MainProductsList = SelectListHelper.BindSelectList(allMainProducts.ToList(), vm.FKMainProductId).ToList();
            vm.SubProductsList = new List<SelectListItem>();
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

            var entity = await _orderBuyFromFactoryService.GetByIdAsync(id.Value);
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

            vm = _mapper.Map<OrderBuyFromFactoryVM>(entity);
            vm.IsFactoryUser = isFactoryUser;
            if (isFactoryUser)
            {
                vm.FKUserId = loggedInUserId;
            }
            if (entity.SubProduct != null)
            {
                vm.FKMainProductId = entity.SubProduct.FKMainProductId;
                vm.BuyPriceUnit = entity.SubProduct.BuyPriceUnit;
                vm.BuyPriceKilo = entity.SubProduct.BuyPriceKilo;
                vm.BuyPriceTon = entity.SubProduct.BuyPriceTon;
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

            vm.MainProductsList = SelectListHelper.BindSelectList(allMainProducts.ToList(), vm.FKMainProductId).ToList();
            vm.SubProductsList = SelectListHelper.BindSelectList(allSubProducts.Where(x => x.FKMainProductId == vm.FKMainProductId).ToList(), vm.FKSubProductId).ToList();
            vm.StatusesList = SelectListHelper.BindSelectList(allStatuses.ToList(), vm.StatusId).ToList();
            vm.UsersList = SelectListHelper.BindSelectList(allUserHasFactorysOnly.ToList(), null, "Id", "FullNameAr", "FullNameEn").ToList();

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEdit(OrderBuyFromFactoryVM model)
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
                var allMainProducts = await _unitOfWork.MainProducts.GetAllAsync();
                var allSubProducts = await _unitOfWork.SubProducts.GetAllAsync();
                var allStatuses = await _unitOfWork.Statuses.GetAllAsync();
                var allUserHasFactorysOnly = await _unitOfWork.Users.GetAllAsync(x => x.FKUserType == 3);

                model.MainProductsList = SelectListHelper.BindSelectList(allMainProducts.ToList(), model.FKMainProductId).ToList();
                model.SubProductsList = SelectListHelper.BindSelectList(allSubProducts.Where(x => x.FKMainProductId == model.FKMainProductId).ToList(), model.FKSubProductId).ToList();
                model.StatusesList = SelectListHelper.BindSelectList(allStatuses.ToList(), model.StatusId).ToList();
                model.UsersList = SelectListHelper.BindSelectList(allUserHasFactorysOnly.ToList(), null, "Id", "FullNameAr", "FullNameEn").ToList();

                return View(model);
            }

            var entity = _mapper.Map<Domain.Entities.Product.OrderBuyFromFactory>(model);

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
                await _orderBuyFromFactoryService.AddAsync(entity);
                if (pointsDiscountApplied)
                    await _unitOfWork.CompleteAsync();
                return RedirectToAction(nameof(AddEdit), new { id = entity.Id });
            }

            FkUserId = entity.FKUserId;
            await _orderBuyFromFactoryService.UpdateAsync(entity);


            var financial = await _unitOfWork.Financials.Table.FirstOrDefaultAsync(x => x.TableType == "OrderBuyFromFactory" && x.ItsId == model.Id);
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

            var vm = new OrderBuyFromFactoryVM();
            var allMainProducts = await _unitOfWork.MainProducts.GetAllAsync();
            var allSubProducts = await _unitOfWork.SubProducts.GetAllAsync();
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

            vm.MainProductsList = SelectListHelper.BindSelectList(allMainProducts.ToList(), vm.FKMainProductId).ToList();
            vm.SubProductsList = new List<SelectListItem>();
            vm.StatusesList = SelectListHelper.BindSelectList(allStatuses.ToList(), vm.StatusId).ToList();
            vm.UsersList = SelectListHelper.BindSelectList(allUserHasFactorysOnly.ToList(), null, "Id", "FullNameAr", "FullNameEn").ToList();

            if (!id.HasValue || id.Value == 0)
            {
                vm.OrderDate = AppDubaiTime.Now;
                return View(vm);
            }

            var entity = await _orderBuyFromFactoryService.GetByIdAsync(id.Value);
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

            vm = _mapper.Map<OrderBuyFromFactoryVM>(entity);
            vm.IsFactoryUser = isFactoryUser;
            if (isFactoryUser)
            {
                vm.FKUserId = loggedInUserId;
            }
            if (entity.SubProduct != null)
            {
                vm.FKMainProductId = entity.SubProduct.FKMainProductId;
                vm.BuyPriceUnit = entity.SubProduct.BuyPriceUnit;
                vm.BuyPriceKilo = entity.SubProduct.BuyPriceKilo;
                vm.BuyPriceTon = entity.SubProduct.BuyPriceTon;
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

            vm.MainProductsList = SelectListHelper.BindSelectList(allMainProducts.ToList(), vm.FKMainProductId).ToList();
            vm.SubProductsList = SelectListHelper.BindSelectList(allSubProducts.Where(x => x.FKMainProductId == vm.FKMainProductId).ToList(), vm.FKSubProductId).ToList();
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
        public async Task<IActionResult> UpdateStatus(OrderBuyFromFactoryVM model)
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
                var oldEntityForView = await _unitOfWork.OrderBuyFromFactorys.Table
                    .Include(x => x.Status)
                    .Include(x => x.SubProduct)
                    .FirstOrDefaultAsync(x => x.Id == model.Id);
                if (oldEntityForView != null)
                {
                    await PopulateUpdateStatusViewModelAsync(model, oldEntityForView);
                }

                return View(model);
            }

            var oldEntity = await _unitOfWork.OrderBuyFromFactorys.Table
                .Include(x => x.Status)
                .Include(x => x.SubProduct)
                .FirstOrDefaultAsync(x => x.Id == model.Id);
            if (oldEntity == null)
            {
                return NotFound();
            }
            FkUserId = oldEntity.FKUserId;

            string oldStatusChar = oldEntity.Status?.ShortChar ?? string.Empty;

            var newStatus = await _unitOfWork.Statuses.Table.FirstOrDefaultAsync(x => x.Id == model.StatusId);

            StatusTransitionValidationResult? transitionValidation = null;
            var requiresValidation = OrderBuyFromFactoryStatusValidator.RequiresBalanceAndRoomValidation(newStatus?.ShortChar, oldStatusChar);
            if (requiresValidation)
            {
                transitionValidation = await OrderBuyFromFactoryStatusValidator.ValidateTransitionToDoneAsync(_unitOfWork, oldEntity);
                if (!transitionValidation.Success)
                {
                    ModelState.AddModelError(string.Empty, transitionValidation.ErrorMessage!);
                    model.StatusId = oldEntity.StatusId;
                    await PopulateUpdateStatusViewModelAsync(model, oldEntity);
                    return View(model);
                }
            }

            oldEntity.StatusId = model.StatusId;
            await _orderBuyFromFactoryService.UpdateAsync(oldEntity);

            if (newStatus != null)
            {
                var financial = await _unitOfWork.Financials.Table.FirstOrDefaultAsync(x => x.TableType == "OrderBuyFromFactory" && x.ItsId == model.Id);

                if (oldStatusChar == "P" && newStatus.ShortChar == "D" && financial == null)
                {
                    var FKUserType = await _PermissionScanner.GetFKUserType(FkUserId);
                    await _unitOfWork.Financials.AddAsync(new Domain.Entities.Financial
                    {
                        TableType = "OrderBuyFromFactory",
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
                    && OrderBuyFromFactoryStatusValidator.RequiresRoomDeduction(newStatus.ShortChar, oldStatusChar))
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

            var entity = await _orderBuyFromFactoryService.GetByIdAsync(id);
            if (entity == null)
            {
                return NotFound();
            }

            var vm = _mapper.Map<OrderBuyFromFactoryVM>(entity);

            var loggedInUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var loggedInUser = !string.IsNullOrEmpty(loggedInUserId) ? await _unitOfWork.Users.GetByIdAsync(loggedInUserId) : null;
            var isFactoryUser = loggedInUser != null &&( loggedInUser.FKUserType == 3); // مصنع فقط

            vm.IsFactoryUser = isFactoryUser;
            if (isFactoryUser)
            {
                vm.FKUserId = loggedInUserId;
            }

            ViewBag.phone = $"{loggedInUser?.PhoneNumber}";
            ViewBag.phone += loggedInUser?.Phone2?.Length > 2 ? $" - {loggedInUser?.Phone2}" : "";

            if (entity.SubProduct != null)
            {
                vm.FKMainProductId = entity.SubProduct.FKMainProductId;
                vm.BuyPriceUnit = entity.SubProduct.BuyPriceUnit;
                vm.BuyPriceKilo = entity.SubProduct.BuyPriceKilo;
                vm.BuyPriceTon = entity.SubProduct.BuyPriceTon;
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

            var allMainProducts = await _unitOfWork.MainProducts.GetAllAsync();
            var allSubProducts = await _unitOfWork.SubProducts.GetAllAsync();
            var allStatuses = await _unitOfWork.Statuses.GetAllAsync();
            var allUserHasFactorysOnly = await _unitOfWork.Users.GetAllAsync(x => x.FKUserType == 3);

            vm.MainProductsList = SelectListHelper.BindSelectList(allMainProducts.ToList(), vm.FKMainProductId).ToList();
            vm.SubProductsList = SelectListHelper.BindSelectList(allSubProducts.Where(x => x.FKMainProductId == vm.FKMainProductId).ToList(), vm.FKSubProductId).ToList();
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

            var hasRelated = await _orderBuyFromFactoryService.HasRelatedObjectsInDb(id);
            if (hasRelated)
            {
                TempData["Error"] = "Cannot delete this item because it has related records.";
                return RedirectToAction(nameof(Index));
            }

            await _orderBuyFromFactoryService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [IgnoreAction]
        [HttpGet]
        public async Task<IActionResult> GetSubProductsByMainProduct(int mainProductId)
        {
            var subProducts = await _unitOfWork.SubProducts.Table
                .Where(x => x.FKMainProductId == mainProductId  && x.StatusChar == null) // only active subProduct
                .Select(x => new { x.Id, x.NameAr, x.NameEn, x.BuyPriceUnit, x.BuyPriceKilo, x.BuyPriceTon })
                .ToListAsync();

            return Json(subProducts);
        }

        [IgnoreAction]
        [HttpGet]
        public async Task<IActionResult> GetSubProductPrices(int subProductId)
        {
            var subProduct = await _unitOfWork.SubProducts.GetByIdAsync(subProductId);
            if (subProduct == null)
            {
                return Json(new { success = false });
            }

            return Json(new
            {
                success = true,
                buyPriceUnit = subProduct.BuyPriceUnit,
                buyPriceKilo = subProduct.BuyPriceKilo,
                buyPriceTon = subProduct.BuyPriceTon
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
        public async Task<IActionResult> Print(string? searchTerm, int? mainProductId, int? subProductId)
        {
            var items = await _orderBuyFromFactoryService.GetAllAsync(searchTerm, mainProductId, subProductId);

            var loggedInUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var loggedInUser = !string.IsNullOrEmpty(loggedInUserId) ? await _unitOfWork.Users.GetByIdAsync(loggedInUserId) : null;
            var isFactoryUser = loggedInUser != null &&( loggedInUser.FKUserType == 3); // مصنع فقط

            if (isFactoryUser)
            {
                items = items.Where(x => x.FKUserId == loggedInUserId).ToList();
            }

            var allMainProducts = await _unitOfWork.MainProducts.GetAllAsync();
            var allSubProducts = await _unitOfWork.SubProducts.GetAllAsync();
            var allStatuses = await _unitOfWork.Statuses.GetAllAsync();
            var allUserHasFactorysOnly = await _unitOfWork.Users.GetAllAsync(x => x.FKUserType == 3);

            var vm = new OrderBuyFromFactoryVM
            {
                Items = items,
                SearchString = searchTerm,
                MainProductFilterId = mainProductId,
                SubProductFilterId = subProductId,
                TotalCount = items.Count(),
                MainProductsList = SelectListHelper.BindSelectList(allMainProducts.ToList(), mainProductId).ToList(),
                SubProductsList = SelectListHelper.BindSelectList(allSubProducts.ToList(), subProductId).ToList(),
                StatusesList = SelectListHelper.BindSelectList(allStatuses.ToList(), null).ToList(),
                UsersList = SelectListHelper.BindSelectList(allUserHasFactorysOnly.ToList(), null, "Id", "FullNameAr", "FullNameEn").ToList()
            };

            return View(vm);
        }

        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> createExcelReport_Download(string? searchTerm, int? mainProductId, int? subProductId)
        {
            var items = await _orderBuyFromFactoryService.GetAllAsync(searchTerm, mainProductId, subProductId);

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
                return RedirectToAction(nameof(Index), new { searchTerm, mainProductId, subProductId });
            }

            var titles = new List<string> { "Factory", "Sub Product", "Main Product", "Order Date", "Units", "Kilo", "Discount", "Total", "Status" };
            var excelData = list.Select(x => new ExcelDataDTO
            {
                t1 = usersList.FirstOrDefault(u => u.Value == x.FKUserId)?.Text ?? string.Empty,
                t2 = SessionHelper.GetCurrentLanguage() == "ar" ? x.SubProduct?.NameAr ?? string.Empty : x.SubProduct?.NameEn ?? string.Empty,
                t3 = SessionHelper.GetCurrentLanguage() == "ar" ? x.SubProduct?.MainProduct?.NameAr ?? string.Empty : x.SubProduct?.MainProduct?.NameEn ?? string.Empty,
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
                return RedirectToAction(nameof(Index), new { searchTerm, mainProductId, subProductId });
            }

            return File(
                bytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"OrderBuyFromFactorys_{AppDubaiTime.Now:yyyyMMdd_HHmmss}.xlsx");
        }

        [IgnoreAction]
        [HttpGet]
        public async Task<IActionResult> GetOrderAttachments(int id, string returnAction = "AddEdit")
        {
            var attachments = await _unitOfWork.OrderBuyFromFactoryAttachments.Table
                .Where(a => a.OrderBuyFromFactoryId == id)
                .ToListAsync();

            var vm = new OrderBuyFromFactoryAttachmentsVM
            {
                OrderBuyFromFactoryId = id,
                ReturnAction = returnAction,
                Attachments = attachments
            };

            return PartialView("_OrderAttachmentsModal", vm);
        }

        [IgnoreAction]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadOrderFiles(OrderBuyFromFactoryAttachmentsVM model)
        {
            var uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Files", "Orders");
            if (!Directory.Exists(uploadDir))
            {
                Directory.CreateDirectory(uploadDir);
            }

            // 1. Get existing attachments from DB
            var existingAttachments = await _unitOfWork.OrderBuyFromFactoryAttachments.Table
                .Where(a => a.OrderBuyFromFactoryId == model.OrderBuyFromFactoryId)
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
                _unitOfWork.OrderBuyFromFactoryAttachments.Delete(toRemove);
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

                        var newAttachment = new Domain.Entities.Product.OrderBuyFromFactoryAttachment
                        {
                            Name = attachment.Name ?? attachment.File.FileName,
                            Path = $"Files/Orders/{fileName}",
                            OrderBuyFromFactoryId = model.OrderBuyFromFactoryId
                        };

                        await _unitOfWork.OrderBuyFromFactoryAttachments.AddAsync(newAttachment);
                    }
                }
            }

            await _unitOfWork.CompleteAsync();

            TempData["Success"] = Domain.Resources.Resource2.ToastDone;

            if (string.Equals(model.ReturnAction, nameof(UpdateStatus), StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction(nameof(UpdateStatus), new { id = model.OrderBuyFromFactoryId });
            }

            return RedirectToAction(nameof(AddEdit), new { id = model.OrderBuyFromFactoryId });
        }

        private async Task PopulateUpdateStatusViewModelAsync(OrderBuyFromFactoryVM model, OrderBuyFromFactory entity)
        {
            var allMainProducts = await _unitOfWork.MainProducts.GetAllAsync();
            var allSubProducts = await _unitOfWork.SubProducts.GetAllAsync();
            var allStatuses = await _unitOfWork.Statuses.GetAllAsync();
            var allUserHasFactorysOnly = await _unitOfWork.Users.GetAllAsync(x => x.FKUserType == 3);

            model.FKSubProductId ??= entity.FKSubProductId;
            model.Total ??= entity.Total;
            model.Address ??= entity.Address;
            model.DiscountRatio ??= entity.DiscountRatio;
            model.DiscountValue ??= entity.DiscountValue;

            if (entity.SubProduct != null)
            {
                model.FKMainProductId = entity.SubProduct.FKMainProductId;
                model.BuyPriceUnit = entity.SubProduct.BuyPriceUnit;
                model.BuyPriceKilo = entity.SubProduct.BuyPriceKilo;
                model.BuyPriceTon = entity.SubProduct.BuyPriceTon;
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

            model.MainProductsList = SelectListHelper.BindSelectList(allMainProducts.ToList(), model.FKMainProductId).ToList();
            model.SubProductsList = SelectListHelper.BindSelectList(allSubProducts.Where(x => x.FKMainProductId == model.FKMainProductId).ToList(), model.FKSubProductId).ToList();

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
