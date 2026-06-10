using Application.Helpers;
using Application.Interfaces.Admin;
using Application.Services.Admin;
using AutoMapper;
using DocumentFormat.OpenXml.Vml.Office;
using Domain.DTOs;
using Domain.Entities.Product;
using Infrastructure.Repositories.InterfacesDB;
using KhaledTeamRecycling.Areas.Admin.ViewModels.OrderSellToClient;
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
    public class OrderSellToClientsController : Controller
    {
        private readonly IOrderSellToClientService _orderSellToClientService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly PermissionScanner _PermissionScanner;

        public OrderSellToClientsController(IOrderSellToClientService orderSellToClientService, PermissionScanner PermissionScanner, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _orderSellToClientService = orderSellToClientService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _PermissionScanner = PermissionScanner;
        }

        [YesGet]
        public async Task<IActionResult> Index(string? searchTerm, int? mainProductId, int? subProductId, int page = 1, int pageSize = 50)
        {
            var query = _unitOfWork.OrderSellToClients.Table
                .Include(x => x.SubProduct!)
                .ThenInclude(x => x.MainProduct!)
                .Include(x => x.Status!)
                .AsQueryable();

            var loggedInUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var loggedInUser = !string.IsNullOrEmpty(loggedInUserId) ? await _unitOfWork.Users.GetByIdAsync(loggedInUserId) : null;
            var isClientUser = loggedInUser != null &&( loggedInUser.FKUserType == 1 || loggedInUser.FKUserType == 2); // شركة او فرد

            if (isClientUser)
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
            var allUserHasIndividualsOnly = await _unitOfWork.Users.GetAllAsync(x => x.FKUserType == 1 || x.FKUserType == 2);

            var vm = new OrderSellToClientVM
            {
                IsClientUser = isClientUser,
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
                UsersList = SelectListHelper.BindSelectList(allUserHasIndividualsOnly.ToList(), null, "Id", "FullNameAr", "FullNameEn").ToList()
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

            var vm = new OrderSellToClientVM();
            var allMainProducts = await _unitOfWork.MainProducts.GetAllAsync();
            var allSubProducts = await _unitOfWork.SubProducts.GetAllAsync();
            var allStatuses = await _unitOfWork.Statuses.GetAllAsync();
            var allUserHasIndividualsOnly = await _unitOfWork.Users.GetAllAsync(x=>x.FKUserType==1 || x.FKUserType == 2);

            var loggedInUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var loggedInUser = !string.IsNullOrEmpty(loggedInUserId) ? await _unitOfWork.Users.GetByIdAsync(loggedInUserId) : null;
            var isClientUser = loggedInUser != null &&( loggedInUser.FKUserType == 1 || loggedInUser.FKUserType == 2); // شركة او فرد

            vm.IsClientUser = isClientUser;
            if (isClientUser)
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
            vm.UsersList = SelectListHelper.BindSelectList(allUserHasIndividualsOnly.ToList(), null, "Id", "FullNameAr", "FullNameEn").ToList();

            if (!id.HasValue || id.Value == 0)
            {
                vm.OrderDate = AppDubaiTime.Now;

                if (isClientUser && !string.IsNullOrEmpty(loggedInUserId))
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

            var entity = await _orderSellToClientService.GetByIdAsync(id.Value);
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

            vm = _mapper.Map<OrderSellToClientVM>(entity);
            vm.IsClientUser = isClientUser;
            if (isClientUser)
            {
                vm.FKUserId = loggedInUserId;
            }
            if (entity.SubProduct != null)
            {
                vm.FKMainProductId = entity.SubProduct.FKMainProductId;
                vm.SellPriceUnit = entity.SubProduct.SellPriceUnit;
                vm.SellPriceKilo = entity.SubProduct.SellPriceKilo;
                vm.SellPriceTon = entity.SubProduct.SellPriceTon;
            }

            // Determine which checkboxes should be checked based on stored values
            if (entity.CountUnits.HasValue && entity.CountUnits.Value > 0)
            {
                vm.IsUnitsSelected = true;
                vm.UnitsValue = entity.CountUnits.Value;
            }

            //if (entity.Kilo.HasValue && entity.Kilo.Value > 0)
            //{
            //    // Check if kilo value is less than 1000, treat as kilos
            //    if (entity.Kilo.Value < 1000)
            //    {
            //        vm.IsKilosSelected = true;
            //        vm.KilosValue = entity.Kilo.Value;
            //    }
            //    else
            //    {
            //        // If kilo value is 1000 or more, treat as tons
            //        vm.IsTonSelected = true;
            //        vm.TonValue = entity.Kilo.Value / 1000;
            //    }
            //}

            vm.MainProductsList = SelectListHelper.BindSelectList(allMainProducts.ToList(), vm.FKMainProductId).ToList();
            vm.SubProductsList = SelectListHelper.BindSelectList(allSubProducts.Where(x => x.FKMainProductId == vm.FKMainProductId).ToList(), vm.FKSubProductId).ToList();
            vm.StatusesList = SelectListHelper.BindSelectList(allStatuses.ToList(), vm.StatusId).ToList();
            vm.UsersList = SelectListHelper.BindSelectList(allUserHasIndividualsOnly.ToList(), null, "Id", "FullNameAr", "FullNameEn").ToList();

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEdit(OrderSellToClientVM model)
        {
            var loggedInUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var loggedInUser = !string.IsNullOrEmpty(loggedInUserId) ? await _unitOfWork.Users.GetByIdAsync(loggedInUserId) : null;
            var isClientUser = loggedInUser != null && loggedInUser.FKUserType == 1;
            var FKUserId = "";
            if (isClientUser)
            {
                model.FKUserId = loggedInUserId;
                model.IsClientUser = true;
            }

            if (!ModelState.IsValid)
            {
                var allMainProducts = await _unitOfWork.MainProducts.GetAllAsync();
                var allSubProducts = await _unitOfWork.SubProducts.GetAllAsync();
                var allStatuses = await _unitOfWork.Statuses.GetAllAsync();
                var allUserHasIndividualsOnly = await _unitOfWork.Users.GetAllAsync(x => x.FKUserType == 1 || x.FKUserType == 2);

                model.MainProductsList = SelectListHelper.BindSelectList(allMainProducts.ToList(), model.FKMainProductId).ToList();
                model.SubProductsList = SelectListHelper.BindSelectList(allSubProducts.Where(x => x.FKMainProductId == model.FKMainProductId).ToList(), model.FKSubProductId).ToList();
                model.StatusesList = SelectListHelper.BindSelectList(allStatuses.ToList(), model.StatusId).ToList();
                model.UsersList = SelectListHelper.BindSelectList(allUserHasIndividualsOnly.ToList(), null, "Id", "FullNameAr", "FullNameEn").ToList();

                return View(model);
            }

            var entity = _mapper.Map<Domain.Entities.Product.OrderSellToClient>(model);

            // Calculate total based on selected checkboxes
            if (model.IsUnitsSelected && model.UnitsValue.HasValue && model.SellPriceUnit.HasValue)
            {
                entity.CountUnits = (int?)model.UnitsValue.Value;
            }
            //if (model.IsKilosSelected && model.KilosValue.HasValue && model.SellPriceKilo.HasValue)
            //{
            //    entity.Kilo = model.KilosValue.Value;
            //}
            //if (model.IsTonSelected && model.TonValue.HasValue && model.SellPriceTon.HasValue)
            //{
            //    // Convert ton to kilo (1 ton = 1000 kilo)
            //    entity.Kilo = (entity.Kilo ?? 0) + (model.TonValue.Value * 1000);
            //}

            // Calculate total
            double total = 0;
            if (model.IsUnitsSelected && model.UnitsValue.HasValue && model.SellPriceUnit.HasValue)
            {
                total += model.UnitsValue.Value * model.SellPriceUnit.Value;
            }
            if (model.IsKilosSelected && model.KilosValue.HasValue && model.SellPriceKilo.HasValue)
            {
                total += model.KilosValue.Value * model.SellPriceKilo.Value;
            }
            if (model.IsTonSelected && model.TonValue.HasValue && model.SellPriceTon.HasValue)
            {
                total += model.TonValue.Value * model.SellPriceTon.Value;
            }

            // Points discount logic: only for new orders by Client
            bool pointsDiscountApplied = false;
            if (model.Id == 0 && isClientUser && model.UsePointsDiscount && !string.IsNullOrEmpty(loggedInUserId))
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
                entity.DiscountRatio = isClientUser ? null : model.DiscountRatio;
            }

            // Apply discount
            if (entity.DiscountRatio.HasValue && entity.DiscountRatio.Value > 0)
            {
                entity.DiscountValue = total * (entity.DiscountRatio.Value / 100);
                total -= entity.DiscountValue.Value;
            }
            else if (!isClientUser && model.DiscountValue.HasValue && model.DiscountValue.Value > 0)
            {
                total -= model.DiscountValue.Value;
            }

            entity.Total = total;

            if (model.Id == 0)
            {
                await _orderSellToClientService.AddAsync(entity);
                if (pointsDiscountApplied)
                    await _unitOfWork.CompleteAsync();
                return RedirectToAction(nameof(AddEdit), new { id = entity.Id });
            }
            FKUserId = entity.FKUserId;

            await _orderSellToClientService.UpdateAsync(entity);

            var financial = await _unitOfWork.Financials.Table.FirstOrDefaultAsync(x => x.TableType == "OrderSellToClient" && x.ItsId == model.Id);
            if (financial != null)
            {
                financial.Total = entity.Total;
                var FKUserType = await _PermissionScanner.GetFKUserType(FKUserId);
                financial.FKUserType = FKUserType;
                financial.FKUserId = FKUserId;
                _unitOfWork.Financials.Update(financial);
                await _unitOfWork.CompleteAsync();
            }

            return RedirectToAction(nameof(AddEdit), new { id = model.Id });
        }


        [YesGet]
        public async Task<IActionResult> UpdateStatus(int? id)
        {

            var vm = new OrderSellToClientVM();
            var allMainProducts = await _unitOfWork.MainProducts.GetAllAsync();
            var allSubProducts = await _unitOfWork.SubProducts.GetAllAsync();
            var allStatuses = await _unitOfWork.Statuses.GetAllAsync();
            var allUserHasIndividualsOnly = await _unitOfWork.Users.GetAllAsync(x => x.FKUserType == 1 || x.FKUserType == 2);

            var loggedInUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var loggedInUser = !string.IsNullOrEmpty(loggedInUserId) ? await _unitOfWork.Users.GetByIdAsync(loggedInUserId) : null;
            var isClientUser = loggedInUser != null && (loggedInUser.FKUserType == 1 || loggedInUser.FKUserType == 2); // شركة او فرد

            vm.IsClientUser = isClientUser;
            if (isClientUser)
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
            vm.UsersList = SelectListHelper.BindSelectList(allUserHasIndividualsOnly.ToList(), null, "Id", "FullNameAr", "FullNameEn").ToList();

            if (!id.HasValue || id.Value == 0)
            {
                vm.OrderDate = AppDubaiTime.Now;
                return View(vm);
            }

            var entity = await _orderSellToClientService.GetByIdAsync(id.Value);
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

            vm = _mapper.Map<OrderSellToClientVM>(entity);
            vm.IsClientUser = isClientUser;
            if (isClientUser)
            {
                vm.FKUserId = loggedInUserId;
            }
            if (entity.SubProduct != null)
            {
                vm.FKMainProductId = entity.SubProduct.FKMainProductId;
                vm.SellPriceUnit = entity.SubProduct.SellPriceUnit;
                vm.SellPriceKilo = entity.SubProduct.SellPriceKilo;
                vm.SellPriceTon = entity.SubProduct.SellPriceTon;
            }

            // Determine which checkboxes should be checked based on stored values
            if (entity.CountUnits.HasValue && entity.CountUnits.Value > 0)
            {
                vm.IsUnitsSelected = true;
                vm.UnitsValue = entity.CountUnits.Value;
            }

            //if (entity.Kilo.HasValue && entity.Kilo.Value > 0)
            //{
            //    // Check if kilo value is less than 1000, treat as kilos
            //    if (entity.Kilo.Value < 1000)
            //    {
            //        vm.IsKilosSelected = true;
            //        vm.KilosValue = entity.Kilo.Value;
            //    }
            //    else
            //    {
            //        // If kilo value is 1000 or more, treat as tons
            //        vm.IsTonSelected = true;
            //        vm.TonValue = entity.Kilo.Value / 1000;
            //    }
            //}

            vm.MainProductsList = SelectListHelper.BindSelectList(allMainProducts.ToList(), vm.FKMainProductId).ToList();
            vm.SubProductsList = SelectListHelper.BindSelectList(allSubProducts.Where(x => x.FKMainProductId == vm.FKMainProductId).ToList(), vm.FKSubProductId).ToList();
            var statusesToBind = allStatuses.ToList();
            if (currentStatusObj != null && currentStatusObj.ShortChar == "D")
            {
                statusesToBind = statusesToBind.Where(s => s.ShortChar == "S" || s.ShortChar == "C" || s.Id == currentStatusObj.Id).ToList();
            }
            vm.StatusesList = SelectListHelper.BindSelectList(statusesToBind, vm.StatusId).ToList();
            vm.UsersList = SelectListHelper.BindSelectList(allUserHasIndividualsOnly.ToList(), null, "Id", "FullNameAr", "FullNameEn").ToList();

            return View(vm);
        }

        [IgnoreAction]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(OrderSellToClientVM model)
        {
            var loggedInUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var loggedInUser = !string.IsNullOrEmpty(loggedInUserId) ? await _unitOfWork.Users.GetByIdAsync(loggedInUserId) : null;
            var isClientUser = loggedInUser != null && (loggedInUser.FKUserType == 1 || loggedInUser.FKUserType == 2); // شركة او فرد
            var FKUserId = "";
            if (isClientUser)
            {
                model.FKUserId = loggedInUserId;
                model.IsClientUser = true;
            }

            if (!ModelState.IsValid)
            {
                var oldEntityForView = await _unitOfWork.OrderSellToClients.Table
                    .Include(x => x.Status)
                    .Include(x => x.SubProduct)
                    .FirstOrDefaultAsync(x => x.Id == model.Id);

                if (oldEntityForView != null)
                {
                    await PopulateUpdateStatusViewModelAsync(model, oldEntityForView);
                }

                return View(model);
            }

            var oldEntity = await _unitOfWork.OrderSellToClients.Table
                .Include(x => x.Status)
                .Include(x => x.SubProduct)
                .FirstOrDefaultAsync(x => x.Id == model.Id);
            if (oldEntity == null)
            {
                return NotFound();
            }

            string oldStatusChar = oldEntity.Status?.ShortChar ?? string.Empty;

            var newStatus = await _unitOfWork.Statuses.Table.FirstOrDefaultAsync(x => x.Id == model.StatusId);

            StatusTransitionValidationResult? transitionValidation = null;
            var requiresValidation = OrderSellToClientStatusValidator.RequiresBalanceAndRoomValidation(newStatus?.ShortChar, oldStatusChar);
            if (requiresValidation)
            {
                transitionValidation = await OrderSellToClientStatusValidator.ValidateTransitionToDoneAsync(_unitOfWork, oldEntity);
                if (!transitionValidation.Success)
                {
                    ModelState.AddModelError(string.Empty, transitionValidation.ErrorMessage!);
                    model.StatusId = oldEntity.StatusId;
                    await PopulateUpdateStatusViewModelAsync(model, oldEntity);
                    return View(model);
                }
            }

            oldEntity.StatusId = model.StatusId;
            FKUserId = oldEntity.FKUserId;
            await _orderSellToClientService.UpdateAsync(oldEntity);

            if (newStatus != null)
            {
                var financial = await _unitOfWork.Financials.Table.FirstOrDefaultAsync(x => x.TableType == "OrderSellToClient" && x.ItsId == model.Id);

                if (oldStatusChar == "P" && newStatus.ShortChar == "D" && financial == null)
                {
                    var FKUserType = await _PermissionScanner.GetFKUserType(FKUserId);
                    await _unitOfWork.Financials.AddAsync(new Domain.Entities.Financial
                    {
                        TableType = "OrderSellToClient",
                        ItsId = model.Id,
                        TypeTransaction = '-',
                        StatusId = model.StatusId,
                        Total = oldEntity.Total,
                        FKUserType = FKUserType,
                        FKUserId = FKUserId,
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
                    && OrderSellToClientStatusValidator.RequiresRoomDeduction(newStatus.ShortChar, oldStatusChar))
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

            var entity = await _orderSellToClientService.GetByIdAsync(id);
            if (entity == null)
            {
                return NotFound();
            }

            var vm = _mapper.Map<OrderSellToClientVM>(entity);

            var loggedInUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var loggedInUser = !string.IsNullOrEmpty(loggedInUserId) ? await _unitOfWork.Users.GetByIdAsync(loggedInUserId) : null;
            var isClientUser = loggedInUser != null &&( loggedInUser.FKUserType == 1 || loggedInUser.FKUserType == 2); // شركة او فرد

            vm.IsClientUser = isClientUser;
            if (isClientUser)
            {
                vm.FKUserId = loggedInUserId;
            }

            if (entity.SubProduct != null)
            {
                vm.FKMainProductId = entity.SubProduct.FKMainProductId;
                vm.SellPriceUnit = entity.SubProduct.SellPriceUnit;
                vm.SellPriceKilo = entity.SubProduct.SellPriceKilo;
                vm.SellPriceTon = entity.SubProduct.SellPriceTon;
            }

            // Determine which checkboxes should be checked based on stored values
            if (entity.CountUnits.HasValue && entity.CountUnits.Value > 0)
            {
                vm.IsUnitsSelected = true;
                vm.UnitsValue = entity.CountUnits.Value;
            }

            //if (entity.Kilo.HasValue && entity.Kilo.Value > 0)
            //{
            //    if (entity.Kilo.Value < 1000)
            //    {
            //        vm.IsKilosSelected = true;
            //        vm.KilosValue = entity.Kilo.Value;
            //    }
            //    else
            //    {
            //        vm.IsTonSelected = true;
            //        vm.TonValue = entity.Kilo.Value / 1000;
            //    }
            //}

            var allMainProducts = await _unitOfWork.MainProducts.GetAllAsync();
            var allSubProducts = await _unitOfWork.SubProducts.GetAllAsync();
            var allStatuses = await _unitOfWork.Statuses.GetAllAsync();
            var allUserHasIndividualsOnly = await _unitOfWork.Users.GetAllAsync(x => x.FKUserType == 1 || x.FKUserType == 2);

            vm.MainProductsList = SelectListHelper.BindSelectList(allMainProducts.ToList(), vm.FKMainProductId).ToList();
            vm.SubProductsList = SelectListHelper.BindSelectList(allSubProducts.Where(x => x.FKMainProductId == vm.FKMainProductId).ToList(), vm.FKSubProductId).ToList();
            vm.StatusesList = SelectListHelper.BindSelectList(allStatuses.ToList(), vm.StatusId).ToList();
            vm.UsersList = SelectListHelper.BindSelectList(allUserHasIndividualsOnly.ToList(), null, "Id", "FullNameAr", "FullNameEn").ToList();

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

            var hasRelated = await _orderSellToClientService.HasRelatedObjectsInDb(id);
            if (hasRelated)
            {
                TempData["Error"] = "Cannot delete this item because it has related records.";
                return RedirectToAction(nameof(Index));
            }

            await _orderSellToClientService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [IgnoreAction]
        [HttpGet]
        public async Task<IActionResult> GetSubProductsByMainProduct(int mainProductId)
        {
            var subProducts = await _unitOfWork.SubProducts.Table
                .Where(x => x.FKMainProductId == mainProductId)
                .Select(x => new { x.Id, x.NameAr, x.NameEn, x.SellPriceUnit, x.SellPriceKilo, x.SellPriceTon })
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
                buyPriceUnit = subProduct.SellPriceUnit,
                buyPriceKilo = subProduct.SellPriceKilo,
                buyPriceTon = subProduct.SellPriceTon
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
            var items = await _orderSellToClientService.GetAllAsync(searchTerm, mainProductId, subProductId);

            var loggedInUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var loggedInUser = !string.IsNullOrEmpty(loggedInUserId) ? await _unitOfWork.Users.GetByIdAsync(loggedInUserId) : null;
            var isClientUser = loggedInUser != null &&( loggedInUser.FKUserType == 1 || loggedInUser.FKUserType == 2); // شركة او فرد

            if (isClientUser)
            {
                items = items.Where(x => x.FKUserId == loggedInUserId).ToList();
            }

            var allMainProducts = await _unitOfWork.MainProducts.GetAllAsync();
            var allSubProducts = await _unitOfWork.SubProducts.GetAllAsync();
            var allStatuses = await _unitOfWork.Statuses.GetAllAsync();
            var allUserHasIndividualsOnly = await _unitOfWork.Users.GetAllAsync(x => x.FKUserType == 1 || x.FKUserType == 2);

            var vm = new OrderSellToClientVM
            {
                Items = items,
                SearchString = searchTerm,
                MainProductFilterId = mainProductId,
                SubProductFilterId = subProductId,
                TotalCount = items.Count(),
                MainProductsList = SelectListHelper.BindSelectList(allMainProducts.ToList(), mainProductId).ToList(),
                SubProductsList = SelectListHelper.BindSelectList(allSubProducts.ToList(), subProductId).ToList(),
                StatusesList = SelectListHelper.BindSelectList(allStatuses.ToList(), null).ToList(),
                UsersList = SelectListHelper.BindSelectList(allUserHasIndividualsOnly.ToList(), null, "Id", "FullNameAr", "FullNameEn").ToList()
            };

            return View(vm);
        }

        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> createExcelReport_Download(string? searchTerm, int? mainProductId, int? subProductId)
        {
            var items = await _orderSellToClientService.GetAllAsync(searchTerm, mainProductId, subProductId);

            var loggedInUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var loggedInUser = !string.IsNullOrEmpty(loggedInUserId) ? await _unitOfWork.Users.GetByIdAsync(loggedInUserId) : null;
            var isClientUser = loggedInUser != null &&( loggedInUser.FKUserType == 1 || loggedInUser.FKUserType == 2); // شركة او فرد

            if (isClientUser)
            {
                items = items.Where(x => x.FKUserId == loggedInUserId).ToList();
            }

            var list = items.ToList();
            var allUserHasIndividualsOnly = await _unitOfWork.Users.GetAllAsync(x => x.FKUserType == 1 || x.FKUserType == 2);
            var usersList = SelectListHelper.BindSelectList(allUserHasIndividualsOnly.ToList(), null, "Id", "FullNameAr", "FullNameEn").ToList();

            if (!list.Any())
            {
                return RedirectToAction(nameof(Index), new { searchTerm, mainProductId, subProductId });
            }

            var titles = new List<string> { "Client", "Sub Product", "Main Product", "Order Date", "Units", "Discount", "Total", "Status" };
            var excelData = list.Select(x => new ExcelDataDTO
            {
                t1 = usersList.FirstOrDefault(u => u.Value == x.FKUserId)?.Text ?? string.Empty,
                t2 = SessionHelper.GetCurrentLanguage() == "ar" ? x.SubProduct?.NameAr ?? string.Empty : x.SubProduct?.NameEn ?? string.Empty,
                t3 = SessionHelper.GetCurrentLanguage() == "ar" ? x.SubProduct?.MainProduct?.NameAr ?? string.Empty : x.SubProduct?.MainProduct?.NameEn ?? string.Empty,
                t4 = x.OrderDate?.ToString("yyyy-MM-dd") ?? string.Empty,
                t5 = x.CountUnits?.ToString() ?? string.Empty,
                t6 = x.DiscountValue?.ToString("F2") ?? string.Empty,
                t7 = x.Total?.ToString("F2") ?? string.Empty,
                t8 = SessionHelper.GetCurrentLanguage() == "ar" ? x.Status?.NameAr ?? string.Empty : x.Status?.NameEn ?? string.Empty
            }).ToList();

            var (ok, bytes) = ExcelStaticReport.ExcelReportArEn_(excelData, titles, 0, "ar");
            if (!ok || bytes == null || bytes.Length == 0)
            {
                return RedirectToAction(nameof(Index), new { searchTerm, mainProductId, subProductId });
            }

            return File(
                bytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"OrderSellToClients_{AppDubaiTime.Now:yyyyMMdd_HHmmss}.xlsx");
        }

        [IgnoreAction]
        [HttpGet]
        public async Task<IActionResult> GetOrderAttachments(int id, string returnAction = "AddEdit")
        {
            var attachments = await _unitOfWork.OrderSellToClientAttachments.Table
                .Where(a => a.OrderSellToClientId == id)
                .ToListAsync();

            var vm = new OrderSellToClientAttachmentsVM
            {
                OrderSellToClientId = id,
                ReturnAction = returnAction,
                Attachments = attachments
            };

            return PartialView("_OrderAttachmentsModal", vm);
        }

        [IgnoreAction]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadOrderFiles(OrderSellToClientAttachmentsVM model)
        {
            var uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Files", "Orders");
            if (!Directory.Exists(uploadDir))
            {
                Directory.CreateDirectory(uploadDir);
            }

            // 1. Get existing attachments from DB
            var existingAttachments = await _unitOfWork.OrderSellToClientAttachments.Table
                .Where(a => a.OrderSellToClientId == model.OrderSellToClientId)
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
                _unitOfWork.OrderSellToClientAttachments.Delete(toRemove);
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

                        var newAttachment = new Domain.Entities.Product.OrderSellToClientAttachment
                        {
                            Name = attachment.Name ?? attachment.File.FileName,
                            Path = $"Files/Orders/{fileName}",
                            OrderSellToClientId = model.OrderSellToClientId
                        };

                        await _unitOfWork.OrderSellToClientAttachments.AddAsync(newAttachment);
                    }
                }
            }

            await _unitOfWork.CompleteAsync();

            TempData["Success"] = Domain.Resources.Resource2.ToastDone;

            if (string.Equals(model.ReturnAction, nameof(UpdateStatus), StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction(nameof(UpdateStatus), new { id = model.OrderSellToClientId });
            }

            return RedirectToAction(nameof(AddEdit), new { id = model.OrderSellToClientId });
        }

        private async Task PopulateUpdateStatusViewModelAsync(OrderSellToClientVM model, OrderSellToClient entity)
        {
            var allMainProducts = await _unitOfWork.MainProducts.GetAllAsync();
            var allSubProducts = await _unitOfWork.SubProducts.GetAllAsync();
            var allStatuses = await _unitOfWork.Statuses.GetAllAsync();
            var allUserHasIndividualsOnly = await _unitOfWork.Users.GetAllAsync(x => x.FKUserType == 1 || x.FKUserType == 2);

            model.FKSubProductId ??= entity.FKSubProductId;
            model.Total ??= entity.Total;
            model.Address ??= entity.Address;
            model.DiscountRatio ??= entity.DiscountRatio;
            model.DiscountValue ??= entity.DiscountValue;

            if (entity.SubProduct != null)
            {
                model.FKMainProductId = entity.SubProduct.FKMainProductId;
                model.SellPriceUnit = entity.SubProduct.SellPriceUnit;
                model.SellPriceKilo = entity.SubProduct.SellPriceKilo;
                model.SellPriceTon = entity.SubProduct.SellPriceTon;
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

            //if (entity.Kilo.HasValue && entity.Kilo.Value > 0)
            //{
            //    if (entity.Kilo.Value < 1000)
            //    {
            //        model.IsKilosSelected = true;
            //        model.KilosValue = entity.Kilo.Value;
            //    }
            //    else
            //    {
            //        model.IsTonSelected = true;
            //        model.TonValue = entity.Kilo.Value / 1000;
            //    }
            //}

            model.MainProductsList = SelectListHelper.BindSelectList(allMainProducts.ToList(), model.FKMainProductId).ToList();
            model.SubProductsList = SelectListHelper.BindSelectList(allSubProducts.Where(x => x.FKMainProductId == model.FKMainProductId).ToList(), model.FKSubProductId).ToList();

            var statusesToBind = allStatuses.ToList();
            if (entity.Status?.ShortChar == "D")
            {
                statusesToBind = statusesToBind.Where(s => s.ShortChar == "S" || s.ShortChar == "C" || s.Id == entity.StatusId).ToList();
            }

            model.StatusesList = SelectListHelper.BindSelectList(statusesToBind, model.StatusId).ToList();
            model.UsersList = SelectListHelper.BindSelectList(allUserHasIndividualsOnly.ToList(), null, "Id", "FullNameAr", "FullNameEn").ToList();
        }
    }
}
