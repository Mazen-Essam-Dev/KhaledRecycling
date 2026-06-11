using KhaledTeamRecycling.Areas.Admin.ViewModels.Statistics;
using KhaledTeamRecycling.Attributes;
using KhaledTeamRecycling.Middelware;
using Infrastructure.Identity;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KhaledTeamRecycling.Areas.Admin.Controllers
{
    [AdminAuthorize]
    [Area("Admin")]
    public class StatisticsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StatisticsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [YesGet]
        public async Task<IActionResult> Index()
        {
            var vm = new StatisticsVM();

            var trackedUserTypes = new[] { 1, 2, 3 };
            var userTypeNames = await _context.UserTypes
                .AsNoTracking()
                .Where(x => trackedUserTypes.Contains(x.Id))
                .ToDictionaryAsync(x => x.Id, x => x.NameAr ?? x.NameEn ?? $"نوع {x.Id}");

            vm.TopUsersByType = await BuildTopUsersByTypeAsync(userTypeNames);
            vm.TopUsersByAmountPerType = await BuildTopUsersByAmountPerTypeAsync(userTypeNames);
            vm.TopUsersByPendingCountPerType = await BuildTopUsersByPendingCountPerTypeAsync(userTypeNames);
            vm.TotalUsersWithPoints = await _context.UserPointss
                .AsNoTracking()
                .Where(x => x.FKUserId != null)
                .Select(x => x.FKUserId!)
                .Distinct()
                .CountAsync();

            var pendingFinancials = await _context.Financials
                .AsNoTracking()
                .Where(x => x.Status != null && x.Status.ShortChar == "S")
                .Select(x => new PendingFinancialRow(x.TableType, x.ItsId, x.FKUserType))
                .ToListAsync();

            vm.TotalPendingFinancials = pendingFinancials.Count;
            vm.PendingFinancialsByUserType = trackedUserTypes
                .Select(typeId => new SimpleChartItemVM
                {
                    Label = GetUserTypeName(userTypeNames, typeId),
                    Count = pendingFinancials.Count(x => x.FKUserType == typeId)
                })
                .ToList();

            var productOrderIds = pendingFinancials
                .Where(x => x.ItsId.HasValue && (x.TableType == "OrderBuyFromFactory" || x.TableType == "OrderSellToClient"))
                .Select(x => x.ItsId!.Value)
                .Distinct()
                .ToList();

            var wasteOrderIds = pendingFinancials
                .Where(x => x.ItsId.HasValue && (x.TableType == "OrderBuyFromClient" || x.TableType == "OrderSellToFactory"))
                .Select(x => x.ItsId!.Value)
                .Distinct()
                .ToList();

            var buyFromFactories = await _context.OrderBuyFromFactories
                .AsNoTracking()
                .Where(x => productOrderIds.Contains(x.Id))
                .Include(x => x.SubProduct)
                    .ThenInclude(x => x!.MainProduct)
                .ToDictionaryAsync(x => x.Id);

            var sellToClients = await _context.OrderSellToClients
                .AsNoTracking()
                .Where(x => productOrderIds.Contains(x.Id))
                .Include(x => x.SubProduct)
                    .ThenInclude(x => x!.MainProduct)
                .ToDictionaryAsync(x => x.Id);

            var buyFromClients = await _context.OrderBuyFromClients
                .AsNoTracking()
                .Where(x => wasteOrderIds.Contains(x.Id))
                .Include(x => x.SubWaste)
                    .ThenInclude(x => x!.MainWaste)
                .ToDictionaryAsync(x => x.Id);

            var sellToFactories = await _context.OrderSellToFactories
                .AsNoTracking()
                .Where(x => wasteOrderIds.Contains(x.Id))
                .Include(x => x.SubWaste)
                    .ThenInclude(x => x!.MainWaste)
                .ToDictionaryAsync(x => x.Id);

            var subProductCounts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            var mainProductCounts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            var subWasteCounts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            var mainWasteCounts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

            foreach (var financial in pendingFinancials)
            {
                if (!financial.ItsId.HasValue)
                {
                    continue;
                }

                switch (financial.TableType)
                {
                    case "OrderBuyFromFactory" when buyFromFactories.TryGetValue(financial.ItsId.Value, out var buyFromFactory):
                        AddCount(
                            subProductCounts,
                            GetDetailedName(
                                buyFromFactory.SubProduct?.NameAr,
                                buyFromFactory.SubProduct?.NameEn,
                                buyFromFactory.SubProduct?.MainProduct?.NameAr,
                                buyFromFactory.SubProduct?.MainProduct?.NameEn));
                        AddCount(mainProductCounts, GetDisplayName(buyFromFactory.SubProduct?.MainProduct?.NameAr, buyFromFactory.SubProduct?.MainProduct?.NameEn, "غير محدد"));
                        break;

                    case "OrderSellToClient" when sellToClients.TryGetValue(financial.ItsId.Value, out var sellToClient):
                        AddCount(
                            subProductCounts,
                            GetDetailedName(
                                sellToClient.SubProduct?.NameAr,
                                sellToClient.SubProduct?.NameEn,
                                sellToClient.SubProduct?.MainProduct?.NameAr,
                                sellToClient.SubProduct?.MainProduct?.NameEn));
                        AddCount(mainProductCounts, GetDisplayName(sellToClient.SubProduct?.MainProduct?.NameAr, sellToClient.SubProduct?.MainProduct?.NameEn, "غير محدد"));
                        break;

                    case "OrderBuyFromClient" when buyFromClients.TryGetValue(financial.ItsId.Value, out var buyFromClient):
                        AddCount(
                            subWasteCounts,
                            GetDetailedName(
                                buyFromClient.SubWaste?.NameAr,
                                buyFromClient.SubWaste?.NameEn,
                                buyFromClient.SubWaste?.MainWaste?.NameAr,
                                buyFromClient.SubWaste?.MainWaste?.NameEn));
                        AddCount(mainWasteCounts, GetDisplayName(buyFromClient.SubWaste?.MainWaste?.NameAr, buyFromClient.SubWaste?.MainWaste?.NameEn, "غير محدد"));
                        break;

                    case "OrderSellToFactory" when sellToFactories.TryGetValue(financial.ItsId.Value, out var sellToFactory):
                        AddCount(
                            subWasteCounts,
                            GetDetailedName(
                                sellToFactory.SubWaste?.NameAr,
                                sellToFactory.SubWaste?.NameEn,
                                sellToFactory.SubWaste?.MainWaste?.NameAr,
                                sellToFactory.SubWaste?.MainWaste?.NameEn));
                        AddCount(mainWasteCounts, GetDisplayName(sellToFactory.SubWaste?.MainWaste?.NameAr, sellToFactory.SubWaste?.MainWaste?.NameEn, "غير محدد"));
                        break;
                }
            }

            vm.PendingFinancialsBySubProduct = ToChartItems(subProductCounts);
            vm.PendingFinancialsByMainProduct = ToChartItems(mainProductCounts);
            vm.PendingFinancialsBySubWaste = ToChartItems(subWasteCounts);
            vm.PendingFinancialsByMainWaste = ToChartItems(mainWasteCounts);

            return View(vm);
        }

        private async Task<List<UserTypeTopChartsVM>> BuildTopUsersByAmountPerTypeAsync(Dictionary<int, string> userTypeNames)
        {
            var topCandidates = await (
                from financial in _context.Financials.AsNoTracking()
                join user in _context.Users.AsNoTracking() on financial.FKUserId equals user.Id into userGroup
                from user in userGroup.DefaultIfEmpty()
                where financial.FKUserId != null
                    && financial.FKUserType.HasValue
                    && (financial.FKUserType == 1 || financial.FKUserType == 2 || financial.FKUserType == 3)
                group financial by new
                {
                    financial.FKUserType,
                    financial.FKUserId,
                    UserName = user != null
                        ? (user.FullNameAr ?? user.FullNameEn ?? user.UserName ?? financial.FKUserId!)
                        : (financial.FKUserId ?? "غير معروف")
                }
                into g
                select new TopFinancialAmountCandidateRow
                {
                    UserTypeId = g.Key.FKUserType!.Value,
                    UserName = g.Key.UserName,
                    TotalAmount = g.Sum(x => x.Total ?? 0d)
                })
                .ToListAsync();

            return new[] { 1, 2, 3 }
                .Select(typeId => new UserTypeTopChartsVM
                {
                    UserTypeId = typeId,
                    UserTypeName = GetUserTypeName(userTypeNames, typeId),
                    AmountItems = topCandidates
                        .Where(x => x.UserTypeId == typeId)
                        .OrderByDescending(x => x.TotalAmount)
                        .ThenBy(x => x.UserName)
                        .Take(3)
                        .Select(x => new DecimalChartItemVM
                        {
                            Label = x.UserName,
                            Value = Convert.ToDecimal(x.TotalAmount)
                        })
                        .ToList()
                })
                .ToList();
        }

        private async Task<List<UserTypeTopChartsVM>> BuildTopUsersByPendingCountPerTypeAsync(Dictionary<int, string> userTypeNames)
        {
            var topCandidates = await (
                from financial in _context.Financials.AsNoTracking()
                join user in _context.Users.AsNoTracking() on financial.FKUserId equals user.Id into userGroup
                from user in userGroup.DefaultIfEmpty()
                where financial.FKUserId != null
                    && financial.FKUserType.HasValue
                    && (financial.FKUserType == 1 || financial.FKUserType == 2 || financial.FKUserType == 3)
                    && financial.Status != null
                    && financial.Status.ShortChar == "S"
                group financial by new
                {
                    financial.FKUserType,
                    financial.FKUserId,
                    UserName = user != null
                        ? (user.FullNameAr ?? user.FullNameEn ?? user.UserName ?? financial.FKUserId!)
                        : (financial.FKUserId ?? "غير معروف")
                }
                into g
                select new TopFinancialCountCandidateRow
                {
                    UserTypeId = g.Key.FKUserType!.Value,
                    UserName = g.Key.UserName,
                    Count = g.Count()
                })
                .ToListAsync();

            return new[] { 1, 2, 3 }
                .Select(typeId => new UserTypeTopChartsVM
                {
                    UserTypeId = typeId,
                    UserTypeName = GetUserTypeName(userTypeNames, typeId),
                    CountItems = topCandidates
                        .Where(x => x.UserTypeId == typeId)
                        .OrderByDescending(x => x.Count)
                        .ThenBy(x => x.UserName)
                        .Take(3)
                        .Select(x => new SimpleChartItemVM
                        {
                            Label = x.UserName,
                            Count = x.Count
                        })
                        .ToList()
                })
                .ToList();
        }

        private async Task<List<TopUserStatVM>> BuildTopUsersByTypeAsync(Dictionary<int, string> userTypeNames)
        {
            var topCandidates = await (
                from user in _context.Users.AsNoTracking()
                join points in _context.UserPointss.AsNoTracking() on user.Id equals points.FKUserId
                where user.FKUserType.HasValue && (user.FKUserType == 1 || user.FKUserType == 2 || user.FKUserType == 3)
                group points by new
                {
                    user.Id,
                    user.FKUserType,
                    user.FullNameAr,
                    user.FullNameEn,
                    user.UserName
                }
                into g
                select new TopUserCandidateRow
                {
                    UserTypeId = g.Key.FKUserType!.Value,
                    UserName = g.Key.FullNameAr ?? g.Key.FullNameEn ?? g.Key.UserName ?? "غير معروف",
                    Total = g.Sum(x => x.Totals ?? 0m)
                })
                .ToListAsync();

            return new[] { 1, 2, 3 }
                .Select(typeId =>
                {
                    var topItem = topCandidates
                        .Where(x => x.UserTypeId == typeId)
                        .OrderByDescending(x => x.Total)
                        .ThenBy(x => x.UserName)
                        .FirstOrDefault();

                    return new TopUserStatVM
                    {
                        UserTypeId = typeId,
                        UserTypeName = GetUserTypeName(userTypeNames, typeId),
                        UserName = topItem?.UserName ?? "لا توجد بيانات",
                        Total = topItem?.Total ?? 0m
                    };
                })
                .ToList();
        }

        private static string GetUserTypeName(Dictionary<int, string> userTypeNames, int typeId)
        {
            if (userTypeNames.TryGetValue(typeId, out var value) && !string.IsNullOrWhiteSpace(value))
            {
                return value;
            }

            return typeId switch
            {
                1 => "فرد",
                2 => "شركة",
                3 => "مصنع",
                _ => $"نوع {typeId}"
            };
        }

        private static string GetDisplayName(string? ar, string? en, string fallback)
        {
            return !string.IsNullOrWhiteSpace(ar)
                ? ar
                : !string.IsNullOrWhiteSpace(en)
                    ? en
                    : fallback;
        }

        private static string GetDetailedName(string? childAr, string? childEn, string? parentAr, string? parentEn)
        {
            var childName = GetDisplayName(childAr, childEn, "غير محدد");
            var parentName = GetDisplayName(parentAr, parentEn, "غير محدد");

            return $"{childName} - {parentName}";
        }

        private static void AddCount(Dictionary<string, int> counts, string label)
        {
            if (counts.ContainsKey(label))
            {
                counts[label]++;
                return;
            }

            counts[label] = 1;
        }

        private static List<SimpleChartItemVM> ToChartItems(Dictionary<string, int> counts, int maxItems = 10)
        {
            var ordered = counts
                .OrderByDescending(x => x.Value)
                .ThenBy(x => x.Key)
                .ToList();

            if (ordered.Count <= maxItems)
            {
                return ordered
                    .Select(x => new SimpleChartItemVM { Label = x.Key, Count = x.Value })
                    .ToList();
            }

            var items = ordered
                .Take(maxItems)
                .Select(x => new SimpleChartItemVM { Label = x.Key, Count = x.Value })
                .ToList();

            var othersCount = ordered.Skip(maxItems).Sum(x => x.Value);
            if (othersCount > 0)
            {
                items.Add(new SimpleChartItemVM { Label = "أخرى", Count = othersCount });
            }

            return items;
        }

        private sealed class PendingFinancialRow
        {
            public PendingFinancialRow(string? tableType, int? itsId, int? fkUserType)
            {
                TableType = tableType;
                ItsId = itsId;
                FKUserType = fkUserType;
            }

            public string? TableType { get; }
            public int? ItsId { get; }
            public int? FKUserType { get; }
        }

        private sealed class TopUserCandidateRow
        {
            public int UserTypeId { get; set; }
            public string UserName { get; set; } = string.Empty;
            public decimal Total { get; set; }
        }

        private sealed class TopFinancialAmountCandidateRow
        {
            public int UserTypeId { get; set; }
            public string UserName { get; set; } = string.Empty;
            public double TotalAmount { get; set; }
        }

        private sealed class TopFinancialCountCandidateRow
        {
            public int UserTypeId { get; set; }
            public string UserName { get; set; } = string.Empty;
            public int Count { get; set; }
        }
    }
}
