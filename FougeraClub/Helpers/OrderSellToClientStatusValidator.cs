using Domain.Entities.Product;
using Infrastructure.Repositories.InterfacesDB;
using Microsoft.EntityFrameworkCore;

namespace KhaledTeamRecycling.Helpers
{
    public static class OrderSellToClientStatusValidator
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
            OrderSellToClient order)
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

            var requiredKilos = CalculateOrderKilos(order, subProduct);

            if (!order.FKSubProductId.HasValue || order.FKSubProductId.Value <= 0)
            {
                return new StatusTransitionValidationResult
                {
                    Success = false,
                    ErrorMessage = "لا يمكن تحديد نوع النفايات الفرعية لهذا الطلب"
                };
            }

            var rooms = await unitOfWork.RoomGalleries.Table
                .Where(x => x.FkSubProduct == order.FKSubProductId.Value && x.MaxUnit.HasValue)
                .ToListAsync();

            var fittingRooms = rooms
                .Where(x => (x.MaxUnit ?? 0) >= requiredKilos)
                .OrderByDescending(x => x.MaxUnit)
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

            var bestRoom = rooms.OrderByDescending(x => x.MaxUnit).FirstOrDefault();
            var availableKilos = bestRoom?.MaxUnit ?? 0;
            var kiloShortage = requiredKilos - availableKilos;

            return new StatusTransitionValidationResult
            {
                Success = false,
                ErrorMessage = $"المساحة المتاحة أقل بمقدار {kiloShortage:0.##} كيلو"
            };
        }

        public static double CalculateOrderKilos(OrderSellToClient order, SubProduct? subProduct)
        {
            double totalKilo = 0;

            //if (order.Kilo.HasValue && order.Kilo.Value > 0)
            //{
            //    totalKilo += order.Kilo.Value;
            //}

            if (order.CountUnits.HasValue && order.CountUnits.Value > 0
                && subProduct?.RatioCountFor1Kilo is > 0)
            {
                totalKilo += order.CountUnits.Value / subProduct.RatioCountFor1Kilo.Value;
            }

            return totalKilo;
        }
    }
}
