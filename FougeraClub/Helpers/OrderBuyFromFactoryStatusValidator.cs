using Domain.Entities.Product;
using Infrastructure.Repositories.InterfacesDB;
using Microsoft.EntityFrameworkCore;

namespace KhaledTeamRecycling.Helpers
{

    public static class OrderBuyFromFactoryStatusValidator
    {
        private static int CalculateAvailableRoomUnits(Domain.Entities.Gallery.RoomGallery room)
        {
            var maxUnit = room.MaxUnit ?? 0;
            var filledUnits = room.FilledUnits ?? 0;
            var reservedUnits = room.ReservedUnits ?? 0;
            return Math.Max(0, maxUnit - filledUnits - reservedUnits);
        }

        public static bool RequiresBalanceAndRoomValidation(string? newStatusChar, string oldStatusChar)
        {
            if (newStatusChar is not ("D" or "S") || oldStatusChar == newStatusChar)
            {
                return false;
            }

            // D -> S: already validated and room deducted when moved to D
            if (oldStatusChar == "D" && newStatusChar == "S")
            {
                return false;
            }

            return true;
        }

        public static bool RequiresRoomDeduction(string? newStatusChar, string oldStatusChar)
        {
            return RequiresBalanceAndRoomValidation(newStatusChar, oldStatusChar);
        }

        public static async Task<StatusTransitionValidationResult> ValidateTransitionToDoneAsync(
            IUnitOfWork unitOfWork,
            OrderBuyFromFactory order)
        {
            var orderTotal = (decimal)(order.Total ?? 0);

            var financialPlus = await unitOfWork.Financials.Table
                .Where(x => x.TypeTransaction == '+'
                    && x.Status != null
                    && x.Status.ShortChar == "S")
                .SumAsync(x => (decimal?)(x.Total ?? 0)) ?? 0;

            var financialMinus = await unitOfWork.Financials.Table
                .Where(x => x.TypeTransaction == '-'
                    && x.Status != null
                    && x.Status.ShortChar == "S")
                .SumAsync(x => (decimal?)(x.Total ?? 0)) ?? 0;

            var moneyPlus = await unitOfWork.MoneyPusheds.Table
                .Where(x => x.TypeTransaction == '+')
                .SumAsync(x => (decimal?)(x.Money ?? 0)) ?? 0;

            var moneyMinus = await unitOfWork.MoneyPusheds.Table
                .Where(x => x.TypeTransaction == '-')
                .SumAsync(x => (decimal?)(x.Money ?? 0)) ?? 0;

            var balance = (financialPlus - financialMinus) + (moneyPlus - moneyMinus) - orderTotal;

            if (balance <= 0)
            {
                var shortage = Math.Abs(balance);
                return new StatusTransitionValidationResult
                {
                    Success = false,
                    ErrorMessage = $"الرصيد لا يكفي بسبب فرق {shortage:0.##} جنيه"
                };
            }

            var subProduct = order.SubProduct;
            if (subProduct == null && order.FKSubProductId.HasValue)
            {
                subProduct = await unitOfWork.SubProducts.GetByIdAsync(order.FKSubProductId.Value);
            }

            var requiredUnits = CalculateOrderUnits(order);

            if (!order.FKSubProductId.HasValue || order.FKSubProductId.Value <= 0)
            {
                return new StatusTransitionValidationResult
                {
                    Success = false,
                    ErrorMessage = "لا يمكن تحديد المنتج الفرعي لهذا الطلب"
                };
            }

            var rooms = await unitOfWork.RoomGalleries.Table
                .Where(x => x.FkSubProduct == order.FKSubProductId.Value && x.MaxUnit.HasValue)
                .ToListAsync();

            var fittingRooms = rooms
                .Select(x => new
                {
                    Room = x,
                    AvailableUnits = CalculateAvailableRoomUnits(x)
                })
                .Where(x => x.AvailableUnits >= requiredUnits)
                .OrderBy(x => x.AvailableUnits)
                .ToList();

            if (fittingRooms.Any())
            {
                var selectedRoom = fittingRooms.First().Room;
                return new StatusTransitionValidationResult
                {
                    Success = true,
                    SelectedRoomId = selectedRoom.Id,
                    RequiredKilos = requiredUnits,
                    RoomAllocations = new List<RoomAllocationResult>
                    {
                        new()
                        {
                            RoomId = selectedRoom.Id,
                            AllocatedKilos = requiredUnits
                        }
                    }
                };
            }

            var availableRooms = rooms
                .Select(x => new
                {
                    Room = x,
                    AvailableUnits = CalculateAvailableRoomUnits(x)
                })
                .Where(x => x.AvailableUnits > 0)
                .OrderBy(x => x.AvailableUnits)
                .ToList();

            var totalAvailableUnits = availableRooms.Sum(x => x.AvailableUnits);
            if (totalAvailableUnits >= requiredUnits)
            {
                var remainingUnits = requiredUnits;
                var allocations = new List<RoomAllocationResult>();

                foreach (var room in availableRooms)
                {
                    if (remainingUnits <= 0)
                    {
                        break;
                    }

                    var roomAvailable = room.AvailableUnits;
                    if (roomAvailable <= 0)
                    {
                        continue;
                    }

                    var allocated = Math.Min(roomAvailable, remainingUnits);
                    allocations.Add(new RoomAllocationResult
                    {
                        RoomId = room.Room.Id,
                        AllocatedKilos = allocated
                    });
                    remainingUnits -= allocated;
                }

                if (remainingUnits <= 0 && allocations.Any())
                {
                    return new StatusTransitionValidationResult
                    {
                        Success = true,
                        SelectedRoomId = allocations.First().RoomId,
                        RequiredKilos = requiredUnits,
                        RoomAllocations = allocations
                    };
                }
            }

            var unitShortage = requiredUnits - totalAvailableUnits;

            return new StatusTransitionValidationResult
            {
                Success = false,
                ErrorMessage = $"المساحة المتاحة فقط ({totalAvailableUnits:0}) اي اقل بمقدار {unitShortage:0} وحدة"
            };
        }

        public static int CalculateOrderUnits(OrderBuyFromFactory order)
        {
            var countUnits = order.CountUnits ?? 0;
            return Math.Max(0, countUnits);
        }
    }
}
