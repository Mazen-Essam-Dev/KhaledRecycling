using KhaledTeamRecycling.Areas.Admin.ViewModels.Statistics;
using KhaledTeamRecycling.Attributes;
using KhaledTeamRecycling.Middelware;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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
            var loggedInUserTypeId = await GetLoggedInUserTypeIdAsync();
            var visibleUserTypes = loggedInUserTypeId.HasValue && trackedUserTypes.Contains(loggedInUserTypeId.Value)
                ? new[] { loggedInUserTypeId.Value }
                : trackedUserTypes;

            vm.LoggedInUserTypeId = loggedInUserTypeId;
            vm.VisibleUserTypeIds = visibleUserTypes.ToList();

            var userTypeNames = await _context.UserTypes
                .AsNoTracking()
                .Where(x => trackedUserTypes.Contains(x.Id))
                .ToDictionaryAsync(x => x.Id, x => x.NameAr ?? x.NameEn ?? $"نوع {x.Id}");

            vm.TopUsersByType = await BuildTopUsersByTypeAsync(userTypeNames, visibleUserTypes);
            vm.TopUsersByAmountPerType = await BuildTopUsersByAmountPerTypeAsync(userTypeNames, visibleUserTypes);
            vm.TopUsersByPendingCountPerType = await BuildTopUsersByPendingCountPerTypeAsync(userTypeNames, visibleUserTypes);
            vm.TotalUsersWithPoints = await (
                from user in _context.Users.AsNoTracking()
                join points in _context.UserPointss.AsNoTracking() on user.Id equals points.FKUserId
                where user.FKUserType.HasValue && visibleUserTypes.Contains(user.FKUserType.Value)
                select user.Id)
                .Distinct()
                .CountAsync();

            var pendingFinancials = await (
                from financial in _context.Financials.AsNoTracking()
                join user in _context.Users.AsNoTracking() on financial.FKUserId equals user.Id into userGroup
                from user in userGroup.DefaultIfEmpty()
                where financial.Status != null && financial.Status.ShortChar == "S"
                select new PendingFinancialRow(
                    financial.TableType,
                    financial.ItsId,
                    financial.FKUserType ?? user!.FKUserType))
                .ToListAsync();

            var pendingFinancialsForVisibleTypes = pendingFinancials
                .Where(x => x.FKUserType.HasValue && visibleUserTypes.Contains(x.FKUserType.Value))
                .ToList();

            vm.TotalPendingFinancials = pendingFinancialsForVisibleTypes.Count;
            vm.PendingFinancialsByUserType = visibleUserTypes
                .Select(typeId => new SimpleChartItemVM
                {
                    Label = GetUserTypeName(userTypeNames, typeId),
                    Count = pendingFinancialsForVisibleTypes.Count(x => x.FKUserType == typeId)
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

        private async Task<List<UserTypeTopChartsVM>> BuildTopUsersByAmountPerTypeAsync(Dictionary<int, string> userTypeNames, IReadOnlyCollection<int> visibleUserTypes)
        {
            var topCandidates = await (
                from financial in _context.Financials.AsNoTracking()
                join user in _context.Users.AsNoTracking() on financial.FKUserId equals user.Id into userGroup
                from user in userGroup.DefaultIfEmpty()
                where financial.FKUserId != null
                    && (financial.FKUserType ?? user!.FKUserType).HasValue
                    && visibleUserTypes.Contains((financial.FKUserType ?? user!.FKUserType)!.Value)
                group financial by new
                {
                    EffectiveUserType = financial.FKUserType ?? user!.FKUserType,
                    financial.FKUserId,
                    UserName = user != null
                        ? (user.FullNameAr ?? user.FullNameEn ?? user.UserName ?? financial.FKUserId!)
                        : (financial.FKUserId ?? "غير معروف")
                }
                into g
                select new TopFinancialAmountCandidateRow
                {
                    UserTypeId = g.Key.EffectiveUserType!.Value,
                    UserName = g.Key.UserName,
                    TotalAmount = g.Sum(x => x.Total ?? 0d)
                })
                .ToListAsync();

            return visibleUserTypes
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

        private async Task<List<UserTypeTopChartsVM>> BuildTopUsersByPendingCountPerTypeAsync(Dictionary<int, string> userTypeNames, IReadOnlyCollection<int> visibleUserTypes)
        {
            var topCandidates = await (
                from financial in _context.Financials.AsNoTracking()
                join user in _context.Users.AsNoTracking() on financial.FKUserId equals user.Id into userGroup
                from user in userGroup.DefaultIfEmpty()
                where financial.FKUserId != null
                    && (financial.FKUserType ?? user!.FKUserType).HasValue
                    && visibleUserTypes.Contains((financial.FKUserType ?? user!.FKUserType)!.Value)
                    && financial.Status != null
                    && financial.Status.ShortChar == "S"
                group financial by new
                {
                    EffectiveUserType = financial.FKUserType ?? user!.FKUserType,
                    financial.FKUserId,
                    UserName = user != null
                        ? (user.FullNameAr ?? user.FullNameEn ?? user.UserName ?? financial.FKUserId!)
                        : (financial.FKUserId ?? "غير معروف")
                }
                into g
                select new TopFinancialCountCandidateRow
                {
                    UserTypeId = g.Key.EffectiveUserType!.Value,
                    UserName = g.Key.UserName,
                    Count = g.Count()
                })
                .ToListAsync();

            return visibleUserTypes
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

        private async Task<List<TopUserStatVM>> BuildTopUsersByTypeAsync(Dictionary<int, string> userTypeNames, IReadOnlyCollection<int> visibleUserTypes)
        {
            var topCandidates = await (
                from user in _context.Users.AsNoTracking()
                join points in _context.UserPointss.AsNoTracking() on user.Id equals points.FKUserId
                where user.FKUserType.HasValue && visibleUserTypes.Contains(user.FKUserType.Value)
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

            return visibleUserTypes
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

        private async Task<int?> GetLoggedInUserTypeIdAsync()
        {
            var loggedInUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrWhiteSpace(loggedInUserId))
            {
                return null;
            }

            return await _context.Users
                .AsNoTracking()
                .Where(x => x.Id == loggedInUserId)
                .Select(x => x.FKUserType)
                .FirstOrDefaultAsync();
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
