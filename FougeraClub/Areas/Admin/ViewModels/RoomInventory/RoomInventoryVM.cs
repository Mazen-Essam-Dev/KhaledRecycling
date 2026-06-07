using Infrastructure.Repositories.InterfacesDB;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace KhaledTeamRecycling.Areas.Admin.ViewModels.RoomInventory
{
    public class RoomInventoryVM : IValidatableObject
    {
        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 50;
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }

        public int Id { get; set; }

        [LocalizedRequired("Required"), LocalizedMaxLength(100, "MaxLength_100")]
        public string? GenCode { get; set; }

        [LocalizedRequired("Required")]
        public int? FKSubWaste { get; set; }

        [LocalizedRequired("Required")]
        public double? MaxKilo { get; set; }

        [LocalizedRequired("Required")]
        public int? FkInventory { get; set; }

        [LocalizedMaxLength(500, "MaxLength_500")]
        public string? Description { get; set; }

        public List<SelectListItem>? InventoriesList { get; set; } = new();
        public List<SelectListItem>? SubWastesList { get; set; } = new();
        public IEnumerable<Domain.Entities.Inventory.RoomInventory>? Items { get; set; }
        public string? SearchString { get; set; }
        public int? InventoryFilterId { get; set; }
        public int? SubWasteFilterId { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var unitOfWork = (IUnitOfWork?)validationContext.GetService(typeof(IUnitOfWork));
            if (unitOfWork == null)
            {
                yield break;
            }

            if (FkInventory.HasValue && FkInventory.Value > 0)
            {
                var noRooms = unitOfWork.Inventories.Table
                    .Where(x => x.Id == FkInventory.Value)
                    .Select(x => x.NoRooms)
                    .FirstOrDefault();

                if (noRooms.HasValue)
                {
                    var existingCount = unitOfWork.RoomInventories.Table
                        .Count(x => x.FkInventory == FkInventory.Value && x.Id != Id);

                    if (existingCount >= noRooms.Value)
                    {
                        yield return new ValidationResult(
                            "لا يمكن إضافة المزيد من الغرف؛ تم الوصول إلى العدد المسموح به في هذا المخزن",
                            new[] { nameof(FkInventory) });
                    }
                }

                if (!string.IsNullOrWhiteSpace(Description))
                {
                    var description = Description.Trim().ToLower();
                    if (unitOfWork.RoomInventories.Any(x =>
                            x.FkInventory == FkInventory.Value &&
                            x.Id != Id &&
                            x.Description != null &&
                            x.Description.ToLower() == description))
                    {
                        yield return new ValidationResult(
                            "الاسم مكرر في نفس المخزن",
                            new[] { nameof(Description) });
                    }
                }

                if (!string.IsNullOrWhiteSpace(GenCode))
                {
                    var code = GenCode.Trim().ToLower();
                    if (unitOfWork.RoomInventories.Any(x =>
                            x.FkInventory == FkInventory.Value &&
                            x.Id != Id &&
                            x.GenCode != null &&
                            x.GenCode.ToLower() == code))
                    {
                        yield return new ValidationResult(
                            "الكود مكرر في نفس المخزن",
                            new[] { nameof(GenCode) });
                    }
                }
            }
        }
    }
}
