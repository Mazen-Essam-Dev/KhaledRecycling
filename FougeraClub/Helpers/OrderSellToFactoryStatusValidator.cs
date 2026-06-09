using Domain.Entities.Waste;
using Infrastructure.Repositories.InterfacesDB;
using Microsoft.EntityFrameworkCore;

namespace KhaledTeamRecycling.Helpers
{

    public static class OrderSellToFactoryStatusValidator
    {
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
            OrderSellToFactory order)
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

            var subWaste = order.SubWaste;
            if (subWaste == null && order.FKSubWasteId.HasValue)
            {
                subWaste = await unitOfWork.SubWastes.GetByIdAsync(order.FKSubWasteId.Value);
            }

            var requiredKilos = CalculateOrderKilos(order, subWaste);

            if (!order.FKSubWasteId.HasValue || order.FKSubWasteId.Value <= 0)
            {
                return new StatusTransitionValidationResult
                {
                    Success = false,
                    ErrorMessage = "لا يمكن تحديد نوع النفايات الفرعية لهذا الطلب"
                };
            }

            var rooms = await unitOfWork.RoomInventories.Table
                .Where(x => x.FKSubWaste == order.FKSubWasteId.Value && x.MaxKilo.HasValue)
                .ToListAsync();

            var fittingRooms = rooms
                .Where(x => (x.MaxKilo ?? 0) >= requiredKilos)
                .OrderByDescending(x => x.MaxKilo)
                .ToList();

            if (fittingRooms.Any())
            {
                var selectedRoom = fittingRooms.First();
                return new StatusTransitionValidationResult
                {
                    Success = true,
                    SelectedRoomId = selectedRoom.Id,
                    RequiredKilos = requiredKilos
                };
            }

            var bestRoom = rooms.OrderByDescending(x => x.MaxKilo).FirstOrDefault();
            var availableKilos = bestRoom?.MaxKilo ?? 0;
            var kiloShortage = requiredKilos - availableKilos;

            return new StatusTransitionValidationResult
            {
                Success = false,
                ErrorMessage = $"المساحة المتاحة أقل بمقدار {kiloShortage:0.##} كيلو"
            };
        }

        public static double CalculateOrderKilos(OrderSellToFactory order, SubWaste? subWaste)
        {
            double totalKilo = 0;

            if (order.Kilo.HasValue && order.Kilo.Value > 0)
            {
                totalKilo += order.Kilo.Value;
            }

            if (order.CountUnits.HasValue && order.CountUnits.Value > 0
                && subWaste?.RatioCountFor1Kilo is > 0)
            {
                totalKilo += order.CountUnits.Value / subWaste.RatioCountFor1Kilo.Value;
            }

            return totalKilo;
        }
    }
}
