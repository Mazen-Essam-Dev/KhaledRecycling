using Application.Helpers;
using Application.Services.Admin;
using Domain.DTOs;
using Infrastructure.Persistence;
using KhaledTeamRecycling.Areas.Admin.ViewModels.FinancialTimeline;
using KhaledTeamRecycling.Attributes;
using KhaledTeamRecycling.Middelware;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KhaledTeamRecycling.Areas.Admin.Controllers
{
    [AdminAuthorize]
    [Area("Admin")]
    public class FinancialTimelineController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FinancialTimelineController(ApplicationDbContext context)
        {
            _context = context;
        }

        [YesGet]
        public async Task<IActionResult> Index(
            string? searchTerm,
            DateOnly? dateFrom,
            DateOnly? dateTo,
            string? transactionFilter,
            string? statusFilter,
            string? sourceFilter,
            int page = 1,
            int pageSize = 50)
        {
            var allItems = await BuildItemsAsync();
            allItems = ApplyFilters(allItems, searchTerm, dateFrom, dateTo, transactionFilter, statusFilter, sourceFilter);

            var orderedItems = allItems
                .OrderBy(x => x.SortDate ?? DateTime.MaxValue)
                .ThenBy(x => x.SourceType)
                .ThenBy(x => x.Id)
                .ToList();

            var totalRecords = orderedItems.Count;
            var items = orderedItems
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var vm = new FinancialTimelineVM
            {
                Items = items,
                SearchString = searchTerm,
                DateFrom = dateFrom,
                DateTo = dateTo,
                TransactionFilter = transactionFilter,
                StatusFilter = statusFilter,
                SourceFilter = sourceFilter,
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
        public async Task<IActionResult> Print(
            string? searchTerm,
            DateOnly? dateFrom,
            DateOnly? dateTo,
            string? transactionFilter,
            string? statusFilter,
            string? sourceFilter)
        {
            var items = await BuildFilteredItemsAsync(searchTerm, dateFrom, dateTo, transactionFilter, statusFilter, sourceFilter);

            var vm = new FinancialTimelineVM
            {
                Items = items,
                SearchString = searchTerm,
                DateFrom = dateFrom,
                DateTo = dateTo,
                TransactionFilter = transactionFilter,
                StatusFilter = statusFilter,
                SourceFilter = sourceFilter,
                TotalCount = items.Count
            };

            return View(vm);
        }

        [YesGet]
        public async Task<IActionResult> createExcelReport_Download(
            string? searchTerm,
            DateOnly? dateFrom,
            DateOnly? dateTo,
            string? transactionFilter,
            string? statusFilter,
            string? sourceFilter)
        {
            var items = await BuildFilteredItemsAsync(searchTerm, dateFrom, dateTo, transactionFilter, statusFilter, sourceFilter);
            if (!items.Any())
            {
                return RedirectToAction(nameof(Index), new { searchTerm, dateFrom, dateTo, transactionFilter, statusFilter, sourceFilter });
            }

            var titles = new List<string>
            {
                "التاريخ",
                "المصدر",
                "نوع العملية",
                "الحركة",
                "الطرف",
                "اسم العميل / المصنع",
                "الصنف",
                "ملاحظات",
                "المبلغ",
                "الحالة"
            };

            var excelData = items.Select(x => new ExcelDataDTO
            {
                t1 = x.SortDate?.ToString("yyyy-MM-dd HH:mm") ?? string.Empty,
                t2 = x.SourceName ?? string.Empty,
                t3 = x.OperationType ?? string.Empty,
                t4 = GetTransactionName(x.TypeTransaction),
                t5 = x.PartyType ?? string.Empty,
                t6 = x.UserName ?? string.Empty,
                t7 = x.ItemName ?? string.Empty,
                t8 = x.Notes ?? string.Empty,
                t9 = x.Amount?.ToString("0.00") ?? string.Empty,
                t10 = x.StatusName ?? string.Empty
            }).ToList();

            var (ok, bytes) = ExcelStaticReport.ExcelReportArEn_(excelData, titles, 10, "ar", "تقرير الحركة المالية");
            if (!ok || bytes == null || bytes.Length == 0)
            {
                return RedirectToAction(nameof(Index), new { searchTerm, dateFrom, dateTo, transactionFilter, statusFilter, sourceFilter });
            }

            return File(
                bytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"FinancialTimeline_{AppDubaiTime.Now:yyyyMMdd_HHmmss}.xlsx");
        }

        private async Task<List<FinancialTimelineItemDTO>> BuildFilteredItemsAsync(
            string? searchTerm,
            DateOnly? dateFrom,
            DateOnly? dateTo,
            string? transactionFilter,
            string? statusFilter,
            string? sourceFilter)
        {
            var items = await BuildItemsAsync();
            return ApplyFilters(items, searchTerm, dateFrom, dateTo, transactionFilter, statusFilter, sourceFilter)
                .OrderBy(x => x.SortDate ?? DateTime.MaxValue)
                .ThenBy(x => x.SourceType)
                .ThenBy(x => x.Id)
                .ToList();
        }

        private async Task<List<FinancialTimelineItemDTO>> BuildItemsAsync()
        {
            var users = await _context.Users
                .AsNoTracking()
                .Select(x => new { x.Id, x.FullNameAr, x.FullNameEn, x.UserName })
                .ToDictionaryAsync(x => x.Id, x => x.FullNameAr ?? x.FullNameEn ?? x.UserName ?? x.Id);

            var financials = await _context.Financials
                .AsNoTracking()
                .Include(x => x.Status)
                .Where(x => x.Status != null && (x.Status.ShortChar == "D" || x.Status.ShortChar == "S"))
                .ToListAsync();

            var moneyPusheds = await _context.MoneyPusheds
                .AsNoTracking()
                .ToListAsync();

            var orderBuyFromClients = await _context.OrderBuyFromClients
                .AsNoTracking()
                .Include(x => x.SubWaste)
                    .ThenInclude(x => x!.MainWaste)
                .ToDictionaryAsync(x => x.Id);

            var orderSellToFactories = await _context.OrderSellToFactories
                .AsNoTracking()
                .Include(x => x.SubWaste)
                    .ThenInclude(x => x!.MainWaste)
                .ToDictionaryAsync(x => x.Id);

            var orderBuyFromFactories = await _context.OrderBuyFromFactories
                .AsNoTracking()
                .Include(x => x.SubProduct)
                    .ThenInclude(x => x!.MainProduct)
                .ToDictionaryAsync(x => x.Id);

            var orderSellToClients = await _context.OrderSellToClients
                .AsNoTracking()
                .Include(x => x.SubProduct)
                    .ThenInclude(x => x!.MainProduct)
                .ToDictionaryAsync(x => x.Id);

            var items = new List<FinancialTimelineItemDTO>();

            foreach (var financial in financials)
            {
                var detail = ResolveFinancialDetail(
                    financial.TableType,
                    financial.ItsId,
                    orderBuyFromClients,
                    orderSellToFactories,
                    orderBuyFromFactories,
                    orderSellToClients);

                items.Add(new FinancialTimelineItemDTO
                {
                    Id = financial.Id,
                    SourceType = "Financial",
                    SourceName = "Financials",
                    TableType = financial.TableType,
                    OperationType = detail.OperationType,
                    TypeTransaction = financial.TypeTransaction,
                    Amount = financial.Total,
                    UserName = GetUserName(users, financial.FKUserId),
                    PartyType = detail.PartyType,
                    ItemName = detail.ItemName,
                    Notes = detail.Notes,
                    StatusName = financial.Status?.NameAr ?? financial.Status?.NameEn,
                    StatusShortChar = financial.Status?.ShortChar,
                    ApprovedDate = financial.ApprovedDate,
                    CreatedDate = financial.CreatedDate,
                    SortDate = financial.ApprovedDate ?? financial.CreatedDate
                });
            }

            foreach (var moneyPushed in moneyPusheds)
            {
                items.Add(new FinancialTimelineItemDTO
                {
                    Id = moneyPushed.Id,
                    SourceType = "MoneyPushed",
                    SourceName = "MoneyPusheds",
                    TableType = "MoneyPushed",
                    OperationType = moneyPushed.TypeTransaction == '+' ? "إيداع" : moneyPushed.TypeTransaction == '-' ? "سحب" : "حركة مالية",
                    TypeTransaction = moneyPushed.TypeTransaction,
                    Amount = moneyPushed.Money.HasValue ? Convert.ToDouble(moneyPushed.Money.Value) : null,
                    UserName = GetUserName(users, moneyPushed.FKUserId),
                    PartyType = "مستخدم",
                    ItemName = moneyPushed.ItemName,
                    Notes = moneyPushed.Notes,
                    StatusName = "-",
                    StatusShortChar = null,
                    CreatedDate = moneyPushed.CreatedDate,
                    SortDate = moneyPushed.CreatedDate
                });
            }

            return items;
        }

        private static List<FinancialTimelineItemDTO> ApplyFilters(
            List<FinancialTimelineItemDTO> items,
            string? searchTerm,
            DateOnly? dateFrom,
            DateOnly? dateTo,
            string? transactionFilter,
            string? statusFilter,
            string? sourceFilter)
        {
            IEnumerable<FinancialTimelineItemDTO> query = items;

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(x =>
                    Contains(x.UserName, searchTerm) ||
                    Contains(x.ItemName, searchTerm) ||
                    Contains(x.Notes, searchTerm) ||
                    Contains(x.OperationType, searchTerm) ||
                    Contains(x.StatusName, searchTerm) ||
                    Contains(x.TableType, searchTerm));
            }

            if (dateFrom.HasValue)
            {
                var from = dateFrom.Value.ToDateTime(TimeOnly.MinValue);
                query = query.Where(x => x.SortDate >= from);
            }

            if (dateTo.HasValue)
            {
                var to = dateTo.Value.ToDateTime(TimeOnly.MaxValue);
                query = query.Where(x => x.SortDate <= to);
            }

            if (!string.IsNullOrWhiteSpace(transactionFilter))
            {
                query = query.Where(x => x.TypeTransaction?.ToString() == transactionFilter);
            }

            if (!string.IsNullOrWhiteSpace(statusFilter))
            {
                query = query.Where(x => x.StatusShortChar == statusFilter);
            }

            if (!string.IsNullOrWhiteSpace(sourceFilter))
            {
                query = query.Where(x => x.SourceType == sourceFilter);
            }

            return query.ToList();
        }

        private static bool Contains(string? source, string searchTerm)
        {
            return !string.IsNullOrWhiteSpace(source)
                && source.Contains(searchTerm, StringComparison.OrdinalIgnoreCase);
        }

        private static string GetUserName(Dictionary<string, string> users, string? userId)
        {
            return !string.IsNullOrWhiteSpace(userId) && users.TryGetValue(userId, out var name)
                ? name
                : "غير معروف";
        }

        private static string GetTransactionName(char? typeTransaction)
        {
            return typeTransaction == '+'
                ? "قبض / إيداع"
                : typeTransaction == '-'
                    ? "دفع / سحب"
                    : typeTransaction?.ToString() ?? "-";
        }

        private static FinancialDetail ResolveFinancialDetail(
            string? tableType,
            int? id,
            Dictionary<int, Domain.Entities.Waste.OrderBuyFromClient> orderBuyFromClients,
            Dictionary<int, Domain.Entities.Waste.OrderSellToFactory> orderSellToFactories,
            Dictionary<int, Domain.Entities.Product.OrderBuyFromFactory> orderBuyFromFactories,
            Dictionary<int, Domain.Entities.Product.OrderSellToClient> orderSellToClients)
        {
            if (!id.HasValue)
            {
                return new FinancialDetail("عملية مالية", string.Empty, string.Empty, string.Empty);
            }

            return tableType switch
            {
                "OrderBuyFromClient" when orderBuyFromClients.TryGetValue(id.Value, out var buyFromClient) =>
                    new FinancialDetail(
                        "شراء من عميل",
                        "عميل",
                        JoinName(buyFromClient.SubWaste?.MainWaste?.NameAr, buyFromClient.SubWaste?.NameAr),
                        buyFromClient.Address),

                "OrderSellToClient" when orderSellToClients.TryGetValue(id.Value, out var sellToClient) =>
                    new FinancialDetail(
                        "بيع إلى عميل",
                        "عميل",
                        JoinName(sellToClient.SubProduct?.MainProduct?.NameAr, sellToClient.SubProduct?.NameAr),
                        sellToClient.Address),

                "OrderBuyFromFactory" when orderBuyFromFactories.TryGetValue(id.Value, out var buyFromFactory) =>
                    new FinancialDetail(
                        "شراء من مصنع",
                        "مصنع",
                        JoinName(buyFromFactory.SubProduct?.MainProduct?.NameAr, buyFromFactory.SubProduct?.NameAr),
                        buyFromFactory.Address),

                "OrderSellToFactory" when orderSellToFactories.TryGetValue(id.Value, out var sellToFactory) =>
                    new FinancialDetail(
                        "بيع إلى مصنع",
                        "مصنع",
                        JoinName(sellToFactory.SubWaste?.MainWaste?.NameAr, sellToFactory.SubWaste?.NameAr),
                        sellToFactory.Address),

                _ => new FinancialDetail(tableType ?? "عملية مالية", string.Empty, string.Empty, string.Empty)
            };
        }

        private static string JoinName(string? parent, string? child)
        {
            if (string.IsNullOrWhiteSpace(parent))
            {
                return child ?? string.Empty;
            }

            if (string.IsNullOrWhiteSpace(child))
            {
                return parent;
            }

            return $"{parent} - {child}";
        }

        private sealed record FinancialDetail(string OperationType, string PartyType, string ItemName, string? Notes);
    }
}
